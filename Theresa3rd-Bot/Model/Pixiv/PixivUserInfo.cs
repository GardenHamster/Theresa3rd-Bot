using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Model.Pixiv
{
    public class PixivUserInfo
    {
        public Dictionary<string, PixivUserWorkInfo> illusts { get; set; }
        public PixivUserExtraData extraData { get; set; }
    }

    public class PixivUserWorkInfo
    {
        public string id { get; set; }
        public string title { get; set; }
        public int userId { get; set; }
        public string userName { get; set; }
        public int pageCount { get; set; }
        public List<string> tags { get; set; }
        public DateTime createDate { get; set; }
        public int xRestrict { get; set; }
        public int aiType { get; set; }
        public bool IsImproper() => xRestrict > 1 || (tags != null && tags.IsImproper());
        public bool isR18() => xRestrict > 0 || (tags != null && tags.IsR18());
        public bool isAI() => aiType > 1 || (tags != null && tags.IsAI());
        public string hasBanTag() => tags?.hasBanTags();
    }

    public class PixivUserExtraData
    {
        public PixivUserMeta meta { get; set; }
    }

    public class PixivUserMeta
    {
        public string title { get; set; }
    }

}
