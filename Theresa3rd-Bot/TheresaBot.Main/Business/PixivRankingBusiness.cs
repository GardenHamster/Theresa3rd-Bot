using TheresaBot.Main.Common;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Model.Pixiv;
using TheresaBot.Main.Model.PixivRanking;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Business
{
    public class PixivRankingBusiness : SetuBusiness
    {
        private const int eachPage = 50;

        public async Task<(List<PixivRankingContent>, string)> getRankingDatas(PixivRankingItem rankingItem, string mode)
        {
            string date = string.Empty;
            int maxScan = BotConfig.PixivRankingConfig.MaxScan;
            int maxPage = MathHelper.getMaxPage(maxScan, eachPage);
            List<PixivRankingContent> rankingContents = new List<PixivRankingContent>();
            for (int page = 1; page < maxPage + 1; page++)
            {
                PixivRankingData rankingData = await PixivHelper.GetPixivRankingData(mode, page);
                if (page == 1) date = rankingData.date;
                if (rankingData.contents is null || rankingData.contents.Count == 0) throw new ApiException("无法从api中获取任何排行信息");
                rankingContents.AddRange(rankingData.contents);
            }
            List<PixivRankingContent> filterContents = new List<PixivRankingContent>();
            for (int i = 0; i < rankingContents.Count && i < maxScan; i++)
            {
                if (checkContentIsOk(rankingItem,rankingContents[i]) == false) continue;
                filterContents.Add(rankingContents[i]);
            }

            List<PixivRankingContent> sortList = sortContentList(filterContents, BotConfig.PixivRankingConfig.SortType);
            return (sortList, date);
        }

        public List<PixivRankingContent> sortContentList(List<PixivRankingContent> contents, PixivRankingSortType sortType)
        {
            if (sortType == PixivRankingSortType.Ranking)
            {
                return contents.OrderByDescending(x => x.rating_count).ToList();
            }
            if (sortType == PixivRankingSortType.RankingRate)
            {
                return contents.OrderByDescending(x => x.rating_rate).ToList();
            }
            return contents.ToList();
        }

        public async Task<List<PixivWorkInfo>> getRankingWorks(List<PixivRankingContent> contents)
        {
            List<PixivWorkInfo> workInfos = new List<PixivWorkInfo>();
            foreach (var content in contents)
            {
                try
                {
                    PixivWorkInfo workInfo = await PixivHelper.GetPixivWorkInfoAsync(content.illust_id.ToString());
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex);
                }
            }
            return workInfos;
        }

        public async Task<List<PixivRankingPreview>> getRankingPreviews(List<PixivRankingContent> rankingContents)
        {
            List<PixivRankingPreview> previewFiles = new List<PixivRankingPreview>();
            foreach (var content in rankingContents)
            {
                string downloadUrl = content.url;
                string pixivId = content.illust_id.ToString();
                FileInfo previewFile = await PixivHelper.DownPixivImgAsync(pixivId, downloadUrl);
                PixivRankingPreview rankingPreview = new PixivRankingPreview(content, previewFile);
                previewFiles.Add(rankingPreview);
            }
            return previewFiles;
        }

        public string getRankingInfo(string date, string rankingName, string template)
        {
            if (string.IsNullOrWhiteSpace(template)) return getDefaultRankingInfo(date, rankingName);
            template = template.Replace("{Date}", date);
            template = template.Replace("{Ranking}", rankingName);
            return template;
        }

        public string getDefaultRankingInfo(string date, string rankingName)
        {
            return $"{date}{rankingName}内容如下：";
        }

        /// <summary>
        /// 检查日榜内容是否存在包含违禁标签
        /// </summary>
        /// <param name="rankingItem"></param>
        /// <param name="rankingContent"></param>
        /// <returns></returns>
        private bool checkContentIsOk(PixivRankingItem rankingItem, PixivRankingContent rankingContent)
        {
            if (rankingContent.isImproper()) return false;
            if (rankingContent.hasBanTag()) return false;
            if (rankingContent.isIllust() == false) return false;
            if (rankingContent.rating_count < rankingItem.MinRatingCount) return false;
            if (rankingContent.rating_rate < rankingItem.MinRatingRate) return false;
            return true;
        }

    }
}
