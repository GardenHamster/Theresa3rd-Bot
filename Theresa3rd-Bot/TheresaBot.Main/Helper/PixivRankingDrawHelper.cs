using SkiaSharp;
using TheresaBot.Main.Common;
using TheresaBot.Main.Model.Cache;
using TheresaBot.Main.Model.PixivRanking;

namespace TheresaBot.Main.Helper
{
    public static class PixivRankingDrawHelper
    {
        private const int AreaMargin = 30;
        private const int HeaderMargin = 15;
        private const int HeaderFontSize = 50;
        private const int DetailFontSize = 18;
        private const int RemarkMargin = 10;
        private const int RemarkFontSize = 25;
        private const int WatermarkMargin = 5;
        private const int WatermarkFontSize = 30;
        private const int MaxColumn = 5;
        private const int IllustWidth = 280;
        private const int IllustHeight = 400;
        private const int AreaWidth = IllustWidth;
        private const int AreaHeight = IllustHeight + DetailFontSize;

        public static FileInfo DrawPreview(PixivRankingInfo rankingInfo, List<PixivRankingDetail> datas, string savePath)
        {
            int row = 1;
            int column = 1;
            int areaX = 0;
            int areaY = 0;
            int startX = 0;
            int startY = 0;
            int imgNum = datas.Count;
            int maxRow = MathHelper.getMaxPage(imgNum, MaxColumn);

            int headAreaHeight = HeaderMargin + HeaderFontSize;
            int remarkAreaHeight = RemarkMargin + RemarkFontSize;
            int workAreaWidth = MaxColumn * AreaWidth + (MaxColumn + 1) * AreaMargin;
            int workAreaHeight = headAreaHeight + maxRow * AreaHeight + (maxRow + 1) * AreaMargin;
            int watermarkAreaHeight = WatermarkMargin + WatermarkFontSize;

            int canvasWidth = workAreaWidth;
            int canvasHeight = headAreaHeight + remarkAreaHeight + workAreaHeight + watermarkAreaHeight;

            var imgInfo = new SKImageInfo(canvasWidth, canvasHeight);
            using SKSurface surface = SKSurface.Create(imgInfo);
            SKCanvas canvas = surface.Canvas;
            canvas.Clear(SKColors.LightGray);

            DrawHeader(canvas, rankingInfo, AreaMargin, areaY);
            areaY += headAreaHeight;

            DrawRemark(canvas, rankingInfo, AreaMargin, areaY);
            areaY += remarkAreaHeight;

            for (int i = 0; i < datas.Count; i++)
            {
                row = 1 + (i / MaxColumn);
                column = 1 + (i % MaxColumn);
                startX = areaX + AreaMargin * column + AreaWidth * (column - 1);
                startY = areaY + AreaMargin * row + AreaHeight * (row - 1);
                DrawDetails(canvas, datas[i], startX, startY);
                DrawImage(canvas, datas[i], startX, startY);
            }
            areaY += workAreaHeight;

            DrawWatermark(canvas, AreaMargin, areaY);
            areaY += watermarkAreaHeight;

            using SKImage image = surface.Snapshot();
            using SKData data = image.Encode(SKEncodedImageFormat.Jpeg, 100);
            using FileStream outputStream = File.OpenWrite(savePath);
            data.SaveTo(outputStream);
            return new FileInfo(savePath);
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
            string headerText = $"图片信息：#排名 PID 点赞率%/收藏率%";
            canvas.DrawText(headerText, new SKPoint(x, y), paint);
        }

        private static void DrawDetails(SKCanvas canvas, PixivRankingDetail detail, int startX, int startY)
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
                TextSize = DetailFontSize,
                Typeface = SKTypeface.FromFamilyName("SimSun")
            };
            PixivRankingContent content = detail.RankingContent;
            string likeRate = detail.WorkInfo.likeRate.toPercent();
            string bookRate = detail.WorkInfo.bookmarkRate.toPercent();
            string detailText = $"#{content.rank} {content.illust_id} {likeRate}/{bookRate}";
            canvas.DrawText(detailText, new SKPoint(x, y), paint);
        }

        private static void DrawImage(SKCanvas canvas, PixivRankingDetail detail, int startX, int startY)
        {
            int x = startX;
            int y = startY + DetailFontSize;
            FileInfo imgFile = GetDrawImg(detail).Result;
            if (imgFile is null) imgFile = FilePath.GetDownErrorImg();
            if (imgFile is null) return;
            using FileStream fileStream = File.OpenRead(imgFile.FullName);
            using SKBitmap originBitmap = SKBitmap.Decode(fileStream);
            double widthScale = Convert.ToDouble(IllustWidth) / originBitmap.Width;
            double heightScale = Convert.ToDouble(IllustHeight) / originBitmap.Height;
            double scale = Math.Min(widthScale, heightScale);
            int imgWidth = (int)(originBitmap.Width * scale);
            int imgHeight = (int)(originBitmap.Height * scale);
            using SKBitmap resizeBitmap = originBitmap.Resize(new SKImageInfo(imgWidth, imgHeight), SKFilterQuality.Low);
            canvas.DrawBitmap(resizeBitmap, new SKPoint(x, y));
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
                return await PixivHelper.DownPixivImgAsync(detail.WorkInfo.PixivId, detail.RankingContent.url, fullFileName);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return null;
            }
        }

    }
}
