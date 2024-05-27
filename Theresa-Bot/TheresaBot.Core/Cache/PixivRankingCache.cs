using TheresaBot.Core.Mode;
using TheresaBot.Core.Model.Cache;

namespace TheresaBot.Core.Cache
{
    public static class PixivRankingCache
    {
        private static readonly Dictionary<PixivRankingMode, List<PixivRankingInfo>> CacheDic = new();

        public static void AddCache(PixivRankingMode rankingMode, PixivRankingInfo rankingInfo, string searchDate)
        {
            ClearCahce();
            if (rankingInfo is null) return;
            if (CacheDic.ContainsKey(rankingMode) == false) CacheDic[rankingMode] = new();
            PixivRankingInfo cacheInfo = rankingInfo with { SearchDate = searchDate };
            CacheDic[rankingMode].RemoveAll(o => o.SearchDate == searchDate || o.RankingDate == searchDate);
            CacheDic[rankingMode].Add(cacheInfo);
        }

        public static PixivRankingInfo GetCache(PixivRankingMode rankingMode, string searchDate)
        {
            ClearCahce();
            if (CacheDic.ContainsKey(rankingMode) == false) return null;
            List<PixivRankingInfo> pixivRankingList = CacheDic[rankingMode];
            if (pixivRankingList is null) return null;
            if (pixivRankingList.Count == 0) return null;
            return pixivRankingList.Where(o => o.SearchDate == searchDate || o.RankingDate == searchDate).OrderByDescending(o => o.CreateDate).FirstOrDefault();
        }

        private static void ClearCahce()
        {
            foreach (var item in CacheDic)
            {
                if (item.Value == null || item.Value.Count == 0) continue;
                List<PixivRankingInfo> pixivRankingList = item.Value;
                pixivRankingList.RemoveAll(o => o.ExpireDate <= DateTime.Now);
            }
        }

    }
}
