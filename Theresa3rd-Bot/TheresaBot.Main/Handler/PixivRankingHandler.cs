using TheresaBot.Main.Business;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Mode;
using TheresaBot.Main.Model.Cache;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.PixivRanking;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Handler
{
    public class PixivRankingHandler : SetuHandler
    {
        private PixivBusiness pixivBusiness;
        private PixivRankingBusiness rankingBusiness;

        public PixivRankingHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            pixivBusiness = new PixivBusiness();
            rankingBusiness = new PixivRankingBusiness();
        }
        public async Task sendDailyRanking(GroupCommand command)
        {
            PixivRankingMode rankingMode = PixivRankingMode.Daily;
            PixivRankingItem rankingItem = BotConfig.PixivRankingConfig.Daily;
            await sendRanking(command, rankingMode, rankingItem);
        }

        public async Task sendDailyAIRanking(GroupCommand command)
        {
            PixivRankingMode rankingMode = PixivRankingMode.DailyAI;
            PixivRankingItem rankingItem = BotConfig.PixivRankingConfig.DailyAI;
            await sendRanking(command, rankingMode, rankingItem);
        }

        public async Task sendMaleRanking(GroupCommand command)
        {
            PixivRankingMode rankingMode = PixivRankingMode.Male;
            PixivRankingItem rankingItem = BotConfig.PixivRankingConfig.Male;
            await sendRanking(command, rankingMode, rankingItem);
        }

        public async Task sendWeeklyRanking(GroupCommand command)
        {
            PixivRankingMode rankingMode = PixivRankingMode.Weekly;
            PixivRankingItem rankingItem = BotConfig.PixivRankingConfig.Weekly;
            await sendRanking(command, rankingMode, rankingItem);
        }

        public async Task sendMonthlyRanking(GroupCommand command)
        {
            PixivRankingMode rankingMode = PixivRankingMode.Monthly;
            PixivRankingItem rankingItem = BotConfig.PixivRankingConfig.Monthly;
            await sendRanking(command, rankingMode, rankingItem);
        }

        private async Task sendRanking(GroupCommand command, PixivRankingMode rankingMode, PixivRankingItem rankingItem)
        {
            try
            {
                string search_date = command.KeyWord;
                CoolingCache.SetHanding(command.GroupId, command.MemberId);
                CoolingCache.SetPixivRankingHanding(command.GroupId);
                if (string.IsNullOrWhiteSpace(BotConfig.PixivRankingConfig.ProcessingMsg) == false)
                {
                    await command.ReplyGroupTemplateWithAtAsync(BotConfig.PixivRankingConfig.ProcessingMsg);
                }

                PixivRankingInfo rankingInfo = PixivRankingCache.GetCache(rankingMode, search_date);
                if (rankingInfo == null)
                {
                    rankingInfo = await getRankingInfos(rankingItem, rankingMode, search_date);
                    PixivRankingCache.AddCache(rankingMode, rankingInfo, search_date);
                }

                await sendRankingPreview(command, rankingInfo, rankingMode);
                CoolingCache.SetGroupPixivRankingCooling(rankingMode.Type, command.GroupId);
            }
            catch (Exception ex)
            {
                string errMsg = $"sendRanking异常";
                LogHelper.Error(ex, errMsg);
                await command.ReplyError(ex);
                await Task.Delay(1000);
                Reporter.SendError(ex, errMsg);
            }
            finally
            {
                CoolingCache.SetHandFinish(command.GroupId, command.MemberId);
                CoolingCache.SetPixivRankingHandFinish(command.GroupId);
            }
        }

        private async Task<PixivRankingInfo> getRankingInfos(PixivRankingItem rankingItem, PixivRankingMode rankingMode, string search_date)
        {
            (List<PixivRankingContent> rankingContents, string ranking_date) = await rankingBusiness.getRankingDatas(rankingMode, search_date);
            List<PixivRankingDetail> rankingDetails = await rankingBusiness.filterContents(rankingItem, rankingContents);
            return new PixivRankingInfo(rankingDetails, rankingItem, rankingMode, ranking_date, BotConfig.PixivRankingConfig.CacheSeconds);
        }

        private async Task sendRankingPreview(GroupCommand command, PixivRankingInfo pixivRankingInfo, PixivRankingMode rankingMode)
        {
            string template = BotConfig.PixivRankingConfig.Template;
            string rankingInfo = rankingBusiness.getRankingInfo(pixivRankingInfo.RankingDate, rankingMode.Name, template);

            List<string> PreviewFilePaths = pixivRankingInfo.PreviewFilePaths;
            if (PreviewFilePaths is null || PreviewFilePaths.IsFilesExists() == false)
            {
                PreviewFilePaths = createPreviewImg(pixivRankingInfo);
                pixivRankingInfo.PreviewFilePaths = PreviewFilePaths;
            }

            List<SetuContent> setuContents = new List<SetuContent>();
            setuContents.AddRange(PreviewFilePaths.Select(o => new SetuContent(new FileInfo(o))));

            await command.ReplyGroupMessageWithAtAsync(rankingInfo);
            await Task.Delay(1000);
            await Session.SendGroupSetuAsync(setuContents, command.GroupId, BotConfig.PixivRankingConfig.SendMerge);
            await Task.Delay(1000);
        }

        private List<string> createPreviewImg(PixivRankingInfo pixivRankingInfo)
        {
            int startIndex = 0;
            int maxInPage = BotConfig.PixivRankingConfig.MaxInPage;
            if (maxInPage <= 0) maxInPage = 30;
            List<string> fileInfos = new List<string>();
            List<PixivRankingDetail> details = pixivRankingInfo.RankingDetails;
            if (details.Count == 0) return fileInfos;
            PixivRankingMode rankingMode = pixivRankingInfo.RankingMode;
            while (startIndex < details.Count)
            {
                string fileName = $"{rankingMode.Code}_preview_{pixivRankingInfo.RankingDate}_{startIndex}_{startIndex + maxInPage}.jpg";
                string savePath = Path.Combine(FilePath.GetDownFileSavePath(), fileName);
                var partList = details.Skip(startIndex).Take(maxInPage).ToList();
                var previewFile = createPreviewImg(partList, savePath);
                if (previewFile is not null) fileInfos.Add(previewFile.FullName);
                startIndex += maxInPage;
            }
            return fileInfos;
        }

        private FileInfo createPreviewImg(List<PixivRankingDetail> datas, string savePath)
        {
            try
            {
                return PixivRankingDrawHelper.DrawPreview(datas, savePath);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "日榜一览图合成失败");
                Reporter.SendError(ex, "日榜一览图合成失败");
                return null;
            }
        }


    }
}
