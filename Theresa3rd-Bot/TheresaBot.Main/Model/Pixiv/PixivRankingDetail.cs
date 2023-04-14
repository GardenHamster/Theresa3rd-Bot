using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Drawing;
using Ubiety.Dns.Core;

namespace TheresaBot.Main.Model.Pixiv
{
    public class PixivRankingDetail
    {
        public PixivRankingContent RankingContent { get; set; }
        public PixivWorkInfo WorkInfo { get; set; }

        public PixivRankingDetail(PixivRankingContent rankingContent, PixivWorkInfo workInfo)
        {
            RankingContent = rankingContent;
            WorkInfo = workInfo;
        }

    }
}
