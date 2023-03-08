namespace TheresaBot.Main.Model.Config
{
    public class PixivTagSubscribeConfig : BaseSubscribeConfig
    {
        public int ShelfLife { get; private set; } = 24 * 60 * 60;

        public int MaxScan { get; private set; } = 300;

        public int MinBookmark { get; private set; } = 300;

        public int MinBookPerHour { get; private set; } = 50;

        public double MinBookRate { get; private set; } = 0.05;
    }
}
