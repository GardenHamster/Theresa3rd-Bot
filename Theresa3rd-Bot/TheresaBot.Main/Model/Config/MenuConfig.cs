using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheresaBot.Main.Model.Config
{
    public class MenuConfig : BasePluginConfig
    {
        public List<string> Commands { get; set; }

        public string Template { get; set; }

    }
}
