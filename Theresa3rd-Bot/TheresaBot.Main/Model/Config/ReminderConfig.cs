using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheresaBot.Main.Model.Config
{
    public class ReminderConfig : BasePluginConfig
    {
        public List<ReminderTimer> Timers { get; set; }
    }

    public class ReminderTimer
    {
        public string Name { get; set; }

        public string Cron { get; set; }

        public List<long> Groups { get; set; }
        
        public bool AtAll { get; set; }

        public List<long> AtMembers { get; set; }

        public string Template { get; set; }
    }


}
