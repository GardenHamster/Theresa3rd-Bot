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

        public override BasePluginConfig FormatConfig()
        {
            base.FormatConfig();
            if (ShelfLife < 0) ShelfLife = 0;
            if (MaxScan < 0) MaxScan = 0;
            if (MinBookmark < 0) MinBookmark = 0;
            if (MinBookPerHour < 0) MinBookPerHour = 0;
            if (MinBookRate < 0) MinBookRate = 0;
            return this;
        }


    }
}
