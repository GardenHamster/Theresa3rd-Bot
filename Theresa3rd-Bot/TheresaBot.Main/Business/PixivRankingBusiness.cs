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

        public async Task<(List<PixivRankingContent>, string)> getRankingDatas(PixivRankingItem rankingItem, string mode)
        {
            string date = string.Empty;
            int maxShow = BotConfig.PixivRankingConfig.MaxShow;
            int maxPage = MathHelper.getMaxPage(maxShow, eachPage);
            List<PixivRankingContent> rankingContents = new List<PixivRankingContent>();
            for (int page = 1; page < maxPage + 1; page++)
            {
                PixivRankingData rankingData = await PixivHelper.GetPixivRankingData(mode, page);
                if (page == 1) date = rankingData.date;
                if (rankingData.contents is null || rankingData.contents.Count == 0) throw new ApiException("无法从api中获取任何排行信息");
                rankingContents.AddRange(rankingData.contents);
            }

            List<PixivRankingContent> filterContents = new List<PixivRankingContent>();
            for (int i = 0; i < rankingContents.Count && i < maxShow; i++)
            {
                if (filterContents.Count >= maxShow) break;
                if (checkContentIsOk(rankingContents[i]) == false) continue;
                filterContents.Add(rankingContents[i]);
            }
            return (filterContents, date);
        }

        public async Task<List<PixivWorkInfo>> getRankingWorks(PixivRankingItem rankingItem, List<PixivRankingContent> contents)
        {
            List<PixivWorkInfo> workInfos = new List<PixivWorkInfo>();
            foreach (var content in contents)
            {
                try
                {
                    PixivWorkInfo workInfo = await PixivHelper.GetPixivWorkInfoAsync(content.illust_id.ToString());
                    if (checkWorkIsOk(rankingItem, workInfo)) workInfos.Add(workInfo);
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
                string pixivId = content.illust_id.ToString();
                string downloadUrl = content.url;
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
        private bool checkContentIsOk(PixivRankingContent rankingContent)
        {
            if (rankingContent.isImproper()) return false;
            if (rankingContent.hasBanTag()) return false;
            return true;
        }

        /// <summary>
        /// 检查日榜内容是否合格
        /// </summary>
        /// <param name="rankingItem"></param>
        /// <param name="workInfo"></param>
        /// <returns></returns>
        private bool checkWorkIsOk(PixivRankingItem rankingItem, PixivWorkInfo workInfo)
        {
            if (workInfo.illustType != 0 && BotConfig.PixivRankingConfig.IllustOnly) return false;
            if (workInfo.likeCount < rankingItem.MinRatingCount) return false;
            double ratingRate = Convert.ToDouble(workInfo.likeCount) / workInfo.viewCount;
            if (ratingRate < rankingItem.MinRatingRate) return false;
            if (workInfo.IsImproper) return false;
            if (workInfo.hasBanTag() is not null) return false;
            return true;
        }

        

    }
}
