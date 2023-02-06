using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheresaBot.Main.Helper;
using Ubiety.Dns.Core;

namespace TheresaBot.Main.Model.PixivRanking
{
    public class PixivRankingPreview
    {
        public PixivRankingContent Content { get; set; }

        public FileInfo Image { get; set; }

        public PixivRankingPreview(PixivRankingContent content, FileInfo image)
        {
            this.Content = content;
            this.Image = image;
        }


    }
}
