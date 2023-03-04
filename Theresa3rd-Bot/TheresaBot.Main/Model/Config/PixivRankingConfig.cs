using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.Config
{
    public class PixivRankingConfig : BasePluginConfig
    {
        public string ProcessingMsg { get; private set; }
        public string Template { get; private set; }
        public int MaxScan { get; private set; }
        public int PreviewInPage { get; private set; }
        public PixivRankingSortType SortType { get; private set; }
        public int GroupCD { get; private set; }
        public int CacheSeconds { get; private set; }
        public int SendDetail { get; private set; }
        public PixivRankingItem Daily { get; private set; }
        public PixivRankingItem DailyAI { get; private set; }
        public PixivRankingItem Male { get; private set; }
        public PixivRankingItem Weekly { get; private set; }
        public PixivRankingItem Monthly { get; private set; }
        public List<PixivRankingTimer> Subscribes { get; private set; }
    }

    public class PixivRankingItem
    {
        public bool Enable { get; private set; }
        public List<string> Commands { get; private set; }
        public int MinRatingCount { get; private set; }
        public double MinRatingRate { get; private set; }
        public int MinBookCount { get; private set; }
        public double MinBookRate { get; private set; }
    }

    public class PixivRankingTimer
    {
        public bool Enable { get; private set; }
        public string Name { get; private set; }
        public List<long> Groups { get; private set; }
        public List<string> Contents { get; private set; }
        public string Cron { get; private set; }
        public int SendDetail { get; private set; }
    }

}
