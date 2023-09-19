using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheresaBot.Main.Model.Config
{
    public class BackstageConfig : BaseConfig
    {
        public string Password { get; set; }

        public string SecretKey { get; set; }

        public override BackstageConfig FormatConfig()
        {
            return this;
        }

    }
}
