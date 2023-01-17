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
using Theresa3rd_Bot.Type;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Handler
{
    public class LolisukiHandler : BaseHandler
    {
        private LolisukiBusiness lolisukiBusiness;

        public LolisukiHandler()
        {
            lolisukiBusiness = new LolisukiBusiness();
        }

        public async Task sendGeneralLolisukiImageAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string message)
        {
            try
            {
                long memberId = args.Sender.Id;
                long groupId = args.Sender.Group.Id;
                DateTime startDateTime = DateTime.Now;
                CoolingCache.SetHanding(groupId, memberId);//请求处理中

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
                string levelStr = getLevelStr(isShowR18);

                if (string.IsNullOrEmpty(tagStr))
                {
                    lolisukiResult = await lolisukiBusiness.getLolisukiResultAsync(r18Mode, levelStr);
                }
                else
                {
                    if (await CheckSetuCustomEnableAsync(session, args) == false) return;
                    string[] tagArr = toLoliconTagArr(tagStr);
                    lolisukiResult = await lolisukiBusiness.getLolisukiResultAsync(r18Mode, levelStr, 1, tagArr);
                }

                if (lolisukiResult == null || lolisukiResult.data.Count == 0)
                {
                    await session.SendTemplateWithAtAsync(args, BotConfig.SetuConfig.NotFoundMsg, " 找不到这类型的图片，换个标签试试吧~");
                    return;
                }

                LolisukiData lolisukiData = lolisukiResult.data.First();
                if (lolisukiData.IsImproper())
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" 该作品含有R18G等内容，不显示相关内容"));
                    return;
                }

                string banTagStr = lolisukiData.hasBanTag();
                if (banTagStr != null)
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage($" 该作品含有被屏蔽的标签【{banTagStr}】，不显示相关内容"));
                    return;
                }

                bool isShowImg = groupId.IsShowSetuImg(lolisukiData.isR18());
                long todayLeftCount = GetSetuLeftToday(groupId, memberId);
                FileInfo fileInfo = isShowImg ? await lolisukiBusiness.downImgAsync(lolisukiData.pid.ToString(), lolisukiData.urls.original, lolisukiData.gif) : null;

                string template = BotConfig.SetuConfig.Lolisuki.Template;
                List<IChatMessage> workMsgs = new List<IChatMessage>();
                if (string.IsNullOrWhiteSpace(template))
                {
                    workMsgs.Add(new PlainMessage(lolisukiBusiness.getDefaultWorkInfo(lolisukiData, fileInfo, startDateTime)));
                }
                else
                {
                    workMsgs.Add(new PlainMessage(lolisukiBusiness.getWorkInfo(lolisukiData, fileInfo, startDateTime, todayLeftCount, template)));
                }

                Task sendGroupTask = session.SendGroupSetuAndRevokeWithAtAsync(args, workMsgs, fileInfo, isShowImg);

                if (BotConfig.SetuConfig.SendPrivate)
                {
                    await Task.Delay(1000);
                    Task sendTempTask = session.SendTempSetuAsync(args, workMsgs, fileInfo, isShowImg);
                }

                CoolingCache.SetMemberSetuCooling(groupId, memberId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "sendGeneralLolisukiImageAsync异常");
                await session.SendTemplateWithAtAsync(args, BotConfig.SetuConfig.ErrorMsg, " 获取图片出错了，再试一次吧~");
            }
            finally
            {
                CoolingCache.SetHandFinish(args.Sender.Group.Id, args.Sender.Id);//请求处理完成
            }
        }

        public async Task sendTimingSetu(IMiraiHttpSession session, TimingSetuTimer timingSetuTimer, long groupId)
        {
            int eachPage = 5;
            bool isShowR18 = groupId.IsShowR18Setu();
            int r18Mode = isShowR18 ? 2 : 0;
            int count = timingSetuTimer.Quantity > 20 ? 20 : timingSetuTimer.Quantity;
            string levelStr = getLevelStr(isShowR18);
            string tagStr = RandomHelper.getRandomItem(timingSetuTimer.Tags);
            string[] tagArr = string.IsNullOrWhiteSpace(tagStr) ? null : toLoliconTagArr(tagStr);
            await sendTimingSetuMessage(session, timingSetuTimer, tagStr, groupId);
            await Task.Delay(2000);
            while (count > 0)
            {
                int num = count >= eachPage ? eachPage : count;
                LolisukiResult lolisukiResult = await lolisukiBusiness.getLolisukiResultAsync(r18Mode, levelStr, num, tagArr);
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
                bool isR18Img = setuInfo.isR18();
                bool isShowImg = groupId.IsShowSetuImg(isR18Img);
                DateTime startTime = DateTime.Now;
                List<IChatMessage> workMsgs = new List<IChatMessage>();
                FileInfo fileInfo = isShowImg ? await lolisukiBusiness.downImgAsync(setuInfo.pid.ToString(), setuInfo.urls.original, setuInfo.isGif()) : null;
                workMsgs.Add(new PlainMessage(lolisukiBusiness.getDefaultWorkInfo(setuInfo, fileInfo, startTime)));
                await session.SendGroupSetuAsync(workMsgs, fileInfo, groupId, isShowImg);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
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
