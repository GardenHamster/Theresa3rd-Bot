using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.Config
{
    public record PixivUserSubscribeConfig : BaseSubscribeConfig
    {
        public PixivScanType ScanMode { get; set; } = PixivScanType.Default;

        public List<string> SyncCommands { get; set; }

        public int ShelfLife { get; set; } = 12 * 60 * 60;

        public bool SendMerge { get; set; } = false;

        public PixivUserSubscribeConfig()
        {
            this.ScanInterval = 60 * 60;
        }

    }
}
