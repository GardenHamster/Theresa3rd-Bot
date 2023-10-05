using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.Config
{
    public record PixivRankingConfig : BasePluginConfig
    {
        public string ProcessingMsg { get; set; }
        public string Template { get; set; }
        public int MaxScan { get; set; }
        public int PreviewInPage { get; set; }
        public PixivRankingSortType SortType { get; set; } = PixivRankingSortType.RankingRate;
        public int GroupCD { get; set; }
        public int CacheSeconds { get; set; }
        public int SendDetail { get; set; }
        public PixivRankingItem Daily { get; set; }
        public PixivRankingItem DailyAI { get; set; }
        public PixivRankingItem Male { get; set; }
        public PixivRankingItem Weekly { get; set; }
        public PixivRankingItem Monthly { get; set; }
        public List<PixivRankingTimer> Subscribes { get; set; }

        public override PixivRankingConfig FormatConfig()
        {
            return this;
        }
    }

    public record PixivRankingItem
    {
        public bool Enable { get; private set; }
        public List<string> Commands { get; private set; }
        public List<string> R18Commands { get; private set; }
        public int MinRatingCount { get; private set; }
        public double MinRatingRate { get; private set; }
        public int MinBookCount { get; private set; }
        public double MinBookRate { get; private set; }
    }

    public record PixivRankingTimer
    {
        public bool Enable { get; private set; }
        public string Name { get; private set; }
        public List<long> Groups { get; private set; }
        public List<string> Contents { get; private set; }
        public string Cron { get; private set; }
        public int SendDetail { get; private set; }
    }

}
