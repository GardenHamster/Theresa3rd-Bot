using SkiaSharp;
using TheresaBot.Main.Common;
using TheresaBot.Main.Model.Cache;
using TheresaBot.Main.Model.PixivRanking;

namespace TheresaBot.Main.Helper
{
    internal static class PixivRankingDrawHelper
    {
        private const int CellMargin = 30;
        private const int HeaderMargin = 15;
        private const int HeaderFontSize = 50;
        private const int RemarkMargin = 20;
        private const int RemarkFontSize = 25;
        private const int TitleFontSize = 18;
        private const int WatermarkMargin = 0;
        private const int WatermarkFontSize = 30;
        private const int MaxColumn = 5;
        private const int IllustWidth = 280;
        private const int IllustHeight = 400;
        private const int CellWidth = IllustWidth;
        private const int CellHeight = IllustHeight + TitleFontSize;

        public static async Task<FileInfo> DrawPreview(PixivRankingInfo rankingInfo, List<PixivRankingDetail> details, string fullSavePath)
        {
            int row = 1;
            int column = 1;
            int areaX = 0;
            int areaY = 0;
            int startX = 0;
            int startY = 0;
            int imgNum = details.Count;
            int maxRow = MathHelper.getMaxPage(imgNum, MaxColumn);

            int headAreaHeight = HeaderMargin + HeaderFontSize;
            int remarkAreaHeight = RemarkMargin + RemarkFontSize;
            int workAreaWidth = MaxColumn * CellWidth + (MaxColumn + 1) * CellMargin;
            int workAreaHeight = headAreaHeight + maxRow * CellHeight + (maxRow + 1) * CellMargin;
            int watermarkAreaHeight = WatermarkMargin + WatermarkFontSize;

            int canvasWidth = workAreaWidth;
            int canvasHeight = headAreaHeight + remarkAreaHeight + workAreaHeight + watermarkAreaHeight;

            var imgInfo = new SKImageInfo(canvasWidth, canvasHeight);
            using SKSurface surface = SKSurface.Create(imgInfo);
            SKCanvas canvas = surface.Canvas;
            canvas.Clear(SKColors.LightGray);

            DrawHeader(canvas, rankingInfo, CellMargin, areaY);
            areaY += headAreaHeight;

            DrawRemark(canvas, rankingInfo, CellMargin, areaY);
            areaY += remarkAreaHeight;

            for (int i = 0; i < details.Count; i++)
            {
                var detail = details[i];
                bool isHorizontal = false;
                var imgFile = await GetDrawImg(detail);
                if (imgFile is not null)
                {
                    using FileStream fileStream = File.OpenRead(imgFile.FullName);
                    using SKBitmap originBitmap = SKBitmap.Decode(fileStream);
                    isHorizontal = originBitmap.IsHorizontal();
                    startX = areaX + CellMargin * column + CellWidth * (column - 1);
                    startY = areaY + CellMargin * row + CellHeight * (row - 1);
                    DrawImage(canvas, originBitmap, startX, startY, detail.WorkInfo.IsR18, isHorizontal);
                }
                DrawTitle(canvas, detail, startX, startY, isHorizontal);
                column += isHorizontal ? 2 : 1;
                if (column > MaxColumn)
                {
                    column = column % MaxColumn;
                    row++;
                }
            }
            areaY += workAreaHeight;

            DrawWatermark(canvas, CellMargin, areaY);
            areaY += watermarkAreaHeight;

            using SKImage image = surface.Snapshot();
            using SKData data = image.Encode(SKEncodedImageFormat.Jpeg, 100);
            using FileStream outputStream = File.OpenWrite(fullSavePath);
            data.SaveTo(outputStream);
            return new FileInfo(fullSavePath);
        }

        private static bool IsHorizontal(this SKBitmap originBitmap)
        {
            return Convert.ToDouble(originBitmap.Width) / originBitmap.Height > 1.2;
        }

        private static void DrawHeader(SKCanvas canvas, PixivRankingInfo rankingInfo, int startX, int startY)
        {
            int x = startX;
            int y = startY + HeaderMargin + HeaderFontSize;
            var paint = new SKPaint
            {
                FakeBoldText = true,
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                TextAlign = SKTextAlign.Left,
                TextSize = HeaderFontSize,
                Typeface = SKTypeface.FromFamilyName("SimSun")
            };
            string headerText = $"{rankingInfo.RankingDate} {rankingInfo.RankingMode.Name}";
            canvas.DrawText(headerText, new SKPoint(x, y), paint);
        }

