using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Model.Subscribe
{
    public class BLBLDynamicInfo
    {
        public BLBLDynamicItem item { get; set; }
    }

    public class BLBLDynamicItem
    {
        public string rp_id { get; set; }
        public string content { get; set; }
    }

}
