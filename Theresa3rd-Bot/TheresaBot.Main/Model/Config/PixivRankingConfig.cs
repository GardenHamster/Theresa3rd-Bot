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
        public List<PixivRankingTimer> Subscribes { get; set; } = new();

        public override PixivRankingConfig FormatConfig()
        {
            if (MaxScan < 0) MaxScan = 0;
            if (MaxScan > 1000) MaxScan = 1000;
            if (PreviewInPage <= 0) PreviewInPage = 25;
            if (PreviewInPage > 50) PreviewInPage = 50;
            if (SendDetail < 0) SendDetail = 0;
            if (Subscribes is null) Subscribes = new();
            if (Daily is not null) Daily.FormatConfig();
            if (DailyAI is not null) DailyAI.FormatConfig();
            if (Male is not null) Male.FormatConfig();
            if (Weekly is not null) Weekly.FormatConfig();
            if (Monthly is not null) Monthly.FormatConfig();
            foreach (var item in Subscribes) item?.FormatConfig();
            return this;
        }
    }

    public record PixivRankingItem : BaseConfig
    {
        public bool Enable { get; private set; }
        public List<string> Commands { get; private set; } = new();
        public List<string> R18Commands { get; private set; } = new();
        public int MinRatingCount { get; private set; }
        public double MinRatingRate { get; private set; }
        public int MinBookCount { get; private set; }
        public double MinBookRate { get; private set; }

        public override BaseConfig FormatConfig()
        {
            if (MinRatingCount < 0) MinRatingCount = 0;
            if (MinRatingRate < 0) MinRatingRate = 0;
            if (MinBookCount < 0) MinBookCount = 0;
            if (MinBookRate < 0) MinBookRate = 0;
            if (Commands is null) Commands = new();
            if (R18Commands is null) R18Commands = new();
            return this;
        }
    }

    public record PixivRankingTimer : BaseConfig
    {
        public bool Enable { get; private set; }
        public string Name { get; private set; }
        public List<long> Groups { get; private set; } = new();
        public List<string> Contents { get; private set; } = new();
        public string Cron { get; private set; }
        public int SendDetail { get; private set; }

        public override BaseConfig FormatConfig()
        {
            if (SendDetail < 0) SendDetail = 0;
            if (Groups is null) Groups = new();
            if (Contents is null) Contents = new();
            return this;
        }
    }

}
