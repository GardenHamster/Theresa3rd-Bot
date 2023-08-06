namespace TheresaBot.Main.Model.Config
{
    public class SubscribeConfig : BaseConfig
    {
        public PixivUserSubscribeConfig PixivUser { get; private set; }

        public PixivTagSubscribeConfig PixivTag { get; private set; }

        public MysUserSubscribeConfig Miyoushe { get; private set; }

        public override SubscribeConfig FormatConfig()
        {
            PixivUser.FormatConfig();
            PixivTag.FormatConfig();
            Miyoushe.FormatConfig();
            return this;
        }

    }

}
