using System.Collections.Generic;

namespace Theresa3rd_Bot.Model.Config
{
    public class TimingSetuConfig
    {
        public bool Enable { get; set; }
        public List<TimingSetuTimers> Timers { get; set; }
    }

    public class TimingSetuTimers
    {
        public string Cron { get; set; }
        public string Name { get; set; }
        public int Source { get; set; }
        public string LocalPath { get; set; }
        public List<string> Tags { get; set; }
        public string Level { get; set; }
        public int Quantity { get; set; }
        public List<long> Groups { get; set; }
        public bool AtAll { get; set; }
        public string Template { get; set; }
    }

}
