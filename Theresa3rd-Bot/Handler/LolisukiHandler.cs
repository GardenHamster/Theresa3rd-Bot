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
using Theresa3rd_Bot.Model.Config;
using Theresa3rd_Bot.Model.Lolisuki;
using Theresa3rd_Bot.Model.Pixiv;
using Theresa3rd_Bot.Type;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Handler
{
    public class LolisukiHandler : SetuHandler
    {
        private LolisukiBusiness lolisukiBusiness;

        public LolisukiHandler()
        {
            lolisukiBusiness = new LolisukiBusiness();
        }

        public async Task lolisukiSearchAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string message)
        {
            try
            {
                long memberId = args.Sender.Id;
                long groupId = args.Sender.Group.Id;
                DateTime startDateTime = DateTime.Now;
                CoolingCache.SetHanding(groupId, memberId);//请求处理中

                bool isShowAI = groupId.IsShowAISetu();
                bool isShowR18 = groupId.IsShowR18Setu();
                string tagStr = message.splitKeyWord(BotConfig.SetuConfig.Lolisuki.Command) ?? "";
                if (await CheckSetuTagEnableAsync(session, args, tagStr) == false) return;
                if (string.IsNullOrWhiteSpace(BotConfig.SetuConfig.ProcessingMsg) == false)
                {
                    await session.SendTemplateWithAtAsync(args, BotConfig.SetuConfig.ProcessingMsg, null);
                    await Task.Delay(1000);
                }

                LolisukiResult lolisukiResult = null;
                int r18Mode = isShowR18 ? 2 : 0;
                int aiMode = isShowAI ? 2 : 0;
                string levelStr = getLevelStr(isShowR18);

                if (string.IsNullOrEmpty(tagStr))
                {
                    lolisukiResult = await lolisukiBusiness.getLolisukiResultAsync(r18Mode, aiMode, levelStr);
                }
                else
                {
                    if (await CheckSetuCustomEnableAsync(session, args) == false) return;
                    string[] tagArr = toLoliconTagArr(tagStr);
                    lolisukiResult = await lolisukiBusiness.getLolisukiResultAsync(r18Mode, aiMode, levelStr, 1, tagArr);
                }

                if (lolisukiResult == null || lolisukiResult.data == null || lolisukiResult.data.Count == 0)
                {
                    await session.SendTemplateWithAtAsync(args, BotConfig.SetuConfig.NotFoundMsg, " 找不到这类型的图片，换个标签试试吧~");
                    return;
                }

                LolisukiData lolisukiData = lolisukiResult.data.First();
                if (await CheckSetuSendable(session, args, lolisukiData, isShowR18, isShowAI) == false) return;

                bool isShowImg = groupId.IsShowSetuImg(lolisukiData.IsR18);
                long todayLeftCount = GetSetuLeftToday(groupId, memberId);
                List<FileInfo> setuFiles = isShowImg ? await lolisukiBusiness.downPixivImgsAsync(lolisukiData) : null;

                string template = BotConfig.SetuConfig.Lolisuki.Template;
                List<IChatMessage> workMsgs = new List<IChatMessage>();
                if (string.IsNullOrWhiteSpace(template))
                {
                    workMsgs.Add(new PlainMessage(lolisukiBusiness.getDefaultWorkInfo(lolisukiData, startDateTime)));
                }
                else
                {
                    workMsgs.Add(new PlainMessage(lolisukiBusiness.getWorkInfo(lolisukiData, startDateTime, todayLeftCount, template)));
                }

                Task sendGroupTask = session.SendGroupSetuAndRevokeAsync(args, workMsgs, setuFiles, BotConfig.SetuConfig.RevokeInterval, true);

                if (BotConfig.SetuConfig.SendPrivate)
                {
                    await Task.Delay(1000);
                    Task sendTempTask = session.SendTempSetuAsync(args, workMsgs, setuFiles);
                }

                CoolingCache.SetMemberSetuCooling(groupId, memberId);
            }
            catch (Exception ex)
            {
                string errMsg = "lolisukiSearchAsync异常";
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
            bool isShowAI = groupId.IsShowAISetu();
            bool isShowR18 = groupId.IsShowR18Setu();
            int r18Mode = isShowR18 ? 2 : 0;
            int aiMode = isShowAI ? 2 : 0;
            int count = timingSetuTimer.Quantity > 20 ? 20 : timingSetuTimer.Quantity;
            string levelStr = getLevelStr(isShowR18);
            string tagStr = RandomHelper.getRandomItem(timingSetuTimer.Tags);
            string[] tagArr = string.IsNullOrWhiteSpace(tagStr) ? null : toLoliconTagArr(tagStr);
            await sendTimingSetuMessage(session, timingSetuTimer, tagStr, groupId);
            await Task.Delay(2000);
            while (count > 0)
            {
                int num = count >= eachPage ? eachPage : count;
                LolisukiResult lolisukiResult = await lolisukiBusiness.getLolisukiResultAsync(r18Mode, aiMode, levelStr, num, tagArr);
                count -= num;
                if (lolisukiResult.data.Count == 0) continue;
                foreach (var setuInfo in lolisukiResult.data)
                {
                    await sendSetuInfoAsync(session, setuInfo, groupId);
                    await Task.Delay(1000);
                }
            }
        }

        private async Task sendSetuInfoAsync(IMiraiHttpSession session, LolisukiData setuInfo, long groupId)
        {
            try
            {
                bool isR18Img = setuInfo.IsR18;
                bool isShowImg = groupId.IsShowSetuImg(isR18Img);
                DateTime startTime = DateTime.Now;
                List<IChatMessage> workMsgs = new List<IChatMessage>();
                List<FileInfo> setuFiles = isShowImg ? await lolisukiBusiness.downPixivImgsAsync(setuInfo) : null;
                workMsgs.Add(new PlainMessage(lolisukiBusiness.getDefaultWorkInfo(setuInfo, startTime)));
                await session.SendGroupSetuAsync(workMsgs, setuFiles, groupId, isShowImg);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "定时涩图发送失败");
                ReportHelper.SendError(ex, "定时涩图发送失败");
            }
        }


        private string getLevelStr(bool isShowR18)
        {
            try
            {
                string levelStr = BotConfig.SetuConfig.Lolisuki.Level;
                if (string.IsNullOrWhiteSpace(levelStr)) return $"{(int)LolisukiLevel.Level0}-{(int)LolisukiLevel.Level3}";

                string[] levelArr = levelStr.Split('-', StringSplitOptions.RemoveEmptyEntries);
                string minLevelStr = levelArr[0].Trim();
                string maxLevelStr = levelArr.Length > 1 ? levelArr[1].Trim() : levelArr[0].Trim();
                int minLevel = int.Parse(minLevelStr);
                int maxLevel = int.Parse(maxLevelStr);
                if (minLevel < (int)LolisukiLevel.Level0) minLevel = (int)LolisukiLevel.Level0;
                if (maxLevel > (int)LolisukiLevel.Level6) maxLevel = (int)LolisukiLevel.Level6;
                if (maxLevel > (int)LolisukiLevel.Level4 && isShowR18 == false) maxLevel = (int)LolisukiLevel.Level4;
                return minLevel == maxLevel ? $"{minLevel}" : $"{minLevel}-{maxLevel}";
            }
            catch (Exception)
            {
                return $"{(int)LolisukiLevel.Level0}-{(int)(isShowR18 ? LolisukiLevel.Level6 : LolisukiLevel.Level3)}";
            }
        }



    }
}
