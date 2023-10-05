namespace TheresaBot.Main.Model.Config
{
    public record PixivTagSubscribeConfig : BaseSubscribeConfig
    {
        public int ShelfLife { get; set; } = 24 * 60 * 60;

        public int MaxScan { get; set; } = 300;

        public int MinBookmark { get; set; } = 300;

        public int MinBookPerHour { get; set; } = 50;

        public double MinBookRate { get; set; } = 0.05;

        public bool SendMerge { get; set; }

        public PixivTagSubscribeConfig()
        {
            this.ScanInterval = 30 * 60;
        }


    }
}
