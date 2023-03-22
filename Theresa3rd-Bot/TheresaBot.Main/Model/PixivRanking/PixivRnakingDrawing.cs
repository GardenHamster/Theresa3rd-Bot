using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheresaBot.Main.Model.PixivRanking
{
    public class PixivRnakingDrawing
    {
        public PixivRankingDetail RankingDetail { get; set; }
        public SKBitmap OriginBitmap { get; set; }
        public bool IsHorizontal { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }

        public PixivRnakingDrawing(PixivRankingDetail rankingDetail, SKBitmap originBitmap, bool isHorizontal, int row, int column)
        {
            this.RankingDetail = rankingDetail;
            this.OriginBitmap = originBitmap;
            this.IsHorizontal = isHorizontal;
            this.Row = row;
            this.Column = column;
        }

    }
}
