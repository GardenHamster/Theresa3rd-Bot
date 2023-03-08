using TheresaBot.Main.Helper;

namespace TheresaBot.Main.Model.Config
{
    public class PixivConfig
    {
        public bool FreeProxy { get; private set; }
        public string HttpProxy { get; private set; }
        public string ImgProxy { get; private set; }
        public int ImgShowMaximum { get; private set; }
        public int TagShowMaximum { get; private set; }
        public int UrlShowMaximum { get; private set; }
        public string ImgSize { get; private set; }
        public int ImgResend { get; private set; }
        public float R18ImgBlur { get; private set; }
        public string OriginUrlProxy { get; private set; }
        public bool SendImgBehind { get; private set; }
        public int ImgRetryTimes { get; private set; }
        public int ErrRetryTimes { get; private set; }
        public int CookieExpire { get; private set; }
        public string CookieExpireMsg { get; private set; }
        public string Template { get; private set; }
        public PixivConfig()
        {
            this.ImgShowMaximum = 1;
            this.TagShowMaximum = 3;
            this.UrlShowMaximum = 3;
            this.R18ImgBlur = 10;
        }

        public PixivConfig FormatConfig()
        {
            this.ImgProxy = StringHelper.formatHttpUrl(ImgProxy);
            this.HttpProxy = StringHelper.formatHttpUrl(HttpProxy, false);
            this.OriginUrlProxy = StringHelper.formatHttpUrl(OriginUrlProxy);
            if (this.R18ImgBlur < 5) this.R18ImgBlur = 5;
            if (this.R18ImgBlur > 100) this.R18ImgBlur = 100;
            return this;
        }

    }
}
