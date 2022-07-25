using System.Collections.Generic;
using System.Linq;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Model.Lolicon
{
    public class LoliconResultV2
    {
        public string error { get; set; }

        public List<LoliconDataV2> data { get; set; }
    }

    public class LoliconDataV2
    {
        public long pid { get; set; }
        public int p { get; set; }
        public string uid { get; set; }
        public string title { get; set; }
        public string author { get; set; }
        public bool r18 { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public List<string> tags { get; set; }
        public string ext { get; set; }
        public long uploadDate { get; set; }
        public LoliconUrlsV2 urls { get; set; }

        public bool IsImproper()
        {
            return tags == null ? false : tags.IsImproper();
        }

        public bool isR18()
        {
            return tags == null ? false : tags.IsR18();
        }
    }

    public class LoliconUrlsV2
    {
        public string original { get; set; }
    }
}
