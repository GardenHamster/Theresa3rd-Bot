using TheresaBot.Main.Common;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Model.Pixiv;
using TheresaBot.Main.Model.PixivRanking;

namespace TheresaBot.Main.Business
{
    public class PixivRankingBusiness : SetuBusiness
    {
        private const int eachPage = 50;

        public async Task<(List<PixivRankingContent>, string)> getRankingData(PixivRankingItem rankingItem, string mode)
        {
            string date = string.Empty;
            int maxShow = BotConfig.PixivRankingConfig.MaxShow;
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
                if (filterContents.Count >= maxShow) break;
                if (checkContentIsOk(rankingItem, rankingContents[i]) == false) continue;
                filterContents.Add(rankingContents[i]);
            }
            return (filterContents, date);
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

        public bool checkRankingWorkIsOk(PixivRankingItem rankingItem, PixivWorkInfo workInfo)
        {
            if (workInfo.illustType != 0 && BotConfig.PixivRankingConfig.IllustOnly) return false;
            if (workInfo.likeCount < rankingItem.MinRatingCount) return false;
            double ratingRate = Convert.ToDouble(workInfo.likeCount) / workInfo.viewCount;
            if (ratingRate < rankingItem.MinRatingRate) return false;
            if (workInfo.IsImproper) return false;
            if (workInfo.hasBanTag() is not null) return false;
            return true;
        }

        private bool checkContentIsOk(PixivRankingItem rankingItem, PixivRankingContent rankingContent)
        {
            if (rankingContent.illust_type != "0" && BotConfig.PixivRankingConfig.IllustOnly) return false;
            if (rankingContent.rating_count < rankingItem.MinRatingCount) return false;
            double ratingRate = Convert.ToDouble(rankingContent.rating_count) / rankingContent.view_count;
            if (ratingRate < rankingItem.MinRatingRate) return false;
            if (rankingContent.isImproper()) return false;
            if (rankingContent.hasBanTag()) return false;
            return true;
        }

    }
}
