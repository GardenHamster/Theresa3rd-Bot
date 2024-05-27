using TheresaBot.Core.Mode;
using TheresaBot.Core.Model.Pixiv;

namespace TheresaBot.Core.Model.Cache
{
    public record PixivRankingInfo
    {

        /// <summary>
        /// 搜索指令中传入的日期，有可能是空字符串，空字符串表示日榜最新日期
        /// </summary>
        public string SearchDate { get; set; }
        /// <summary>
        /// 榜单日期
        /// </summary>
        public string RankingDate { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ExpireDate { get; set; }

        public PixivRankingMode RankingMode { get; set; }

        public int CacheSecond { get; set; }

        public List<PixivRankingDetail> RankingDetails { get; set; }

        public List<string> PreviewFilePaths { get; set; }

        public PixivRankingInfo(List<PixivRankingDetail> rankingDetails, PixivRankingMode rankingMode, string rankingDate, int cacheSecond)
        {
            this.RankingDate = rankingDate;
            this.RankingMode = rankingMode;
            this.CacheSecond = cacheSecond;
            this.RankingDetails = rankingDetails;
            this.CreateDate = DateTime.Now;
            this.ExpireDate = DateTime.Now.AddSeconds(cacheSecond);
        }

    }
}
