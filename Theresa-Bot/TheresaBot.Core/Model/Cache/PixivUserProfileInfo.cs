using TheresaBot.Core.Model.Pixiv;

namespace TheresaBot.Core.Model.Cache
{
    public record PixivUserProfileInfo
    {
        public string UserId { get; private set; }

        public string UserName { get; private set; }

        public List<PixivProfileDetail> ProfileDetails { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ExpireDate { get; set; }

        public int CacheSecond { get; set; }

        public List<string> PreviewFilePaths { get; set; }

        public PixivUserProfileInfo(string userId, string userName, int cacheSecond, List<PixivProfileDetail> details = null)
        {
            this.UserId = userId;
            this.UserName = userName;
            this.ProfileDetails = details ?? new();
            this.CacheSecond = cacheSecond;
            this.CreateDate = DateTime.Now;
            this.ExpireDate = DateTime.Now.AddSeconds(cacheSecond);
        }

    }
}
