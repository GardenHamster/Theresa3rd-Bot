using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Model.Subscribe
{
    public class BLBLArticleInfo
    {
        public BLBLArticleItem item { get; set; }
    }

    public class BLBLArticleItem
    {
        public string id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public int pictures_count { get; set; }
        public List<BLBLArticlePixture> pictures { get; set; }
    }

    public class BLBLArticlePixture
    {
        public string img_src { get; set; }
    }

}
