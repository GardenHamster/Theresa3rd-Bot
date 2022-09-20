using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Model.Config
{
    public class RepeaterConfig
    {
        public bool Enable { get; set; }

        public int RepeatTime { get; set; }

        public int RepeatMode { get; set; }
        public RepeaterConfig()
        {
            this.RepeatTime = 3;
        }

    }
}
