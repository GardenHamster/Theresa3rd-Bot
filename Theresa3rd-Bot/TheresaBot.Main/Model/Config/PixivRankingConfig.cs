using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.Config
{
    public class PixivRankingConfig : BasePluginConfig
    {
        public string ProcessingMsg { get; set; }
        public string Template { get; set; }
        public List<string> DailyCommands { get; set; }
        public List<string> DailyAICommands { get; set; }
        public List<string> DailyMaleCommands { get; set; }
        public List<string> WeeklyCommands { get; set; }
        public List<string> MonthlyCommands { get; set; }
        public int GroupCD { get; set; }
        public PixivRankingSendMode SendMode { get; set; }
        public bool R18Ranking { get; set; }
        public int MaxQuantity { get; set; }
        public bool IllustOnly { get; set; }
        public int CacheSeconds { get; set; }
        public List<PixivRankingSubscribe> Subscribe { get; set; }
    }

    public class PixivRankingSubscribe
    {
        public List<long> Groups { get; set; }
        public List<string> Content { get; set; }
        public string Cron { get; set; }
    }

}
