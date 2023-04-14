using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheresaBot.Main.Model.Pixiv;

namespace TheresaBot.Main.Model.Cache
{
    public record PixivUserProfileInfo
    {
        public string UserId { get; private set; }

        public string UserName { get; private set; }

        public List<PixivUserWorkInfo> WorkInfos { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ExpireDate { get; set; }

        public int CacheSecond { get; set; }

        public List<string> PreviewFilePaths { get; set; }

        public PixivUserProfileInfo(string userId, string userName, int cacheSecond, List<PixivUserWorkInfo> workInfos = null)
        {
            this.UserId = userId;
            this.UserName = userName;
            this.WorkInfos = workInfos ?? new();
            this.CacheSecond = cacheSecond;
            this.CreateDate = DateTime.Now;
            this.ExpireDate = DateTime.Now.AddSeconds(cacheSecond);
        }

    }
}
