using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Model.Pixiv
{
    public class PixivUgoiraMeta
    {
        public string mime_type { get; set; }

        public string originalSrc { get; set; }

        public string src { get; set; }

        public List<PixivUgoiraMetaFrames> frames { get; set; }
    }

    public class PixivUgoiraMetaFrames
    {
        public int delay { get; set; }

        public string file { get; set; }
    }









}
