using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Model.Subscribe
{
    public class SubscribeInfo
    {
        public int SubscribeId { get; set; }
        public string SubscribeCode { get; set; }
        public int SubscribeType { get; set; }
        public int SubscribeSubType { get; set; }
        public string SubscribeName { get; set; }
        public string SubscribeDescription { get; set; }
        public long GroupId { get; set; }
    }

}
