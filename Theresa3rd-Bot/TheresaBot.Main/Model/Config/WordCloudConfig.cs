namespace TheresaBot.Main.Model.Config
{
    public record WordCloudConfig : BasePluginConfig
    {
        public int GroupCD { get; set; }
        public int MaxWords { get; set; }
        public int DefaultWidth { get; set; }
        public int DefaultHeitht { get; set; }
        public string FontPath { get; set; }
        public string ProcessingMsg { get; set; }
        public List<string> BasicCommands { get; set; }
        public List<string> DailyCommands { get; set; }
        public List<string> WeeklyCommands { get; set; }
        public List<string> MonthlyCommands { get; set; }
        public List<string> YearlyCommands { get; set; }
        public List<string> YesterdayCommands { get; set; }
        public List<string> LastWeekCommands { get; set; }
        public List<string> LastMonthCommands { get; set; }
        public List<string> AddWordCommands { get; set; }
        public List<string> HideWordCommands { get; set; }
        public List<string> DefaultMasks { get; set; }
        public List<WordCloudMask> Masks { get; set; }
        public List<WordCloudTimer> Subscribes { get; set; }

        public override WordCloudConfig FormatConfig()
        {
            if (DefaultWidth <= 0) DefaultWidth = 1000;
            if (DefaultHeitht <= 0) DefaultHeitht = 1000;
            return this;
        }
    }

    public record WordCloudMask
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public record WordCloudTimer
    {
        public bool Enable { get; set; }
        public string Name { get; set; }
        public string Cron { get; set; }
        public List<string> Masks { get; set; }
        public List<long> Groups { get; set; }
        public double HourRange { get; set; }
        public string Template { get; set; }
    }



}
