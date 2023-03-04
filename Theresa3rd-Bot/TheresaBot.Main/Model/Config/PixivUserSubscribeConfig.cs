using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.Config
{
    public class PixivUserSubscribeConfig : BaseSubscribeConfig
    {
        public PixivScanType ScanMode { get; private set; }

        public List<string> SyncCommands { get; private set; }

        public int ShelfLife { get; private set; }

        public PixivUserSubscribeConfig()
        {
            this.ScanMode = PixivScanType.Default;
            this.ShelfLife = 12 * 60 * 60;
            this.ScanInterval = 60 * 60;
        }

    }
}
