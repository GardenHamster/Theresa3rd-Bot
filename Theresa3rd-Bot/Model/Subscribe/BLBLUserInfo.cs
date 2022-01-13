using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Model.Subscribe
{
    public class BLBLUserInfo
    {
        public int code { get; set; }
        public BLBLUserInfoData data { get; set; }
    }

    public class BLBLUserInfoData
    {
        public string name { get; set; }

        public string sign { get; set; }

        public int level { get; set; }

        public int mid { get; set; }

        public int rank { get; set; }

        public BLBLUserLiveRoom live_room { get; set; }
    }

    public class BLBLUserLiveRoom
    {
        public string cover { get; set; }

        public int liveStatus { get; set; }

        public int roomid { get; set; }

        public string title { get; set; }

        public int online { get; set; }

        public string url { get; set; }
    }






}
