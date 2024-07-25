using TheresaBot.Core.Helper;
using TheresaBot.Core.Type;

namespace TheresaBot.Core.Model.Config
{
    public record PixivConfig : BaseConfig
    {
        public bool FreeProxy { get; set; }
        public string HttpProxy { get; set; }
        public string ImgProxy { get; set; }
        public int ImgShowMaximum { get; set; } = 1;
        public int TagShowMaximum { get; set; } = 5;
        public int UrlShowMaximum { get; set; } = 3;
        public string ImgSize { get; set; } = PixivImageSize.Thumb;
        public ResendType ImgResend { get; set; } = ResendType.None;
        public float R18ImgBlur { get; set; } = 10;
        public string OriginUrlProxy { get; set; }
        public bool SendImgBehind { get; set; }
        public int ImgRetryTimes { get; set; } = 1;
        public int ErrRetryTimes { get; set; } = 1;
        public int CookieExpire { get; set; } = 60 * 24 * 60 * 60;
        public string CookieExpireMsg { get; set; }
        public string Template { get; set; }
        public double GeneralTarget { get; set; } = 1.0;
        public double AITarget { get; set; } = 0.5;
        public double R18Target { get; set; } = 1.2;

        public override PixivConfig FormatConfig()
        {
            HttpProxy = UrlHelper.FormatHttpUrl(HttpProxy, defaultHttps: false);
            ImgProxy = UrlHelper.FormatHttpUrl(ImgProxy, defaultHttps: true);
            OriginUrlProxy = UrlHelper.FormatHttpUrl(OriginUrlProxy, defaultHttps: true);
            if (R18ImgBlur < 5) R18ImgBlur = 5;
            if (R18ImgBlur > 100) R18ImgBlur = 100;
            return this;
        }

    }
}