        private static void DrawRemark(SKCanvas canvas, PixivRankingInfo rankingInfo, int startX, int startY)
        {
            int x = startX;
            int y = startY + RemarkMargin + RemarkFontSize;
            var paint = new SKPaint
            {
                FakeBoldText = true,
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                TextAlign = SKTextAlign.Left,
                TextSize = RemarkFontSize,
                Typeface = SKTypeface.FromFamilyName("SimSun")
            };
            string headerText = $"#排名 PID 点赞率%/收藏率%";
            canvas.DrawText(headerText, new SKPoint(x, y), paint);
        }

        private static void DrawTitle(SKCanvas canvas, PixivRankingDetail detail, int startX, int startY, bool isHorizontal)
        {
            int x = startX;
            int y = startY;
            var paint = new SKPaint
            {
                FakeBoldText = true,
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                TextAlign = SKTextAlign.Left,
                TextSize = TitleFontSize,
                Typeface = SKTypeface.FromFamilyName("SimSun")
            };
            PixivRankingContent content = detail.RankingContent;
            string likeRate = detail.WorkInfo.likeRate.toPercent();
            string bookRate = detail.WorkInfo.bookmarkRate.toPercent();
            string detailText = $"#{content.rank} {content.illust_id} {likeRate}/{bookRate}";
            canvas.DrawText(detailText, new SKPoint(x, y), paint);
        }

        private static void DrawImage(SKCanvas canvas, SKBitmap originBitmap, int startX, int startY, bool isR18, bool isHorizontal)
        {
            int x = startX;
            int y = startY + TitleFontSize / 2;
            int drawWidth = isHorizontal ? CellWidth + CellMargin + CellWidth : IllustWidth;
            int drawHeight = IllustHeight;
            double scaleX = Convert.ToDouble(drawWidth) / originBitmap.Width;
            double scaleY = Convert.ToDouble(drawHeight) / originBitmap.Height;
            double scale = Math.Max(scaleX, scaleY);
            int imgWidth = (int)(originBitmap.Width * scale);
            int imgHeight = (int)(originBitmap.Height * scale);
            var imgInfo = new SKImageInfo(imgWidth, imgHeight);
            using SKBitmap resizeBitmap = originBitmap.Resize(imgInfo, SKFilterQuality.Low);
            using SKBitmap drawBitmap = isR18 ? resizeBitmap.Blur() : resizeBitmap;
            var source = new SKRect(0, 0, drawWidth, drawHeight);
            var dest = new SKRect(x, y, x + drawWidth, y + drawHeight);
            canvas.DrawBitmap(drawBitmap, source, dest);
        }

        private static void DrawWatermark(SKCanvas canvas, int startX, int startY)
        {
            int x = startX;
            int y = startY + WatermarkMargin;
            var paint = new SKPaint
            {
                FakeBoldText = true,
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                TextAlign = SKTextAlign.Left,
                TextSize = WatermarkFontSize,
                Typeface = SKTypeface.FromFamilyName("SimSun")
            };
            string watermarkText = $"Create by Theresa-Bot {BotConfig.BotVersion} Doc {BotConfig.BotHomepage}";
            canvas.DrawText(watermarkText, new SKPoint(x, y), paint);
        }

        private static async Task<FileInfo> GetDrawImg(PixivRankingDetail detail)
        {
            try
            {
                string imgSavePath = detail.ImageSavePath;
                if (File.Exists(imgSavePath)) return new FileInfo(imgSavePath);
                string fullFileName = detail.RankingContent.url.GetPreviewImgSaveName(detail.WorkInfo.illustId);
                FileInfo imgFile = await PixivHelper.DownPixivImgAsync(detail.WorkInfo.PixivId, detail.RankingContent.url, fullFileName);
                if (imgFile is not null) return imgFile;
                return FilePath.GetDownErrorImg();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return null;
            }
        }

        private static SKBitmap Blur(this SKBitmap bitmap)
        {
            using SKImage blurImage = bitmap.Blur(BotConfig.PixivConfig.R18ImgBlur);
            return SKBitmap.FromImage(blurImage);
        }

    }
}
