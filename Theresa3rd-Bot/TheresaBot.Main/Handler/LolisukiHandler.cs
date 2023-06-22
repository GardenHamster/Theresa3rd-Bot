using TheresaBot.Main.Business;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Lolisuki;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Handler
{
    internal class LolisukiHandler : SetuHandler
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

                List<LolisukiData> dataList;
                int r18Mode = isShowR18 ? 2 : 0;
                int aiMode = isShowAI ? 2 : 0;
                string levelStr = getLevelStr(isShowR18, BotConfig.SetuConfig?.Lolisuki?.Level);

                if (string.IsNullOrEmpty(tagStr))
                {
                    dataList = await lolisukiBusiness.getLolisukiDataListAsync(r18Mode, aiMode, levelStr, 1);
                }
                else
                {
                    if (await CheckSetuCustomEnableAsync(command) == false) return;
                    dataList = await lolisukiBusiness.getLolisukiDataListAsync(r18Mode, aiMode, levelStr, 1, toLoliconTagArr(tagStr));
                }

                if (dataList.Count == 0)
                {
                    await command.ReplyGroupTemplateWithAtAsync(BotConfig.SetuConfig.NotFoundMsg, "找不到这类型的图片，换个标签试试吧~");
                    return;
                }

                LolisukiData lolisukiData = dataList.First();
                if (await CheckSetuSendable(command, lolisukiData, isShowR18) == false) return;

                long todayLeftCount = GetSetuLeftToday(command.GroupId, command.MemberId);
                List<FileInfo> setuFiles = await GetSetuFilesAsync(lolisukiData, command.GroupId);

                string template = BotConfig.SetuConfig.Lolisuki.Template;
                List<BaseContent> workMsgs = new List<BaseContent>();
                workMsgs.Add(new PlainContent(lolisukiBusiness.getWorkInfo(lolisukiData, startDateTime, todayLeftCount, template)));

                PixivSetuContent setuContent = new PixivSetuContent(workMsgs, setuFiles, lolisukiData);
                int[] msgIds = await command.ReplyGroupSetuAsync(setuContent, BotConfig.SetuConfig.RevokeInterval, BotConfig.PixivConfig.SendImgBehind, true);
                Task recordTask = recordBusiness.AddPixivRecord(setuContent, msgIds, command.GroupId);
                if (BotConfig.SetuConfig.SendPrivate)
                {
                    await Task.Delay(1000);
                    Task sendTempTask = command.SendTempSetuAsync(setuContent, BotConfig.PixivConfig.SendImgBehind);
                }

                CoolingCache.SetMemberSetuCooling(command.GroupId, command.MemberId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "lolisukiSearchAsync异常");
                await command.ReplyError(ex);
                await Task.Delay(1000);
                Reporter.SendError(ex, "lolisukiSearchAsync异常");
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
                bool isShowAI = groupId.IsShowAISetu();
                bool isShowR18 = groupId.IsShowR18Setu();
                string levelStr = getLevelStr(isShowR18, BotConfig.TimingSetuConfig?.LolisukiLevel);
                bool sendMerge = timingSetuTimer.SendMerge;
                int aiMode = isShowAI ? 2 : 0;
                int r18Mode = isShowR18 ? 2 : 0;
                string tagStr = RandomHelper.getRandomItem(timingSetuTimer.Tags);
                string[] tagArr = string.IsNullOrWhiteSpace(tagStr) ? new string[0] : toLoliconTagArr(tagStr);
                int quantity = timingSetuTimer.Quantity > 20 ? 20 : timingSetuTimer.Quantity;
                List<LolisukiData> dataList = await lolisukiBusiness.getLolisukiDataListAsync(r18Mode, aiMode, levelStr, quantity, tagArr);
                List<SetuContent> setuContents = await getSetuContent(dataList, groupId);
                await sendTimingSetuMessageAsync(timingSetuTimer, tagStr, groupId);
                await Task.Delay(2000);
                await SendGroupSetuAsync(setuContents, groupId, sendMerge, margeEachPage);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "定时涩图异常");
                Reporter.SendError(ex, "定时涩图异常");
            }
        }

        private async Task<List<SetuContent>> getSetuContent(List<LolisukiData> datas, long groupId)
        {
            List<SetuContent> setuContents = new List<SetuContent>();
            foreach (var data in datas) setuContents.Add(await getSetuContent(data, groupId));
            return setuContents;
        }

        private async Task<SetuContent> getSetuContent(LolisukiData data, long groupId)
        {
            string setuInfo = lolisukiBusiness.getDefaultWorkInfo(data);
            List<FileInfo> setuFiles = await GetSetuFilesAsync(data, groupId);
            return new SetuContent(setuInfo, setuFiles);
        }

        private string getLevelStr(bool isShowR18, string settingLevel)
        {
            if (string.IsNullOrWhiteSpace(settingLevel)) return $"{(int)LolisukiLevel.Level0}-{(int)LolisukiLevel.Level2}";
            string[] levelArr = settingLevel.Split('-', StringSplitOptions.RemoveEmptyEntries);
            string minLevelStr = levelArr[0].Trim();
            string maxLevelStr = levelArr.Length > 1 ? levelArr[1].Trim() : levelArr[0].Trim();
            int minLevelTemp = int.Parse(minLevelStr);
            int maxLevelTemp = int.Parse(maxLevelStr);
            int minLevel = Math.Min(minLevelTemp, maxLevelTemp);
            int maxLevel = Math.Max(minLevelTemp, maxLevelTemp);
            if (minLevel < (int)LolisukiLevel.Level0) minLevel = (int)LolisukiLevel.Level0;
            if (maxLevel > (int)LolisukiLevel.Level6) maxLevel = (int)LolisukiLevel.Level6;
            if (maxLevel > (int)LolisukiLevel.Level4 && isShowR18 == false) maxLevel = (int)LolisukiLevel.Level4;
            return minLevel == maxLevel ? $"{minLevel}" : $"{minLevel}-{maxLevel}";
        }

    }
}
