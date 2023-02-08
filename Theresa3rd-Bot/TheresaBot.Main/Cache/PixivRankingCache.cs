using TheresaBot.Main.Mode;
using TheresaBot.Main.Model.Cache;

namespace TheresaBot.Main.Cache
{
    public class PixivRankingCache
    {
        private static readonly Dictionary<PixivRankingMode, List<PixivRankingInfo>> CacheDic = new();

        public static void AddCache(PixivRankingMode rankingType, PixivRankingInfo rankingInfo)
        {
            ClearCahce();
            if (rankingInfo is null) return;
            if (CacheDic.ContainsKey(rankingType) == false) CacheDic[rankingType] = new();
            CacheDic[rankingType].Add(rankingInfo);
        }

        public static PixivRankingInfo GetCache(PixivRankingMode rankingType, string date = "")
        {
            ClearCahce();
            if (CacheDic.ContainsKey(rankingType) == false) return null;
            List<PixivRankingInfo> pixivRankingList = CacheDic[rankingType];
            if (pixivRankingList is null) return null;
            if (pixivRankingList.Count == 0) return null;
            return pixivRankingList.Where(o => o.Date == date).OrderByDescending(o => o.CreateDate).FirstOrDefault();
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
