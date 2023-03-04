namespace TheresaBot.Main.Model.Config
{
    public class PixivTagSubscribeConfig : BaseSubscribeConfig
    {
        public int ShelfLife { get; private set; }

        public int MinBookmark { get; private set; }

        public int MinBookPerHour { get; private set; }

        public int MaxScan { get; private set; }

        public PixivTagSubscribeConfig()
        {
            this.ScanInterval = 30 * 60;
            this.ShelfLife = 24 * 60 * 60;
            this.MinBookmark = 300;
            this.MinBookPerHour = 60;
            this.MaxScan = 300;
        }
    }
}
