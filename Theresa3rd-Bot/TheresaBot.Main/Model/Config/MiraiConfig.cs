using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheresaBot.Main.Model.Config
{
    public class MiraiConfig
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string AuthKey { get; set; }

        public long BotQQ { get; set; }
    }
}
