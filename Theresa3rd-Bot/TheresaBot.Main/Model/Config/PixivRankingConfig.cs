using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.Config
{
    public class PixivRankingConfig : BasePluginConfig
    {
        public string ProcessingMsg { get; set; }
        public string Template { get; set; }
        public bool SendPreview { get; set; }
        public bool SendDetail { get; set; }
        public bool SendMerge { get; set; }
        public int GroupCD { get; set; }
        public int MaxScan { get; set; }
        public int MaxShow { get; set; }
        public bool IllustOnly { get; set; }
        public bool IncludeR18 { get; set; }
        public int CacheSeconds { get; set; }
        public PixivRankingItem Daily { get; set; }
        public PixivRankingItem DailyAI { get; set; }
        public PixivRankingItem DailyMale { get; set; }
        public PixivRankingItem Weekly { get; set; }
        public PixivRankingItem Monthly { get; set; }
        public List<PixivRankingSubscribe> Subscribe { get; set; }
    }

    public class PixivRankingItem
    {
        public List<string> Commands { get; set; }
        public int MinRatingCount { get; set; }
        public double MinRatingRate { get; set; }
    }

    public class PixivRankingSubscribe
    {
        public List<long> Groups { get; set; }
        public List<string> Content { get; set; }
        public string Cron { get; set; }
    }

}
