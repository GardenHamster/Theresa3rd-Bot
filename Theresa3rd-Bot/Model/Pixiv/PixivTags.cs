using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Model.Pixiv
{
    public class PixivTags
    {
        public List<PixivTagModel> tags { get; set; }

        public List<string> getTags()
        {
            List<string> tagList = new List<string>();
            if (tags is null) return tagList;
            tagList.AddRange(tags.Select(o => o.tag).Where(o => o != null).ToList());
            tagList.AddRange(tags.Select(o => o.translation?.en).Where(o => o != null).ToList());
            tagList.AddRange(tags.Select(o => o.translation?.ko).Where(o => o != null).ToList());
            tagList.AddRange(tags.Select(o => o.translation?.zh).Where(o => o != null).ToList());
            tagList.AddRange(tags.Select(o => o.translation?.zh_tw).Where(o => o != null).ToList());
            return tagList.Where(o => string.IsNullOrWhiteSpace(o) == false).ToList();
        }
    }

    public class PixivTagModel
    {
        public string tag { get; set; }
        public PixivTagTranslation translation { get; set; }
    }

    public class PixivTagTranslation
    {
        public string en { get; set; }
        public string ko { get; set; }
        public string zh { get; set; }
        public string zh_tw { get; set; }
    }


}
