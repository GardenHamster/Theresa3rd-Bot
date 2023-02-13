using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.Config
{
    public class PixivRankingConfig : BasePluginConfig
    {
        public string ProcessingMsg { get; set; }
        public string Template { get; set; }
        public int MaxScan { get; set; }
        public int PreviewInPage { get; set; }
        public PixivRankingSortType SortType { get; set; }
        public int GroupCD { get; set; }
        public int CacheSeconds { get; set; }
        public int SendDetail { get; set; }
        public PixivRankingItem Daily { get; set; }
        public PixivRankingItem DailyAI { get; set; }
        public PixivRankingItem Male { get; set; }
        public PixivRankingItem Weekly { get; set; }
        public PixivRankingItem Monthly { get; set; }
        public List<PixivRankingTimer> Subscribes { get; set; }
    }

    public class PixivRankingItem
    {
        public bool Enable { get; set; }
        public List<string> Commands { get; set; }
        public int MinRatingCount { get; set; }
        public double MinRatingRate { get; set; }
        public int MinBookCount { get; set; }
        public double MinBookRate { get; set; }
    }

    public class PixivRankingTimer
    {
        public bool Enable { get; set; }
        public string Name { get; set; }
        public List<long> Groups { get; set; }
        public List<string> Contents { get; set; }
        public string Cron { get; set; }
        public int SendDetail { get; set; }
    }

}
