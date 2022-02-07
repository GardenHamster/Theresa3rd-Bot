using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theresa3rd_Bot.Type;

namespace Theresa3rd_Bot.Model.Subscribe
{
    public class SubscribeInfo
    {
        public int SubscribeId { get; set; }
        public string SubscribeCode { get; set; }
        public SubscribeType SubscribeType { get; set; }
        public MysSectionType SubscribeSubType { get; set; }
        public string SubscribeName { get; set; }
        public string SubscribeDescription { get; set; }
        public long GroupId { get; set; }
    }

}
