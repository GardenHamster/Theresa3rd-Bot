using TheresaBot.Core.Model.Cache;

namespace TheresaBot.Core.Cache
{
    public static class PixivUserProfileCache
    {
        private static readonly Dictionary<string, List<PixivUserProfileInfo>> CacheDic = new();

        public static void AddCache(string userId, PixivUserProfileInfo profileInfo)
        {
            ClearCahce();
            if (profileInfo is null) return;
            if (CacheDic.ContainsKey(userId) == false) CacheDic[userId] = new();
            CacheDic[userId].Add(profileInfo);
        }

        public static PixivUserProfileInfo GetCache(string userId)
        {
            ClearCahce();
            if (CacheDic.ContainsKey(userId) == false) return null;
            List<PixivUserProfileInfo> pixivProfileList = CacheDic[userId];
            if (pixivProfileList is null) return null;
            if (pixivProfileList.Count == 0) return null;
            return pixivProfileList.OrderByDescending(o => o.CreateDate).FirstOrDefault();
        }

        private static void ClearCahce()
        {
            foreach (var item in CacheDic)
            {
                if (item.Value == null || item.Value.Count == 0) continue;
                List<PixivUserProfileInfo> pixivProfileList = item.Value;
                pixivProfileList.RemoveAll(o => o.ExpireDate <= DateTime.Now);
            }
        }

    }
}
