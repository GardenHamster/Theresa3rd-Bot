using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Model.Pixiv
{
    public class PixivTagDto
    {
        public List<PixivTagModel> tags { get; set; }
    }

    public class PixivTagModel
    {
        public string tag { get; set; }
        public PixivTagTranslation translation { get; set; }
    }

    public class PixivTagTranslation
    {
        public string en { get; set; }
    }


}
