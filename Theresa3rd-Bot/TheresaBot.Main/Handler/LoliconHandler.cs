using TheresaBot.Main.Business;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Exceptions;
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
                DateTime startDateTime = DateTime.Now;
                CoolingCache.SetHanding(command.GroupId, command.MemberId);//请求处理中

                bool isShowAI = command.GroupId.IsShowAISetu();
                bool isShowR18 = command.GroupId.IsShowR18Setu();
                string tagStr = command.KeyWord;
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

                bool isShowImg = command.GroupId.IsShowSetuImg(loliconData.IsR18);
                long todayLeftCount = GetSetuLeftToday(command.GroupId, command.MemberId);
                List<FileInfo> setuFiles = isShowImg ? await downPixivImgsAsync(loliconData) : new List<FileInfo>();

                string template = BotConfig.SetuConfig.Lolicon.Template;
                List<BaseContent> workMsgs = new List<BaseContent>();
                workMsgs.Add(new PlainContent(loliconBusiness.getWorkInfo(loliconData, startDateTime, todayLeftCount, template)));

                Task sendGroupTask = command.ReplyGroupSetuAndRevokeAsync(workMsgs, setuFiles, BotConfig.SetuConfig.RevokeInterval, true);

                if (BotConfig.SetuConfig.SendPrivate)
                {
                    await Task.Delay(1000);
                    Task sendTempTask = command.SendTempSetuAsync(workMsgs, setuFiles);
                }

                CoolingCache.SetMemberSetuCooling(command.GroupId, command.MemberId);
            }
            catch (ApiException ex)
            {
                string errMsg = $"loliconSearchAsync异常";
                LogHelper.Error(ex, errMsg);
                await command.ReplyGroupMessageWithAtAsync($"获取涩图出错了，{ex.Message}");
                Reporter.SendError(ex, errMsg);
            }
            catch (Exception ex)
            {
                string errMsg = "loliconSearchAsync异常";
                LogHelper.Error(ex, errMsg);
                await command.ReplyGroupTemplateWithAtAsync(BotConfig.GeneralConfig.ErrorMsg, "获取涩图出错了，再试一次吧~");
                Reporter.SendError(ex, errMsg);
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
                await Session.SendGroupSetuAsync(setuContents, groupId, sendMerge);
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
            bool isR18Img = data.IsR18;
            bool isShowImg = groupId.IsShowSetuImg(isR18Img);
            string setuInfo = loliconBusiness.getDefaultWorkInfo(data, DateTime.Now);
            List<FileInfo> setuFiles = isShowImg ? await downPixivImgsAsync(data) : new();
            return new SetuContent(setuInfo, setuFiles);
        }

    }
}
