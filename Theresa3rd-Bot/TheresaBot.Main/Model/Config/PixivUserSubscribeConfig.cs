using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.Config
{
    public record PixivUserSubscribeConfig : BaseSubscribeConfig
    {
        public PixivUserScanType ScanMode { get; set; } = PixivUserScanType.Default;

        public List<string> SyncCommands { get; set; } = new();

        public int ShelfLife { get; set; } = 12 * 60 * 60;

        public bool SendMerge { get; set; } = false;

        public PixivUserSubscribeConfig()
        {
            this.ScanInterval = 60 * 60;
        }

        public override BasePluginConfig FormatConfig()
        {
            base.FormatConfig();
            if (SyncCommands is null) SyncCommands = new();
            if (ShelfLife < 0) ShelfLife = 0;
            return this;
        }

    }
}
