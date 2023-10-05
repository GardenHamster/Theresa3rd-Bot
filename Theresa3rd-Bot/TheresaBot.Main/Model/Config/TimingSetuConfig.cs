using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.Config
{
    public record TimingSetuConfig : BasePluginConfig
    {
        public string LocalPath { get; set; }
        public bool FromOneDir { get; set; }
        public string LolisukiLevel { get; set; }
        public List<TimingSetuTimer> Timers { get; set; }

        public override TimingSetuConfig FormatConfig()
        {
            return this;
        }
    }

    public record TimingSetuTimer
    {
        public bool Enable { get; set; }
        public string Cron { get; set; }
        public string Name { get; set; }
        public List<long> Groups { get; set; }
        public TimingSetuSourceType Source { get; set; }
        public bool SendMerge { get; set; }
        public List<string> Tags { get; set; }
        public int Quantity { get; set; }
        public bool AtAll { get; set; }
        public string TimingMsg { get; set; }

    }

}
