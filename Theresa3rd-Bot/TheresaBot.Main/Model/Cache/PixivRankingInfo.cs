using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheresaBot.Main.Mode;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Model.PixivRanking;

namespace TheresaBot.Main.Model.Cache
{
    public record PixivRankingInfo
    {
        public string Date { get; set; }

        public string SearchDate { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ExpireDate { get; set; }

        public PixivRankingMode RankingMode { get; set; }

        public PixivRankingItem RankingItem { get; set; }

        public int CacheSecond { get; set; }

        public List<PixivRankingDetail> RankingDetails { get; set; }

        public List<string> PreviewFilePaths { get; set; }

        public PixivRankingInfo(List<PixivRankingDetail> rankingDetails, PixivRankingItem rankingItem, PixivRankingMode rankingMode, string date, int cacheSecond)
        {
            this.Date = date;
            this.RankingItem = rankingItem;
            this.RankingMode = rankingMode;
            this.CacheSecond = cacheSecond;
            this.RankingDetails = rankingDetails;
            this.CreateDate = DateTime.Now;
            this.ExpireDate = DateTime.Now.AddSeconds(cacheSecond);
        }

    }
}
