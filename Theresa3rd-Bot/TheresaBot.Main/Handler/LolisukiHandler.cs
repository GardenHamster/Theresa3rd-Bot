using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Lolisuki;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Services;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Handler
{
    internal class LolisukiHandler : SetuHandler
    {
        private LolisukiService lolisukiService;

        public LolisukiHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            lolisukiService = new LolisukiService();
        }

        public async Task LolisukiSearchAsync(GroupCommand command)
        {
            try
            {
                List<LolisukiData> dataList;
                string tagStr = command.KeyWord;
                bool isShowAI = command.GroupId.IsShowAISetu();
                bool isShowR18 = command.GroupId.IsShowR18();
                int r18Mode = isShowR18 ? 2 : 0;
                int aiMode = isShowAI ? 2 : 0;

                CoolingCache.SetHanding(command.GroupId, command.MemberId);//请求处理中
                if (await CheckSetuTagEnableAsync(command, tagStr) == false) return;
                string levelStr = GetLevelStr(isShowR18, BotConfig.SetuConfig?.Lolisuki?.Level);
                await command.ReplyProcessingMessageAsync(BotConfig.SetuConfig.ProcessingMsg);

                if (string.IsNullOrEmpty(tagStr))
                {
                    dataList = await lolisukiService.getLolisukiDataListAsync(r18Mode, aiMode, levelStr, 1);
                }
                else
                {
                    if (await CheckSetuCustomEnableAsync(command) == false) return;
                    dataList = await lolisukiService.getLolisukiDataListAsync(r18Mode, aiMode, levelStr, 1, ToLoliconTagArr(tagStr.ToActualPixivTags()));
                }

                if (dataList.Count == 0)
                {
                    await command.ReplyGroupTemplateWithQuoteAsync(BotConfig.SetuConfig.NotFoundMsg, "找不到这类型的图片，换个标签试试吧~");
                    return;
                }

                LolisukiData lolisukiData = dataList.First();
                if (await CheckSetuSendable(command, lolisukiData, isShowR18) == false) return;

                long todayLeftCount = GetSetuLeftToday(command.GroupId, command.MemberId);
                List<FileInfo> setuFiles = await GetSetuFilesAsync(lolisukiData, command.GroupId);

                string template = BotConfig.SetuConfig.Lolisuki.Template;
                List<BaseContent> workMsgs = new List<BaseContent>();
                workMsgs.Add(new PlainContent(lolisukiService.getWorkInfo(lolisukiData, todayLeftCount, template)));

                PixivSetuContent setuContent = new PixivSetuContent(workMsgs, setuFiles, lolisukiData);
                var results = await command.ReplyGroupSetuAsync(setuContent, BotConfig.SetuConfig.RevokeInterval, BotConfig.PixivConfig.SendImgBehind);
                var msgIds = results.Select(o => o.MessageId).ToArray();
                var recordTask = recordService.AddPixivRecord(setuContent, Session.PlatformType, msgIds, command.GroupId);
                if (BotConfig.SetuConfig.SendPrivate)
                {
                    await Task.Delay(1000);
                    Task sendTempTask = command.SendTempSetuAsync(setuContent, BotConfig.PixivConfig.SendImgBehind);
                }

                CoolingCache.SetMemberSetuCooling(command.GroupId, command.MemberId);
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "Lolisuki涩图功能异常");
            }
            finally
            {
                CoolingCache.SetHandFinish(command.GroupId, command.MemberId);//请求处理完成
            }
        }


        public async Task SendTimingSetuAsync(TimingSetuTimer timingSetuTimer, long groupId)
        {
            try
            {
                int margeEachPage = 5;
                bool isShowAI = groupId.IsShowAISetu();
                bool isShowR18 = groupId.IsShowR18();
                string levelStr = GetLevelStr(isShowR18, BotConfig.TimingSetuConfig?.LolisukiLevel);
                bool sendMerge = timingSetuTimer.SendMerge;
                int aiMode = isShowAI ? 2 : 0;
                int r18Mode = isShowR18 ? 2 : 0;
                string tagStr = RandomHelper.RandomItem(timingSetuTimer.Tags);
                string[] tagArr = string.IsNullOrWhiteSpace(tagStr) ? new string[0] : ToLoliconTagArr(tagStr);
                int quantity = timingSetuTimer.Quantity > 20 ? 20 : timingSetuTimer.Quantity;
                List<LolisukiData> dataList = await lolisukiService.getLolisukiDataListAsync(r18Mode, aiMode, levelStr, quantity, tagArr);
                List<SetuContent> setuContents = await GetSetuContent(dataList, groupId);
                await sendTimingSetuMessageAsync(timingSetuTimer, tagStr, groupId);
                await Task.Delay(2000);
                await SendGroupSetuAsync(setuContents, groupId, sendMerge, margeEachPage);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "定时涩图异常");
                await Reporter.SendError(ex, "定时涩图异常");
            }
        }

        private async Task<List<SetuContent>> GetSetuContent(List<LolisukiData> datas, long groupId)
        {
            List<SetuContent> setuContents = new List<SetuContent>();
            foreach (var data in datas) setuContents.Add(await GetSetuContent(data, groupId));
            return setuContents;
        }

        private async Task<SetuContent> GetSetuContent(LolisukiData data, long groupId)
        {
            string setuInfo = lolisukiService.getDefaultWorkInfo(data);
            List<FileInfo> setuFiles = await GetSetuFilesAsync(data, groupId);
            return new SetuContent(setuInfo, setuFiles);
        }

        private string GetLevelStr(bool isShowR18, string settingLevel)
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
