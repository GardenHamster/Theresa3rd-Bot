using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Pixiv;
using Ubiety.Dns.Core;

namespace TheresaBot.Main.Model.PixivRanking
{
    public class PixivRankingDetail
    {
        public PixivRankingContent RankingContent { get; set; }

        public PixivWorkInfo WorkInfo { get; set; }

        public string ImageSavePath { get; set; }

        public PixivRankingDetail(PixivRankingContent rankingContent, PixivWorkInfo workInfo, string imageSavePath)
        {
            this.RankingContent = rankingContent;
            this.WorkInfo = workInfo;
            this.ImageSavePath = imageSavePath;
        }

    }
}
