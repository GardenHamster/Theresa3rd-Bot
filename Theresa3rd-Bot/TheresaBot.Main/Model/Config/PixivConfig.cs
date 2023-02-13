namespace TheresaBot.Main.Model.Config
{
    public class PixivConfig
    {
        public bool FreeProxy { get; set; }
        public string HttpProxy { get; set; }
        public string ImgProxy { get; set; }
        public int ImgShowMaximum { get; set; }
        public int TagShowMaximum { get; set; }
        public int UrlShowMaximum { get; set; }
        public string ImgSize { get; set; }
        public string OriginUrlProxy { get; set; }
        public bool SendImgBehind { get; set; }
        public int ImgRetryTimes { get; set; }
        public int ErrRetryTimes { get; set; }
        public int CookieExpire { get; set; }
        public string CookieExpireMsg { get; set; }
        public string Template { get; set; }
        public PixivConfig()
        {
            this.ImgShowMaximum = 1;
            this.TagShowMaximum = 3;
            this.UrlShowMaximum = 3;
        }
    }
}
