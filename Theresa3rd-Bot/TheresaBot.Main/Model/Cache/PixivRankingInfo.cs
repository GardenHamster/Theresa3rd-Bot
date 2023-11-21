using TheresaBot.Main.Mode;
using TheresaBot.Main.Model.Pixiv;

namespace TheresaBot.Main.Model.Cache
{
    public record PixivRankingInfo
    {
        public string RankingDate { get; set; }

        public string SearchDate { get; set; }

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
