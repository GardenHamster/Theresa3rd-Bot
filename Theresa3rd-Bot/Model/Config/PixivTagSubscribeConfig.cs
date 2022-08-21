namespace Theresa3rd_Bot.Model.Config
{
    public class PixivTagSubscribeConfig : BaseSubscribeConfig
    {
        public int ShelfLife { get; set; }

        public int MinBookmark { get; set; }

        public int MinBookPerHour { get; set; }

        public int MaxScan { get; set; }
    }
}
