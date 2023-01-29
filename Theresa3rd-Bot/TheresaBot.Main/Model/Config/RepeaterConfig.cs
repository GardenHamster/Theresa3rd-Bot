using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheresaBot.Main.Model.Config
{
    public class RepeaterConfig : BasePluginConfig
    {
        public int RepeatTime { get; set; }

        public int RepeatMode { get; set; }
        public RepeaterConfig()
        {
            this.RepeatTime = 3;
        }

    }
}
