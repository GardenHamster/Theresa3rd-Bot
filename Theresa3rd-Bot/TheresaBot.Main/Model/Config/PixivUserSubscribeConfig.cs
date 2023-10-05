using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.Config
{
    public record PixivUserSubscribeConfig : BaseSubscribeConfig
    {
        public PixivScanType ScanMode { get; private set; } = PixivScanType.Default;

        public List<string> SyncCommands { get; private set; }

        public int ShelfLife { get; private set; } = 12 * 60 * 60;

        public bool SendMerge { get; set; } = false;

        public PixivUserSubscribeConfig()
        {
            this.ScanInterval = 60 * 60;
        }

    }
}
