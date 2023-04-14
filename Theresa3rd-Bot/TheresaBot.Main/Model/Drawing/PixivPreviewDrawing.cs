using SkiaSharp;

namespace TheresaBot.Main.Model.Drawing
{
    public class PixivPreviewDrawing<T> where T : BasePixivDrawing
    {
        public T Detail { get; set; }
        public SKBitmap OriginBitmap { get; set; }
        public bool IsHorizontal { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }

        public PixivPreviewDrawing(T detail, SKBitmap originBitmap, bool isHorizontal, int row, int column)
        {
            Detail = detail;
            OriginBitmap = originBitmap;
            IsHorizontal = isHorizontal;
            Row = row;
            Column = column;
        }



    }
}
