namespace TheresaBot.Main.Model.Config
{
    public record SubscribeConfig : BaseConfig
    {
        public PixivUserSubscribeConfig PixivUser { get; set; }

        public PixivTagSubscribeConfig PixivTag { get; set; }

        public MysUserSubscribeConfig Miyoushe { get; set; }

        public override SubscribeConfig FormatConfig()
        {
            if (PixivUser is not null) PixivUser.FormatConfig();
            if (PixivTag is not null) PixivTag.FormatConfig();
            if (Miyoushe is not null) Miyoushe.FormatConfig();
            return this;
        }

    }
}
