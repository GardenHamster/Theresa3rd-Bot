using System.Collections.Generic;
using System.Linq;
using Theresa3rd_Bot.Type;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Model.Lolisuki
{
    public class LolisukiResult
    {
        public string error { get; set; }

        public List<LolisukiData> data { get; set; }
    }

    public class LolisukiData
    {
        public long pid { get; set; }
        public int p { get; set; }
        public int total { get; set; }
        public string uid { get; set; }
        public string author { get; set; }
        public int level { get; set; }
        public LolisukiTaste taste { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public bool r18 { get; set; }
        public bool gif { get; set; }
        public bool original { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string ext { get; set; }
        public long uploadDate { get; set; }
        public LolisukiUrls urls { get; set; }
        public List<string> tags { get; set; }
        public List<string> extags { get; set; }

        public bool isGif() => gif;

        public string hasBanTag() => tags?.hasBanTags() ?? extags?.hasBanTags();

        public bool IsImproper()
        {
            if (tags != null && tags.IsImproper()) return true;
            if (extags != null && tags.IsImproper()) return true;
            return false;
        }

        public bool isR18()
        {
            if (r18) return true;
            if (tags != null && tags.IsR18()) return true;
            if (extags != null && tags.IsR18()) return true;
            return false;
        }
        
    }

    public class LolisukiUrls
    {
        public string thumb { get; set; }
        public string small { get; set; }
        public string regular { get; set; }
        public string original { get; set; }
    }
}
