using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Model.Config
{
    public class GeneralConfig
    {
        public string Prefix { get; set; }

        public List<long> ErrorGroups { get; set; }

        public string ErrorMsg { get; set; }
    }
}
