namespace TheresaBot.Main.Model.Config
{
    public record SubscribeConfig : BaseConfig
    {
        public PixivUserSubscribeConfig PixivUser { get; set; }

        public PixivTagSubscribeConfig PixivTag { get; set; }

        public MysUserSubscribeConfig Miyoushe { get; set; }

        public override SubscribeConfig FormatConfig()
        {
            PixivUser.FormatConfig();
            PixivTag.FormatConfig();
            Miyoushe.FormatConfig();
            return this;
        }

    }

}
