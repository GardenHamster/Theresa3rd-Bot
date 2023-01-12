using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Model.Config
{
    public class PixivConfig
    {
        public bool FreeProxy { get; set; }

        public string HttpProxy { get; set; }

        public string ImgProxy { get; set; }

        public int TagShowMaximum { get; set; }

        public int UrlShowMaximum { get; set; }

        public string ImgSize { get; set; }

        public string OriginUrlProxy { get; set; }

        public int ImgRetryTimes { get; set; }

        public int ErrRetryTimes { get; set; }

        public int CookieExpire { get; set; }

        public string CookieExpireMsg { get; set; }

        public string Template { get; set; }

        public PixivConfig()
        {
            this.TagShowMaximum = 3;
            this.UrlShowMaximum = 3;
        }
    }
}
