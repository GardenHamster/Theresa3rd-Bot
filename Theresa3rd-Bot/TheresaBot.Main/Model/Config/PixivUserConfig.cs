namespace TheresaBot.Main.Model.Config
{
    public record PixivUserConfig : BasePluginConfig
    {
        public List<string> Commands { get; set; }
        public string ProcessingMsg { get; set; }
        public string Template { get; set; }
        public int MaxScan { get; set; }
        public int PreviewInPage { get; set; }
        public int CacheSeconds { get; set; }

        public PixivUserConfig()
        {
            this.MaxScan = 90;
            this.PreviewInPage = 30;
            this.CacheSeconds = 3600;
        }

        public override BasePluginConfig FormatConfig()
        {
            return this;
        }

    }
}
