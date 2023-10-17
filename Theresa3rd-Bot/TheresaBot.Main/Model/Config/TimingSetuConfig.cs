using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.Config
{
    public record TimingSetuConfig : BasePluginConfig
    {
        public string LocalPath { get; set; }
        public bool FromOneDir { get; set; }
        public string LolisukiLevel { get; set; }
        public List<TimingSetuTimer> Timers { get; set; } = new();

        public override TimingSetuConfig FormatConfig()
        {
            if (Timers is null) Timers = new();
            foreach (var timer in Timers) timer?.FormatConfig();
            return this;
        }
    }

    public record TimingSetuTimer : BaseConfig
    {
        public bool Enable { get; set; }
        public string Cron { get; set; }
        public string Name { get; set; }
        public List<long> Groups { get; set; } = new();
        public TimingSetuSourceType Source { get; set; } = TimingSetuSourceType.Lolicon;
        public bool SendMerge { get; set; }
        public List<string> Tags { get; set; } = new();
        public int Quantity { get; set; } = 5;
        public bool AtAll { get; set; }
        public string TimingMsg { get; set; }

        public override BaseConfig FormatConfig()
        {
            if (Quantity < 1) Quantity = 1;
            if (Quantity > 100) Quantity = 100;
            if (Groups is null) Groups = new();
            if (Tags is null) Tags = new();
            return this;
        }
    }

}
