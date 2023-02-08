using TheresaBot.Main.Business;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Mode;
using TheresaBot.Main.Model.Cache;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Pixiv;
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
            try
            {
                string date = command.KeyWord;
                PixivRankingMode rankingMode = PixivRankingMode.Daily;
                PixivRankingType rankingType = rankingMode.Type;
                PixivRankingItem rankingItem = BotConfig.PixivRankingConfig.Daily;

                CoolingCache.SetHanding(command.GroupId, command.MemberId);
                CoolingCache.SetPixivRankingHanding(command.GroupId);
                if (string.IsNullOrWhiteSpace(BotConfig.PixivRankingConfig.ProcessingMsg) == false)
                {
                    await command.ReplyGroupTemplateWithAtAsync(BotConfig.PixivRankingConfig.ProcessingMsg);
                }

                PixivRankingInfo rankingInfo = PixivRankingCache.GetCache(rankingMode, date);
                if (rankingInfo == null)
                {
                    rankingInfo = await getRankingInfos(rankingItem, rankingMode);
                    PixivRankingCache.AddCache(rankingMode, rankingInfo, date);
                }

                await sendRankingPreview(command, rankingInfo, rankingMode);
                CoolingCache.SetGroupPixivRankingCooling(rankingMode.Type, command.GroupId);
            }
            catch (Exception ex)
            {
                string errMsg = $"sendDailyRanking异常";
                LogHelper.Error(ex, errMsg);
                await command.ReplyGroupTemplateWithAtAsync(BotConfig.GeneralConfig.ErrorMsg, "出了点小问题，再试一次吧~");
                Reporter.SendError(ex, errMsg);
            }
            finally
            {
                CoolingCache.SetHandFinish(command.GroupId, command.MemberId);
                CoolingCache.SetPixivRankingHandFinish(command.GroupId);
            }
        }

        public Task sendDailyAIRanking(GroupCommand command)
        {
            return Task.CompletedTask;
        }

        public Task sendDailyMaleRanking(GroupCommand command)
        {
            return Task.CompletedTask;
        }

        public Task sendWeeklyRanking(GroupCommand command)
        {
            return Task.CompletedTask;
        }

        public Task sendMonthlyRanking(GroupCommand command)
        {
            return Task.CompletedTask;
        }

        private async Task<PixivRankingInfo> getRankingInfos(PixivRankingItem rankingItem, PixivRankingMode rankingMode)
        {
            (List<PixivRankingContent> rankingContents, string date) = await rankingBusiness.getRankingDatas(rankingItem, rankingMode);
            List<PixivRankingDetail> rankingDetails = await rankingBusiness.filterContents(rankingItem, rankingContents);
            return new PixivRankingInfo(rankingDetails, rankingItem, rankingMode, date, BotConfig.PixivRankingConfig.CacheSeconds);
        }

        private async Task sendRankingPreview(GroupCommand command, PixivRankingInfo pixivRankingInfo, PixivRankingMode rankingMode)
        {
            string template = BotConfig.PixivRankingConfig.Template;
            string rankingInfo = rankingBusiness.getRankingInfo(pixivRankingInfo.Date, rankingMode.Name, template);

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
                string fileName = $"{rankingMode.Code}_preview_{pixivRankingInfo.Date}_{startIndex}_{startIndex + maxInPage}.jpg";
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
