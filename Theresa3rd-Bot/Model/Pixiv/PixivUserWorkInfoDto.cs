using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Model.Pixiv
{
    public class PixivUserWorkInfoDto
    {
        public bool error { get; set; }
        public string message { get; set; }
        public PixivUserWorkBody body { get; set; }
    }

    public class PixivUserWorkBody
    {
        public Dictionary<string, PixivUserWorkInfo> illusts { get; set; }
    }

    public class PixivUserWorkInfo
    {
        public string id { get; set; }

        public string title { get; set; }

        public int userId { get; set; }

        public string userName { get; set; }

        public int pageCount { get; set; }

        public List<string> tags { get; set; }

        public bool isR18()
        {
            return tags.Where(o => o.ToUpper() == "R-18" || o.ToUpper() == "R18" || o.ToUpper() == "R18+" || o.ToUpper() == "18+" || o.ToUpper() == "R17.9").FirstOrDefault() != null;
        }

    }

}
