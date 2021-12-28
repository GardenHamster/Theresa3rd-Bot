using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Model.Config
{
    public class GeneralConfig
    {
        public string Prefix { get; set; }

        public bool ReceiveAt { get; set; }

        public List<long> AcceptGroups { get; set; }

        public List<long> RefuseGroups { get; set; }
    }
}
