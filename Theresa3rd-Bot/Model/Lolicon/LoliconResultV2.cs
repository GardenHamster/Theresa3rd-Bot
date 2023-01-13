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

        //xRestrict=1为R18,xRestrict=2为R18G
        public bool isR18() => tags != null && tags.IsR18();
        public bool IsImproper() => tags != null && tags.IsImproper();
        public bool isGif() => tags != null && tags.IsGif();
        public string hasBanTag() => tags?.hasBanTags();
    }

    public class LoliconUrlsV2
    {
        public string original { get; set; }
    }
}
