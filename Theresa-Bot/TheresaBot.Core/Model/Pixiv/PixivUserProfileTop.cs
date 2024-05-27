namespace TheresaBot.Core.Model.Pixiv
{
    public class PixivUserProfileTop
    {
        public Dictionary<string, PixivUserWorkInfo> illusts { get; set; }
        public PixivUserExtraData extraData { get; set; }
        public string UserName => extraData?.meta?.UserName ?? string.Empty;
    }

    public class PixivUserExtraData
    {
        public PixivUserMeta meta { get; set; }
    }

    public class PixivUserMeta
    {
        public string title { get; set; }
        public PixivUserOgp ogp { get; set; }
        public string UserName => ogp?.title ?? title?.Replace("- pixiv", "")?.Trim() ?? string.Empty;
        public string Description => ogp?.description ?? string.Empty;
    }

    public class PixivUserOgp
    {
        public string description { get; set; }
        public string title { get; set; }
        public string type { get; set; }
    }

}
