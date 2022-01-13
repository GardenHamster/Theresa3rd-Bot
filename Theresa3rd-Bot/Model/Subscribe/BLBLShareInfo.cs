using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Model.Subscribe
{
    public class BLBLShareInfo
    {
        public BLBLShareItem item { get; set; }
        public string origin { get; set; }
    }

    public class BLBLShareItem
    {
        public string rp_id { get; set; }
        public string content { get; set; }
    }

}
