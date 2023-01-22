using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Theresa3rd_Bot.Business;
using Theresa3rd_Bot.Cache;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Exceptions;
using Theresa3rd_Bot.Model.Config;
using Theresa3rd_Bot.Model.Lolicon;
using Theresa3rd_Bot.Model.Pixiv;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Handler
{
    public class LoliconHandler : SetuHandler
    {
        private LoliconBusiness loliconBusiness;

        public LoliconHandler()
        {
            loliconBusiness = new LoliconBusiness();
        }

        public async Task loliconSearchAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string message)
        {
            try
            {
                long memberId = args.Sender.Id;
                long groupId = args.Sender.Group.Id;
                DateTime startDateTime = DateTime.Now;
                CoolingCache.SetHanding(groupId, memberId);//请求处理中

                bool isShowAI = groupId.IsShowAISetu();
                bool isShowR18 = groupId.IsShowR18Setu();
                string tagStr = message.splitKeyWord(BotConfig.SetuConfig.Lolicon.Command) ?? "";
                if (await CheckSetuTagEnableAsync(session, args, tagStr) == false) return;
                if (string.IsNullOrWhiteSpace(BotConfig.SetuConfig.ProcessingMsg) == false)
                {
                    await session.SendTemplateWithAtAsync(args, BotConfig.SetuConfig.ProcessingMsg, null);
                    await Task.Delay(1000);
                }

                LoliconResultV2 loliconResult = null;
                int r18Mode = isShowR18 ? 2 : 0;
                bool excludeAI = isShowAI == false;

                if (string.IsNullOrEmpty(tagStr))
                {
                    loliconResult = await loliconBusiness.getLoliconResultAsync(r18Mode, excludeAI);
                }
                else
                {
                    if (await CheckSetuCustomEnableAsync(session, args) == false) return;
                    string[] tagArr = toLoliconTagArr(tagStr);
                    loliconResult = await loliconBusiness.getLoliconResultAsync(r18Mode, excludeAI, 1, tagArr);
                }

                if (loliconResult == null || loliconResult.data == null || loliconResult.data.Count == 0)
                {
                    await session.SendTemplateWithAtAsync(args, BotConfig.SetuConfig.NotFoundMsg, " 找不到这类型的图片，换个标签试试吧~");
                    return;
                }

                LoliconDataV2 loliconData = loliconResult.data.First();
                if (await CheckSetuSendable(session, args, loliconData, isShowR18, isShowAI) == false) return;

                long todayLeftCount = GetSetuLeftToday(groupId, memberId);
                bool isShowImg = groupId.IsShowSetuImg(loliconData.IsR18);
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

                Task sendGroupTask = session.SendGroupSetuAndRevokeAsync(args, workMsgs, setuFiles, BotConfig.SetuConfig.RevokeInterval, true);

                if (BotConfig.SetuConfig.SendPrivate)
                {
                    await Task.Delay(1000);
                    Task sendTempTask = session.SendTempSetuAsync(args, workMsgs, setuFiles);
                }

                CoolingCache.SetMemberSetuCooling(groupId, memberId);
            }
            catch (ApiException ex)
            {
                string errMsg = $"loliconSearchAsync异常";
                LogHelper.Error(ex, errMsg);
                await session.SendGroupMessageWithAtAsync(args, $"获取涩图出错了，{ex.Message}");
                ReportHelper.SendError(ex, errMsg);
            }
            catch (Exception ex)
            {
                string errMsg = "loliconSearchAsync异常";
                LogHelper.Error(ex, errMsg);
                await session.SendTemplateWithAtAsync(args, BotConfig.SetuConfig.ErrorMsg, "获取涩图出错了，再试一次吧~");
                ReportHelper.SendError(ex, errMsg);
            }
            finally
            {
                CoolingCache.SetHandFinish(args.Sender.Group.Id, args.Sender.Id);//请求处理完成
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
