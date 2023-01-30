using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.Config
{
    public class TimingSetuConfig : BasePluginConfig
    {
        public List<TimingSetuTimer> Timers { get; set; }
    }

    public class TimingSetuTimer
    {
        public string Cron { get; set; }
        public string Name { get; set; }
        public TimingSetuSourceType Source { get; set; }
        public string LocalPath { get; set; }
        public bool FromOneDir { get; set; }
        public List<string> Tags { get; set; }
        public string Level { get; set; }
        public int Quantity { get; set; }
        public List<long> Groups { get; set; }
        public bool AtAll { get; set; }
        public string TimingMsg { get; set; }
        public string LocalTemplate { get; set; }
    }

}
