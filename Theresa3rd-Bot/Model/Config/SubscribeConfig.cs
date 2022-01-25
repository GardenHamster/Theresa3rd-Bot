namespace Theresa3rd_Bot.Model.Config
{
    public class SubscribeConfig
    {
        public PixivUserSubscribeConfig PixivUser { get; set; }

        public PixivTagSubscribeConfig PixivTag { get; set; }

        public BaseSubscribeConfig BiliUp { get; set; }

        public BaseSubscribeConfig BiliLive { get; set; }

        public BaseSubscribeConfig Mihoyo { get; set; }
    }

}
