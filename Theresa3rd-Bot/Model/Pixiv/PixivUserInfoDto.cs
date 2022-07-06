using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Model.Pixiv
{
    public class PixivUserInfoDto
    {
        public bool error { get; set; }
        public string message { get; set; }
        public PixivUserInfo body { get; set; }
    }

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
        public bool IsImproper()
        {
            return tags != null && tags.IsImproper();
        }
        public bool isR18()
        {
            return tags != null && tags.IsR18();
        }
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
