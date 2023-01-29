using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheresaBot.Main.Model.Pixiv
{
    public class PixivSearchTask
    {
        public int Page { get; set; }

        public int StartIndex { get; set; }

        public int EndIndex { get; set; }

        public PixivSearchTask(int page, int startIndex, int endIndex)
        {
            this.Page = page;
            this.StartIndex = startIndex;
            this.EndIndex = endIndex;
        }

    }
}
