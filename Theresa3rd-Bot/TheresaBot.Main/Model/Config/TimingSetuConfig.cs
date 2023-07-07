using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.Config
{
    public class TimingSetuConfig : BasePluginConfig
    {
        public string LocalPath { get; private set; }
        public bool FromOneDir { get; private set; }
        public string LolisukiLevel { get; private set; }
        public List<TimingSetuTimer> Timers { get; private set; }
    }

    public class TimingSetuTimer
    {
        public bool Enable { get; private set; }
        public string Cron { get; private set; }
        public string Name { get; private set; }
        public List<long> Groups { get; private set; }
        public TimingSetuSourceType Source { get; private set; }
        public bool SendMerge { get; private set; }
        public List<string> Tags { get; private set; }
        public int Quantity { get; private set; }
        public bool AtAll { get; private set; }
        public string TimingMsg { get; private set; }

    }

}
