using TheresaBot.Main.BotPlatform.Base.Command;
using TheresaBot.Main.Business;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Common;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Model.Lolicon;
using TheresaBot.Main.Helper;

namespace TheresaBot.Main.Handler
{
    public class LoliconHandler : SetuHandler
    {
        private LoliconBusiness loliconBusiness;

        public LoliconHandler()
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
                if (await CheckSetuTagEnableAsync(session, command.Args, tagStr) == false) return;
                if (string.IsNullOrWhiteSpace(BotConfig.SetuConfig.ProcessingMsg) == false)
                {
                    await session.SendTemplateWithAtAsync(command.Args, BotConfig.SetuConfig.ProcessingMsg, null);
                    await Task.Delay(1000);
                }

                LoliconResultV2 loliconResult = null;
                int r18Mode = isShowR18 ? 2 : 0;
                bool excludeAI = isShowAI == false;

                if (string.IsNullOrEmpty(tagStr))
                {
                    loliconResult = await loliconBusiness.getLoliconResultAsync(r18Mode, excludeAI, 1);
                }
                else
                {
                    if (await CheckSetuCustomEnableAsync(session, command.Args) == false) return;
                    string[] tagArr = toLoliconTagArr(tagStr);
                    loliconResult = await loliconBusiness.getLoliconResultAsync(r18Mode, excludeAI, 1, tagArr);
                }

                if (loliconResult is null || loliconResult.data is null || loliconResult.data.Count == 0)
                {
                    await session.SendTemplateWithAtAsync(command.Args, BotConfig.SetuConfig.NotFoundMsg, " 找不到这类型的图片，换个标签试试吧~");
                    return;
                }

                LoliconDataV2 loliconData = loliconResult.data.First();
                if (await CheckSetuSendable(session, command.Args, loliconData, isShowR18, isShowAI) == false) return;

                long todayLeftCount = GetSetuLeftToday(command.GroupId, command.MemberId);
                bool isShowImg = command.GroupId.IsShowSetuImg(loliconData.IsR18);
                List<FileInfo> setuFiles = isShowImg ? await loliconBusiness.downPixivImgsAsync(loliconData) : null;

                string template = BotConfig.SetuConfig.Lolicon.Template;
                List<IChatMessage> workMsgs = new List<IChatMessage>();
                if (string.IsNullOrWhiteSpace(template))
                {
                    workMsgs.Add(new PlainMessage(loliconBusiness.getDefaultWorkInfo(loliconData, startDateTime)));
                }
                else
                {
                    workMsgs.Add(new PlainMessage(loliconBusiness.getWorkInfo(loliconData, startDateTime, todayLeftCount, template)));
                }

                Task sendGroupTask = session.SendGroupSetuAndRevokeAsync(command.Args, workMsgs, setuFiles, BotConfig.SetuConfig.RevokeInterval, true);

                if (BotConfig.SetuConfig.SendPrivate)
                {
                    await Task.Delay(1000);
                    Task sendTempTask = session.SendTempSetuAsync(command.Args, workMsgs, setuFiles);
                }

                CoolingCache.SetMemberSetuCooling(command.GroupId, command.MemberId);
            }
            catch (ApiException ex)
            {
                string errMsg = $"loliconSearchAsync异常";
                LogHelper.Error(ex, errMsg);
                await session.ReplyGroupMessageWithAtAsync(command.Args, $"获取涩图出错了，{ex.Message}");
                ReportHelper.SendError(ex, errMsg);
            }
            catch (Exception ex)
            {
                string errMsg = "loliconSearchAsync异常";
                LogHelper.Error(ex, errMsg);
                await session.SendTemplateWithAtAsync(command.Args, BotConfig.SetuConfig.ErrorMsg, "获取涩图出错了，再试一次吧~");
                ReportHelper.SendError(ex, errMsg);
            }
            finally
            {
                CoolingCache.SetHandFinish(command.GroupId, command.MemberId);//请求处理完成
            }
        }

        public async Task sendTimingSetuAsync(IMiraiHttpSession session, TimingSetuTimer timingSetuTimer, long groupId)
        {
            int eachPage = 5;
            bool excludeAI = groupId.IsShowAISetu() == false;
            int r18Mode = groupId.IsShowR18Setu() ? 2 : 0;
            int count = timingSetuTimer.Quantity > 20 ? 20 : timingSetuTimer.Quantity;
            string tagStr = RandomHelper.getRandomItem(timingSetuTimer.Tags);
            string[] tagArr = string.IsNullOrWhiteSpace(tagStr) ? null : toLoliconTagArr(tagStr);
            await sendTimingSetuMessage(session, timingSetuTimer, tagStr, groupId);
            await Task.Delay(2000);
            while (count > 0)
            {
                int num = count >= eachPage ? eachPage : count;
                LoliconResultV2 loliconResult = await loliconBusiness.getLoliconResultAsync(r18Mode, excludeAI, num, tagArr);
                count -= num;
                if (loliconResult.data.Count == 0) continue;
                foreach (var setuInfo in loliconResult.data)
                {
                    await sendSetuInfoAsync(session, setuInfo, groupId);
                    await Task.Delay(1000);
                }
            }
        }

        private async Task sendSetuInfoAsync(IMiraiHttpSession session, LoliconDataV2 setuInfo, long groupId)
        {
            try
            {
                bool isR18Img = setuInfo.IsR18;
                bool isShowImg = groupId.IsShowSetuImg(isR18Img);
                DateTime startTime = DateTime.Now;
                List<IChatMessage> workMsgs = new List<IChatMessage>();
                List<FileInfo> setuFiles = isShowImg ? await loliconBusiness.downPixivImgsAsync(setuInfo) : null;
                workMsgs.Add(new PlainMessage(loliconBusiness.getDefaultWorkInfo(setuInfo, startTime)));
                await session.SendGroupSetuAsync(workMsgs, setuFiles, groupId, isShowImg);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "定时涩图发送失败");
                ReportHelper.SendError(ex, "定时涩图发送失败");
            }
        }


    }
}
