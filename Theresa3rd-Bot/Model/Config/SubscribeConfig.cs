namespace Theresa3rd_Bot.Model.Config
{
    public class SubscribeConfig
    {
        public SubscribeModel PixivUser { get; set; }

        public SubscribeModel PixivTag { get; set; }

        public SubscribeModel BiliUp { get; set; }

        public SubscribeModel BiliLive { get; set; }

        public SubscribeModel Mihoyo { get; set; }
    }

    public class SubscribeModel
    {
        public bool Enable { get; set; }

        public string AddCommand { get; set; }

        public string RmCommand { get; set; }

        public string Template { get; set; }

        public int ScanInterval { get; set; }
    }

}
