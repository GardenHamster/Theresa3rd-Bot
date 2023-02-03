using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.Config
{
    public class PixivRankingConfig : BasePluginConfig
    {
        public string ProcessingMsg { get; set; }
        public string Template { get; set; }
        public PixivRankingSendMode SendMode { get; set; }
        public int GroupCD { get; set; }
        public int MaxQuantity { get; set; }
        public int CacheSeconds { get; set; }
        public PixivRankingContent Daily { get; set; }
        public PixivRankingContent DailyAI { get; set; }
        public PixivRankingContent DailyMale { get; set; }
        public PixivRankingContent Weekly { get; set; }
        public PixivRankingContent Monthly { get; set; }
        public List<PixivRankingSubscribe> Subscribe { get; set; }
    }

    public class PixivRankingContent
    {
        public List<string> Commands { get; set; }
        public int MinBookmark { get; set; }
        public int MinBookRate { get; set; }
    }

    public class PixivRankingSubscribe
    {
        public List<long> Groups { get; set; }
        public List<string> Content { get; set; }
        public string Cron { get; set; }
    }

}
