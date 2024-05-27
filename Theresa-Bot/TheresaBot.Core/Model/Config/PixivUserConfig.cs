namespace TheresaBot.Core.Model.Config
{
    public record PixivUserConfig : BasePluginConfig
    {
        public List<string> Commands { get; set; } = new();
        public string ProcessingMsg { get; set; }
        public string Template { get; set; }
        public int MaxScan { get; set; } = 90;
        public int PreviewInPage { get; set; } = 25;
        public int CacheSeconds { get; set; } = 60 * 60;

        public override BasePluginConfig FormatConfig()
        {
            if (Commands is null) Commands = new();
            return this;
        }

    }
}
