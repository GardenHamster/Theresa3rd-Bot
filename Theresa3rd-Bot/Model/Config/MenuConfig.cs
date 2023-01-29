using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Model.Config
{
    public class MenuConfig : BasePluginConfig
    {
        public List<string> Commands { get; set; }

        public string Template { get; set; }

    }
}
