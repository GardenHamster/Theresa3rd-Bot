using TheresaBot.Main.Business;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Lolisuki;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Handler
{
    public class LolisukiHandler : SetuHandler
    {
        private LolisukiBusiness lolisukiBusiness;

        public LolisukiHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            lolisukiBusiness = new LolisukiBusiness();
        }

        public async Task lolisukiSearchAsync(GroupCommand command)
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

                LolisukiResult lolisukiResult;
                int r18Mode = isShowR18 ? 2 : 0;
                int aiMode = isShowAI ? 2 : 0;
                string levelStr = getLevelStr(isShowR18);

                if (string.IsNullOrEmpty(tagStr))
                {
                    lolisukiResult = await lolisukiBusiness.getLolisukiResultAsync(r18Mode, aiMode, levelStr, 1);
                }
                else
                {
                    if (await CheckSetuCustomEnableAsync(command) == false) return;
                    string[] tagArr = toLoliconTagArr(tagStr);
                    lolisukiResult = await lolisukiBusiness.getLolisukiResultAsync(r18Mode, aiMode, levelStr, 1, tagArr);
                }

                if (lolisukiResult is null || lolisukiResult.data is null || lolisukiResult.data.Count == 0)
                {
                    await command.ReplyGroupTemplateWithAtAsync(BotConfig.SetuConfig.NotFoundMsg, " 找不到这类型的图片，换个标签试试吧~");
                    return;
                }

                LolisukiData lolisukiData = lolisukiResult.data.First();
                if (await CheckSetuSendable(command, lolisukiData, isShowR18, isShowAI) == false) return;

                bool isShowImg = command.GroupId.IsShowSetuImg(lolisukiData.IsR18);
                long todayLeftCount = GetSetuLeftToday(command.GroupId, command.MemberId);
                List<FileInfo> setuFiles = isShowImg ? await downPixivImgsAsync(lolisukiData) : new();

                string template = BotConfig.SetuConfig.Lolisuki.Template;
                List<BaseContent> workMsgs = new List<BaseContent>();
                if (string.IsNullOrWhiteSpace(template))
                {
                    workMsgs.Add(new PlainContent(lolisukiBusiness.getDefaultWorkInfo(lolisukiData, startDateTime)));
                }
                else
                {
                    workMsgs.Add(new PlainContent(lolisukiBusiness.getWorkInfo(lolisukiData, startDateTime, todayLeftCount, template)));
                }

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
                string errMsg = $"lolisukiSearchAsync异常";
                LogHelper.Error(ex, errMsg);
                await command.ReplyGroupMessageWithAtAsync($"获取涩图出错了，{ex.Message}");
                Reporter.SendError(ex, errMsg);
            }
            catch (Exception ex)
            {
                string errMsg = "lolisukiSearchAsync异常";
                LogHelper.Error(ex, errMsg);
                await command.ReplyGroupTemplateWithAtAsync(BotConfig.SetuConfig.ErrorMsg, "获取涩图出错了，再试一次吧~");
                Reporter.SendError(ex, errMsg);
            }
            finally
            {
                CoolingCache.SetHandFinish(command.GroupId, command.MemberId);//请求处理完成
            }
        }

        public async Task sendTimingSetuAsync(TimingSetuTimer timingSetuTimer, long groupId)
        {
            int eachPage = 5;
            bool isShowAI = groupId.IsShowAISetu();
            bool isShowR18 = groupId.IsShowR18Setu();
            int r18Mode = isShowR18 ? 2 : 0;
            int aiMode = isShowAI ? 2 : 0;
            int count = timingSetuTimer.Quantity > 20 ? 20 : timingSetuTimer.Quantity;
            string levelStr = getLevelStr(isShowR18);
            string tagStr = RandomHelper.getRandomItem(timingSetuTimer.Tags);
            string[] tagArr = string.IsNullOrWhiteSpace(tagStr) ? new string[0] : toLoliconTagArr(tagStr);
            await sendTimingSetuMessage(timingSetuTimer, tagStr, groupId);
            await Task.Delay(2000);
            while (count > 0)
            {
                int num = count >= eachPage ? eachPage : count;
                LolisukiResult lolisukiResult = await lolisukiBusiness.getLolisukiResultAsync(r18Mode, aiMode, levelStr, num, tagArr);
                count -= num;
                if (lolisukiResult.data.Count == 0) continue;
                foreach (var setuInfo in lolisukiResult.data)
                {
                    await sendSetuInfoAsync(setuInfo, groupId);
                    await Task.Delay(1000);
                }
            }
        }

        private async Task sendSetuInfoAsync(LolisukiData setuInfo, long groupId)
        {
            try
            {
                bool isR18Img = setuInfo.IsR18;
                bool isShowImg = groupId.IsShowSetuImg(isR18Img);
                DateTime startTime = DateTime.Now;
                List<BaseContent> workMsgs = new List<BaseContent>();
                List<FileInfo> setuFiles = isShowImg ? await downPixivImgsAsync(setuInfo) : new();
                workMsgs.Add(new PlainContent(lolisukiBusiness.getDefaultWorkInfo(setuInfo, startTime)));
                await Session.SendGroupSetuAsync(workMsgs, setuFiles, groupId, isShowImg);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "定时涩图发送失败");
                Reporter.SendError(ex, "定时涩图发送失败");
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
