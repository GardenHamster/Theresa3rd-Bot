using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theresa3rd_Bot.Model.PO;
using Theresa3rd_Bot.Type;

namespace Theresa3rd_Bot.Model.Subscribe
{
    public class BLBLSubscribe
    {
        public SubscribeRecordPO SubscribeRecord { get; set; }

        public FileInfo CoverFileInfo { get; set; }

        public FileInfo PreviewFileInfo { get; set; }

        public SubscribeDynamicType SubscribeDynamicType { get; set; }

        public string DynamicInfo { get; set; }
    }
}
