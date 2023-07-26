namespace TheresaBot.Main.Model.Config
{
    public class WordCloudConfig : BasePluginConfig
    {
        public int GroupCD { get; private set; }
        public string ProcessingMsg { get; private set; }
        public List<string> BasicCommands { get; private set; }
        public List<string> DailyCommands { get; private set; }
        public List<string> WeeklyCommands { get; private set; }
        public List<string> MonthlyCommands { get; private set; }
        public List<string> YearlyCommands { get; private set; }
        public List<string> YesterdayCommands { get; private set; }
        public List<string> LastWeekCommands { get; private set; }
        public List<string> LastMonthCommands { get; private set; }
        public List<string> AddWordCommands { get; private set; }
        public List<string> HideWordCommands { get; private set; }
        public int ImgWidth { get; private set; }
        public int ImgHeight { get; private set; }
        public int MaxWords { get; private set; }
        public string FontPath { get; private set; }
        public List<string> MaskPaths { get; private set; }
        public List<WordCloudTimer> Subscribes { get; private set; }
    }

    public class WordCloudTimer
    {
        public bool Enable { get; private set; }
        public string Name { get; private set; }
        public string Cron { get; private set; }
        public List<long> Groups { get; private set; }
        public double HourRange { get; private set; }
        public string Template { get; private set; }
    }



}
