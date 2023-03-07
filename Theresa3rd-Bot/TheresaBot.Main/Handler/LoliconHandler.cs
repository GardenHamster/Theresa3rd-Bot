using TheresaBot.Main.Business;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Lolicon;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Handler
{
    public class LoliconHandler : SetuHandler
    {
        private LoliconBusiness loliconBusiness;

        public LoliconHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            loliconBusiness = new LoliconBusiness();
        }

        public async Task loliconSearchAsync(GroupCommand command)
        {
            try
            {
                CoolingCache.SetHanding(command.GroupId, command.MemberId);//请求处理中

                string tagStr = command.KeyWord;
                bool isShowAI = command.GroupId.IsShowAISetu();
                bool isShowR18 = command.GroupId.IsShowR18Setu();
                if (await CheckSetuTagEnableAsync(command, tagStr) == false) return;
                if (string.IsNullOrWhiteSpace(BotConfig.SetuConfig.ProcessingMsg) == false)
                {
                    await command.ReplyGroupTemplateWithAtAsync(BotConfig.SetuConfig.ProcessingMsg);
                    await Task.Delay(1000);
                }

                List<LoliconDataV2> dataList;
                int r18Mode = isShowR18 ? 2 : 0;
                bool excludeAI = isShowAI == false;

                if (string.IsNullOrEmpty(tagStr))
                {
                    dataList = await loliconBusiness.getLoliconDataListAsync(r18Mode, excludeAI, 1);
                }
                else
                {
                    if (await CheckSetuCustomEnableAsync(command) == false) return;
                    dataList = await loliconBusiness.getLoliconDataListAsync(r18Mode, excludeAI, 1, toLoliconTagArr(tagStr));
                }

                if (dataList.Count == 0)
                {
                    await command.ReplyGroupTemplateWithAtAsync(BotConfig.SetuConfig.NotFoundMsg, "找不到这类型的图片，换个标签试试吧~");
                    return;
                }

                LoliconDataV2 loliconData = dataList.First();
                if (await CheckSetuSendable(command, loliconData, isShowR18, isShowAI) == false) return;

                long todayLeftCount = GetSetuLeftToday(command.GroupId, command.MemberId);
                List<FileInfo> setuFiles = await GetSetuFilesAsync(loliconData, command.GroupId);

                string template = BotConfig.SetuConfig.Lolicon.Template;
                List<BaseContent> workMsgs = new List<BaseContent>();
                workMsgs.Add(new PlainContent(loliconBusiness.getWorkInfo(loliconData, todayLeftCount, template)));

                Task sendGroupTask = command.ReplyGroupSetuAndRevokeAsync(workMsgs, setuFiles, BotConfig.SetuConfig.RevokeInterval, true);
                if (BotConfig.SetuConfig.SendPrivate)
                {
                    await Task.Delay(1000);
                    Task sendTempTask = command.SendTempSetuAsync(workMsgs, setuFiles);
                }

                CoolingCache.SetMemberSetuCooling(command.GroupId, command.MemberId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "loliconSearchAsync异常");
                await command.ReplyError(ex);
                await Task.Delay(1000);
                Reporter.SendError(ex, "loliconSearchAsync异常");
            }
            finally
            {
                CoolingCache.SetHandFinish(command.GroupId, command.MemberId);//请求处理完成
            }
        }

        public async Task sendTimingSetuAsync(TimingSetuTimer timingSetuTimer, long groupId)
        {
            try
            {
                int margeEachPage = 5;
                bool sendMerge = timingSetuTimer.SendMerge;
                int r18Mode = groupId.IsShowR18Setu() ? 2 : 0;
                bool excludeAI = groupId.IsShowAISetu() == false;
                string tagStr = RandomHelper.getRandomItem(timingSetuTimer.Tags);
                string[] tagArr = string.IsNullOrWhiteSpace(tagStr) ? new string[0] : toLoliconTagArr(tagStr);
                int quantity = timingSetuTimer.Quantity > 20 ? 20 : timingSetuTimer.Quantity;
                List<LoliconDataV2> dataList = await loliconBusiness.getLoliconDataListAsync(r18Mode, excludeAI, quantity, tagArr);
                List<SetuContent> setuContents = await getSetuContent(dataList, groupId);
                await sendTimingSetuMessageAsync(timingSetuTimer, tagStr, groupId);
                await Task.Delay(2000);
                await Session.SendGroupSetuAsync(setuContents, groupId, sendMerge, margeEachPage);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "定时涩图异常");
                Reporter.SendError(ex, "定时涩图异常");
            }
        }

        private async Task<List<SetuContent>> getSetuContent(List<LoliconDataV2> datas, long groupId)
        {
            List<SetuContent> setuContents = new List<SetuContent>();
            foreach (var data in datas) setuContents.Add(await getSetuContent(data, groupId));
            return setuContents;
        }

        private async Task<SetuContent> getSetuContent(LoliconDataV2 data, long groupId)
        {
            string setuInfo = loliconBusiness.getDefaultWorkInfo(data);
            List<FileInfo> setuFiles = await GetSetuFilesAsync(data, groupId);
            return new SetuContent(setuInfo, setuFiles);
        }

    }
}
