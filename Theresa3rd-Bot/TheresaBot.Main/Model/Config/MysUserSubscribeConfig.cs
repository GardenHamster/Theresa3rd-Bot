namespace TheresaBot.Main.Model.Config
{
    public record MysUserSubscribeConfig : BaseSubscribeConfig
    {
        public int ShelfLife { get; set; }

        public MysUserSubscribeConfig() : base()
        {
            ShelfLife = 12 * 60 * 60;
        }

        public override BasePluginConfig FormatConfig()
        {
            base.FormatConfig();
            ShelfLife = ShelfLife < 300 ? 300 : ShelfLife;
            ScanInterval = ScanInterval < 30 ? 30 : ScanInterval;
            return this;
        }

    }
}
