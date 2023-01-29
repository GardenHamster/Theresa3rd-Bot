using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheresaBot.Main.Model.Config
{
    public class WelcomeConfig : BasePluginConfig
    {
        public string Template { get; set; }

        public List<WelcomeSpecial> Special { get; set; }
    }

    public class WelcomeSpecial
    {
        public long GroupId { get; set; }

        public string Template { get; set; }
    }




}
