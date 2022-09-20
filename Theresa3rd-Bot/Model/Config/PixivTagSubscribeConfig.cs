namespace Theresa3rd_Bot.Model.Config
{
    public class PixivTagSubscribeConfig : BaseSubscribeConfig
    {
        public int ShelfLife { get; set; }

        public int MinBookmark { get; set; }

        public int MinBookPerHour { get; set; }

        public int MaxScan { get; set; }

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
