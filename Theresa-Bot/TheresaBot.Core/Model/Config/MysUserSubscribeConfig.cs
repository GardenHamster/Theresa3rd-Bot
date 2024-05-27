namespace TheresaBot.Core.Model.Config
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
            if (ShelfLife < 300) ShelfLife = 300;
            if (ScanInterval < 30) ScanInterval = 30;
            return this;
        }

    }
}
