namespace TheresaBot.Main.Model.Config
{
    public record WordCloudConfig : BasePluginConfig
    {
        public int GroupCD { get; private set; }
        public int MaxWords { get; private set; }
        public int DefaultWidth { get; private set; }
        public int DefaultHeitht { get; private set; }
        public string FontPath { get; private set; }
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
        public List<string> DefaultMasks { get; private set; }
        public List<WordCloudMask> Masks { get; private set; }
        public List<WordCloudTimer> Subscribes { get; private set; }

        public override WordCloudConfig FormatConfig()
        {
            if (DefaultWidth <= 0) DefaultWidth = 1000;
            if (DefaultHeitht <= 0) DefaultHeitht = 1000;
            return this;
        }
    }

    public record WordCloudMask
    {
        public string Name { get; private set; }
        public string Path { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
    }

    public record WordCloudTimer
    {
        public bool Enable { get; private set; }
        public string Name { get; private set; }
        public string Cron { get; private set; }
        public List<string> Masks { get; private set; }
        public List<long> Groups { get; private set; }
        public double HourRange { get; private set; }
        public string Template { get; private set; }
    }



}
