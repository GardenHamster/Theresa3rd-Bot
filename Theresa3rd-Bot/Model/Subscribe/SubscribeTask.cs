using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theresa3rd_Bot.Type;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Model.Subscribe
{
    public class SubscribeTask
    {
        public SubscribeInfo SubscribeInfo { get; set; }
        public List<long> GroupIdList { get; set; }

        public SubscribeTask(SubscribeInfo subscribeInfo)
        {
            this.SubscribeInfo = subscribeInfo;
            this.GroupIdList = new List<long>();
        }


    }
}
