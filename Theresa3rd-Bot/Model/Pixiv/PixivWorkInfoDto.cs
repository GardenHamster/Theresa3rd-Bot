using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public bool isR18()
        {
            return tags.tags.Where(o => o.tag.ToUpper() == "R-18" || o.tag.ToUpper() == "R18").Any();
        }

        public bool isGif()
        {
            return tags.tags.Where(o => o.tag == "うごイラ" || o.tag == "动图").Any();
        }

        public bool hasBanTag()
        {
            //return tags.tags.Where(o => Setting.Pixiv.BanTag.Contains(o.tag.ToUpper())).FirstOrDefault() != null;
            return false;
        }

    }

    public class PixivUrls
    {
        public string original { get; set; }
        public string regular { get; set; }
    }
}
