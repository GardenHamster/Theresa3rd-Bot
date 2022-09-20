using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Model.Pixiv
{
    public class PixivWorkInfoDto
    {
        public bool error { get; set; }
        public string message { get; set; }
        public PixivWorkInfo body { get; set; }
    }

    public class PixivWorkInfo
    {
        public int RelevantCount { get; set; }

        public int bookmarkCount { get; set; }

        public int viewCount { get; set; }

        public int likeCount { get; set; }

        public string illustId { get; set; }

        public string illustTitle { get; set; }

        public string illustComment { get; set; }

        public int userId { get; set; }

        public string userName { get; set; }

        public DateTime createDate { get; set; }

        public PixivUrls urls { get; set; }

        public int pageCount { get; set; }

        public PixivTagDto tags { get; set; }

        public int xRestrict { get; set; }

        public List<string> getTags() => tags?.getTags() ?? new List<string>();

        //xRestrict=1为R18,xRestrict=2为R18G
        public bool isR18() => xRestrict > 0 || getTags().IsR18();

        public bool isGif() => getTags().IsGif();

        public string hasBanTag() => getTags()?.hasBanTags();

        public bool IsImproper() => xRestrict > 1 || getTags().IsImproper();
    }

    public class PixivUrls
    {
        public string original { get; set; }
        public string regular { get; set; }
        public string small { get; set; }
        public string thumb { get; set; }
    }
}
