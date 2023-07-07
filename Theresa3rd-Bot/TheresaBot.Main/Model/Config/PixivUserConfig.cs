namespace TheresaBot.Main.Model.Config
{
    public class PixivUserConfig : BasePluginConfig
    {
        public List<string> Commands { get; private set; }
        public string ProcessingMsg { get; private set; }
        public string Template { get; private set; }
        public int MaxScan { get; private set; }
        public int PreviewInPage { get; private set; }
        public int CacheSeconds { get; private set; }

        public PixivUserConfig()
        {
            this.MaxScan = 90;
            this.PreviewInPage = 30;
            this.CacheSeconds = 3600;
        }

    }
}
