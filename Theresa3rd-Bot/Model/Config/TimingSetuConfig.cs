using System.Collections.Generic;
using Theresa3rd_Bot.Type;

namespace Theresa3rd_Bot.Model.Config
{
    public class TimingSetuConfig
    {
        public bool Enable { get; set; }
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
