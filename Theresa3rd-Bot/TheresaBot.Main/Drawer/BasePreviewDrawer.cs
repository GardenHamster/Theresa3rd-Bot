using SkiaSharp;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Drawing;

namespace TheresaBot.Main.Drawer
{
    internal abstract class BasePreviewDrawer : BaseDrawer
    {
        protected const int CellMargin = 40;
        protected const int HeaderMargin = 15;
        protected const int HeaderFontSize = 50;
        protected const int DateTimeMargin = 20;
        protected const int DateTimeFontSize = 25;
        protected const int ExplainMargin = 20;
        protected const int ExplainFontSize = 25;
        protected const int TitleFontSize = 20;
        protected const int WatermarkMargin = 0;
        protected const int WatermarkFontSize = 25;
        protected const int MaxColumn = 5;
        protected const int IllustWidth = 280;
        protected const int IllustHeight = 400;
        protected const int CellWidth = IllustWidth;
        protected const int CellHeight = IllustHeight + TitleFontSize;
        protected SKPaint HeaderPaint;
        protected SKPaint DateTimePaint;
        protected SKPaint ExplainPaint;
        protected SKPaint TitlePaint;
        protected SKPaint WatermarkPaint;

        public BasePreviewDrawer()
        {
            HeaderPaint = getDefaultPaint(HeaderFontSize);
            DateTimePaint = getDefaultPaint(DateTimeFontSize);
            ExplainPaint = getDefaultPaint(ExplainFontSize);
            TitlePaint = getDefaultPaint(TitleFontSize);
            WatermarkPaint = getDefaultPaint(WatermarkFontSize);
        }

        protected SKPaint getDefaultPaint(int fontSize)
        {
            return new SKPaint
            {
                FakeBoldText = true,
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                TextAlign = SKTextAlign.Left,
                TextSize = fontSize,
                Typeface = DefaultTypeface
            };
        }

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
