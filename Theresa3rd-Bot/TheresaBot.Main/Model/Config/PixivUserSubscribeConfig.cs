using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.Config
{
    public class PixivUserSubscribeConfig : BaseSubscribeConfig
    {
        public PixivScanMode ScanMode { get; set; }

        public List<string> SyncCommands { get; set; }

        public int ShelfLife { get; set; }

        public PixivUserSubscribeConfig()
        {
            this.ScanMode = PixivScanMode.Default;
            this.ShelfLife = 12 * 60 * 60;
            this.ScanInterval = 60 * 60;
        }

    }
}
