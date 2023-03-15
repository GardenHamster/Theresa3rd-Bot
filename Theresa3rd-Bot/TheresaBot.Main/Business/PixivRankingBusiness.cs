using TheresaBot.Main.Common;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Mode;
using TheresaBot.Main.Model.Cache;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Pixiv;
using TheresaBot.Main.Model.PixivRanking;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Business
{
    internal class PixivRankingBusiness : SetuBusiness
    {
        private const int eachPage = 50;

        public async Task<PixivRankingInfo> getRankingInfo(PixivRankingItem rankingItem, PixivRankingMode rankingMode, string search_date, int retryTimes = 2)
        {
            if (retryTimes < 0) retryTimes = 0;
            while (retryTimes >= 0)
            {
                try
                {
                    (List<PixivRankingContent> rankingContents, string ranking_date) = await getRankingDatas(rankingMode, search_date);
                    List<PixivRankingDetail> rankingDetails = await filterContents(rankingItem, rankingContents);
                    return new PixivRankingInfo(rankingDetails, rankingItem, rankingMode, ranking_date, BotConfig.PixivRankingConfig.CacheSeconds);
                }
                catch (NoRankingException)
                {
                    throw;
                }
                catch (Exception)
                {
                    if (--retryTimes < 0) throw;
                    await Task.Delay(2000);
                }
            }
            return null;
        }

        public async Task<(List<PixivRankingContent>, string)> getRankingDatas(PixivRankingMode rankingMode, string search_date)
        {
            string mode = rankingMode.Code;
            PixivRankingData firstpage = await PixivHelper.GetPixivRankingData(mode, 1, search_date);
            string ranking_date = firstpage.date;
            int maxScan = BotConfig.PixivRankingConfig.MaxScan;
            if (maxScan > 500) maxScan = 500;
            if (firstpage.rank_total < maxScan) maxScan = firstpage.rank_total;
            int maxPage = MathHelper.getMaxPage(maxScan, eachPage);

            List<PixivRankingContent> rankingContents = new List<PixivRankingContent>();
            for (int page = 1; page < maxPage + 1; page++)
            {
                PixivRankingData rankingData = await PixivHelper.GetPixivRankingData(mode, page, search_date);
                if (rankingData.contents is null || rankingData.contents.Count == 0) throw new ApiException("无法从api中获取任何排行信息");
                rankingContents.AddRange(rankingData.contents);
                await Task.Delay(500);
            }
            return (rankingContents, ranking_date);
        }

        public async Task<List<PixivRankingDetail>> filterContents(PixivRankingItem rankingItem, List<PixivRankingContent> rankingContents)
        {
            List<PixivRankingDetail> rankingDetails = new List<PixivRankingDetail>();
            foreach (var rankingContent in rankingContents)
            {
                try
                {
                    if (checkContentIsOk(rankingItem, rankingContent) == false) continue;
                    PixivWorkInfo pixivWorkInfo = await getRankingWork(rankingContent);
                    await Task.Delay(500);
                    if (pixivWorkInfo is null) continue;
                    if (checkWorkIsOk(rankingItem, pixivWorkInfo) == false) continue;
                    string fullFileName = rankingContent.url.GetPreviewImgSaveName(pixivWorkInfo.illustId);
                    FileInfo previewFile = await PixivHelper.DownPixivImgAsync(rankingContent.illust_id.ToString(), rankingContent.url, fullFileName);
                    PixivRankingDetail rankingDetail = new PixivRankingDetail(rankingContent, pixivWorkInfo, previewFile?.FullName);
                    rankingDetails.Add(rankingDetail);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex);
                }
            }
            return sortDetails(rankingDetails, BotConfig.PixivRankingConfig.SortType);
        }

        private List<PixivRankingDetail> sortDetails(List<PixivRankingDetail> details, PixivRankingSortType sortType)
        {
            if (sortType == PixivRankingSortType.BookMark)
            {
                return details.OrderByDescending(x => x.WorkInfo.bookmarkCount).ToList();
            }
            if (sortType == PixivRankingSortType.BookMarkRate)
            {
                return details.OrderByDescending(x => x.WorkInfo.bookmarkRate).ToList();
            }
            if (sortType == PixivRankingSortType.Ranking)
            {
                return details.OrderByDescending(x => x.WorkInfo.likeCount).ToList();
            }
            if (sortType == PixivRankingSortType.RankingRate)
            {
                return details.OrderByDescending(x => x.WorkInfo.likeRate).ToList();
            }
            return details.ToList();
        }

        private async Task<PixivWorkInfo> getRankingWork(PixivRankingContent content)
        {
            try
            {
                return await PixivHelper.GetPixivWorkInfoAsync(content.illust_id.ToString());
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return null;
            }
            finally
            {
                await Task.Delay(500);
            }
        }

        public string getRankingMsg(string date, string rankingName, string template)
        {
            if (string.IsNullOrWhiteSpace(template)) return getDefaultRankingInfo(date, rankingName);
            template = template.Replace("{Date}", date);
            template = template.Replace("{Ranking}", rankingName);
            template = template.Replace("{CacheSeconds}", BotConfig.PixivRankingConfig.CacheSeconds.ToString());
            return template;
        }

        public string getDefaultRankingInfo(string date, string rankingName)
        {
            return $"{date}{rankingName}精选，数据缓存{BotConfig.PixivRankingConfig.CacheSeconds.ToString()}秒";
        }

        public List<SetuContent> getRankAndPids(PixivRankingInfo pixivRankingInfo, int eachPage)
        {
            int startIndex = 0;
            if (eachPage <= 0) return new();
            List<SetuContent> rankContents = new List<SetuContent>();
            var details = pixivRankingInfo.RankingDetails.OrderBy(o => o.RankingContent.rank).ToList();
            List<string> rankInfos = details.Select(o => $"#{o.RankingContent.rank.ToString().PadRight(3, ' ')} {o.WorkInfo.illustId}").ToList();
            while (startIndex < rankInfos.Count)
            {
                var pageList = rankInfos.Skip(startIndex).Take(eachPage).ToList();
                rankContents.Add(new(String.Join("\r\n", pageList)));
                startIndex += eachPage;
            }
            return rankContents;
        }

        /// <summary>
        /// 检查日榜内容是否合格
        /// </summary>
        /// <param name="rankingItem"></param>
        /// <param name="rankingContent"></param>
        /// <returns></returns>
        private bool checkContentIsOk(PixivRankingItem rankingItem, PixivRankingContent rankingContent)
        {
            if (rankingContent.isImproper()) return false;
            if (rankingContent.hasBanTag()) return false;
            if (rankingContent.isIllust() == false) return false;
            if (rankingContent.isR18())
            {
                if (rankingContent.rating_count < rankingItem.MinRatingCount * BotConfig.PixivRankingConfig.R18Target) return false;
                if (rankingContent.rating_rate < rankingItem.MinRatingRate * BotConfig.PixivRankingConfig.R18Target) return false;
            }
            else
            {
                if (rankingContent.rating_count < rankingItem.MinRatingCount) return false;
                if (rankingContent.rating_rate < rankingItem.MinRatingRate) return false;
            }
            return true;
        }

        /// <summary>
        /// 检查作品内容是否合格
        /// </summary>
        /// <param name="rankingItem"></param>
        /// <param name="rankingContent"></param>
        /// <returns></returns>
        private bool checkWorkIsOk(PixivRankingItem rankingItem, PixivWorkInfo workInfo)
        {
            if (workInfo.IsImproper) return false;
            if (workInfo.hasBanTag() is not null) return false;
            if (workInfo.IsIllust == false) return false;
            bool isRatingCountOk, isRatingRateOk, isBookmarkCountOk, isBookmarkRateOk;

            if (workInfo.IsR18)
            {
                isRatingCountOk = workInfo.likeCount >= rankingItem.MinRatingCount * BotConfig.PixivRankingConfig.R18Target;
                isRatingRateOk = workInfo.likeRate >= rankingItem.MinRatingRate * BotConfig.PixivRankingConfig.R18Target;
                isBookmarkCountOk = workInfo.bookmarkCount >= rankingItem.MinBookCount * BotConfig.PixivRankingConfig.R18Target;
                isBookmarkRateOk = workInfo.bookmarkRate >= rankingItem.MinBookRate * BotConfig.PixivRankingConfig.R18Target;
            }
            else
            {
                isRatingCountOk = workInfo.likeCount >= rankingItem.MinRatingCount;
                isRatingRateOk = workInfo.likeRate >= rankingItem.MinRatingRate;
                isBookmarkCountOk = workInfo.bookmarkCount >= rankingItem.MinBookCount;
                isBookmarkRateOk = workInfo.bookmarkRate >= rankingItem.MinBookRate;
            }
            return (isRatingCountOk && isRatingRateOk) && (isBookmarkCountOk && isBookmarkRateOk);
        }

    }
}
