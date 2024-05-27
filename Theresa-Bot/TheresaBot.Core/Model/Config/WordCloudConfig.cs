using TheresaBot.Core.Helper;
using YamlDotNet.Serialization;

namespace TheresaBot.Core.Model.Config
{
    public record WordCloudConfig : BasePluginConfig
    {
        public int GroupCD { get; set; }
        public int MaxWords { get; set; }
        public int DefaultWidth { get; set; }
        public int DefaultHeight { get; set; }
        public string FontPath { get; set; }
        public string ProcessingMsg { get; set; }
        public List<string> BasicCommands { get; set; } = new();
        public List<string> DailyCommands { get; set; } = new();
        public List<string> WeeklyCommands { get; set; } = new();
        public List<string> MonthlyCommands { get; set; } = new();
        public List<string> YearlyCommands { get; set; } = new();
        public List<string> YesterdayCommands { get; set; } = new();
        public List<string> LastWeekCommands { get; set; } = new();
        public List<string> LastMonthCommands { get; set; } = new();
        public List<string> AddWordCommands { get; set; } = new();
        public List<string> HideWordCommands { get; set; } = new();
        public List<string> DefaultMasks { get; set; } = new();
        public List<WordCloudMask> Masks { get; set; } = new();
        public List<WordCloudTimer> Subscribes { get; set; } = new();

        public override WordCloudConfig FormatConfig()
        {
            if (GroupCD < 0) GroupCD = 0;
            if (MaxWords < 1) MaxWords = 1;
            if (MaxWords > 1000) MaxWords = 1000;
            if (DefaultWidth < 200) DefaultWidth = 200;
            if (DefaultWidth > 2000) DefaultWidth = 2000;
            if (DefaultHeight < 200) DefaultHeight = 200;
            if (DefaultHeight > 2000) DefaultHeight = 2000;
            if (BasicCommands is null) BasicCommands = new();
            if (DailyCommands is null) DailyCommands = new();
            if (WeeklyCommands is null) WeeklyCommands = new();
            if (MonthlyCommands is null) MonthlyCommands = new();
            if (YearlyCommands is null) YearlyCommands = new();
            if (YesterdayCommands is null) YesterdayCommands = new();
            if (LastWeekCommands is null) LastWeekCommands = new();
            if (LastMonthCommands is null) LastMonthCommands = new();
            if (AddWordCommands is null) AddWordCommands = new();
            if (HideWordCommands is null) HideWordCommands = new();
            if (DefaultMasks is null) DefaultMasks = new();
            if (Masks is null) Masks = new();
            if (Subscribes is null) Subscribes = new();
            foreach (var item in Masks) item?.FormatConfig();
            foreach (var item in Subscribes) item?.FormatConfig();
            return this;
        }
    }

    public record WordCloudMask : BaseConfig
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public int Width { get; set; }

        public override BaseConfig FormatConfig()
        {
            if (Width < 200) Width = 200;
            if (Width > 2000) Width = 2000;
            return this;
        }
    }

    public record WordCloudTimer : BaseConfig
    {
        public bool Enable { get; set; }
        public string Name { get; set; }
        public string Cron { get; set; }
        public List<string> Masks { get; set; } = new();
        public List<long> Groups { get; set; } = new();
        public double HourRange { get; set; }
        public string Template { get; set; }
        [YamlIgnore]
        public List<long> PushGroups => Groups?.ToSendableGroups() ?? new();

        public override BaseConfig FormatConfig()
        {
            if (HourRange < 1) HourRange = 1;
            if (Masks is null) Masks = new();
            if (Groups is null) Groups = new();
            return this;
        }
    }



}
