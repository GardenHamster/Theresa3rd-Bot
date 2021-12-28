using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Model.Config
{
    public class ReminderConfig : BaseConfig
    {
        public List<ReminderTimer> Timers { get; set; }
    }

    public class ReminderTimer
    {
        public string Cron { get; set; }

        public bool AtAll { get; set; }

        public List<long> AtMembers { get; set; }

        public string Template { get; set; }
    }


}
