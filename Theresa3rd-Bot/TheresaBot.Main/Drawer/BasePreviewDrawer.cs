using SkiaSharp;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Drawing;

namespace TheresaBot.Main.Drawer
{
    internal abstract class BasePreviewDrawer : BaseDrawer
    {
        protected virtual int CellMargin => 40;
        protected virtual int HeaderMargin => 15;
        protected virtual int HeaderFontSize => 50;
        protected virtual int DateTimeMargin => 15;
        protected virtual int DateTimeFontSize => 40;
        protected virtual int ExplainMargin => 15;
        protected virtual int ExplainFontSize => 25;
        protected virtual int TitleFontSize => 20;
        protected virtual int WatermarkMargin => 0;
        protected virtual int WatermarkFontSize => 25;
        protected virtual int MaxColumn => 5;
        protected virtual int IllustWidth => 280;
        protected virtual int IllustHeight => 400;
        protected virtual int CellWidth => IllustWidth;
        protected virtual int CellHeight => IllustHeight + TitleFontSize;
        protected virtual SKPaint HeaderPaint => getDefaultPaint(HeaderFontSize);
        protected virtual SKPaint DateTimePaint => getDefaultPaint(DateTimeFontSize);
        protected virtual SKPaint ExplainPaint => getDefaultPaint(ExplainFontSize);
        protected virtual SKPaint TitlePaint => getDefaultPaint(TitleFontSize);
        protected virtual SKPaint WatermarkPaint => getDefaultPaint(WatermarkFontSize);

        protected async Task<List<PixivPreviewDrawing<T>>> ArrangeDrawingAsync<T>(List<T> details) where T : BasePixivDrawing
        {
            int row = 1, column = 1;
            var arrageList = new List<PixivPreviewDrawing<T>>();
            for (int i = 0; i < details.Count; i++)
            {
                bool isHorizontal = false;
                SKBitmap originBitmap = null;
                var detail = details[i];
                var imgFile = await GetDrawImgAsync(detail.ImageHttpUrl, detail.ImageSavePath, detail.PixivId);
                if (imgFile is not null)
                {
                    using FileStream fileStream = File.OpenRead(imgFile.FullName);
                    originBitmap = SKBitmap.Decode(fileStream);
                    isHorizontal = IsHorizontal(originBitmap);
                }
                int colSpan = isHorizontal ? 2 : 1;
                if (column + colSpan > MaxColumn)
                {
                    column = 1;
                    row++;
                }
                else
                {
                    if (i > 0) column++;
                }
                arrageList.Add(new PixivPreviewDrawing<T>(detail, originBitmap, isHorizontal, row, column));
                if (colSpan > 1) column = column + colSpan - 1;
            }
            return arrageList;
        }

        private async Task<FileInfo> GetDrawImgAsync(string imgHttpUrl, string imgSavePath, string pixivId)
        {
            try
            {
                if (File.Exists(imgSavePath)) return new FileInfo(imgSavePath);
                string fullFileName = imgHttpUrl.GetPreviewImgSaveName(pixivId);
                FileInfo imgFile = await PixivHelper.DownPixivImgAsync(imgHttpUrl, pixivId, fullFileName);
                if (imgFile is not null) return imgFile;
                return FilePath.GetDownErrorImg();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return null;
            }
        }



    }
}
