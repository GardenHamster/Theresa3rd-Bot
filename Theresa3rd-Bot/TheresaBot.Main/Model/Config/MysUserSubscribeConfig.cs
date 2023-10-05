namespace TheresaBot.Main.Model.Config
{
    public record MysUserSubscribeConfig : BaseSubscribeConfig
    {
        public int ShelfLife { get; private set; }

        public MysUserSubscribeConfig()
        {
            this.ShelfLife = 12 * 60 * 60;
            this.ScanInterval = 60;
        }

    }
}
