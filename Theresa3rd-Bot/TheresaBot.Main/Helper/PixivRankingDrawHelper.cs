using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheresaBot.Main.Model.PixivRanking;

namespace TheresaBot.Main.Helper
{
    public static class PixivRankingDrawHelper
    {
        private const int margin = 30;
        private const int fontSize = 22;
        private const int maxColumn = 5;
        private const int maxWidth = 280;
        private const int maxHeight = 400;
        private const int areaWidth = maxWidth;
        private const int areaHeight = maxHeight + fontSize;

        public static FileInfo DrawPreview(List<PixivRankingPreview> datas, string savePath)
        {
            int imgNum = datas.Count;
            int maxRow = MathHelper.getMaxPage(imgNum, maxColumn);
            int canvasWidth = maxColumn * areaWidth + (maxColumn + 1) * margin;
            int canvasHeight = maxRow * areaHeight + (maxRow + 1) * margin;

            var imgInfo = new SKImageInfo(canvasWidth, canvasHeight);
            using SKSurface surface = SKSurface.Create(imgInfo);
            SKCanvas canvas = surface.Canvas;
            canvas.Clear(SKColors.LightGray);

            for (int i = 0; i < datas.Count; i++)
            {
                int row = 1 + (i / maxColumn);
                int column = 1 + (i % maxColumn);
                int areaX = margin * column + areaWidth * (column - 1);
                int areaY = margin * row + areaHeight * (row - 1);
                FileInfo imgFile = datas[i].Image;
                DrawRankAndPidText(canvas, datas[i].Content, areaX, areaY);
                if (imgFile is not null) DrawImage(canvas, imgFile, areaX, areaY);
            }

            using SKImage image = surface.Snapshot();
            using SKData data = image.Encode(SKEncodedImageFormat.Jpeg, 100);
            using FileStream outputStream = File.OpenWrite(savePath);
            data.SaveTo(outputStream);
            return new FileInfo(savePath);
        }

        private static void DrawRankAndPidText(SKCanvas canvas, PixivRankingContent content, int areaX, int areaY)
        {
            int x = areaX;
            int y = areaY;
            var paint = new SKPaint
            {
                FakeBoldText = true,
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                TextAlign = SKTextAlign.Left,
                TextSize = fontSize,
                Typeface = SKTypeface.FromFamilyName("SimSun")
            };
            canvas.DrawText($"#{content.rank}  PID：{content.illust_id}", new SKPoint(x, y), paint);
        }

        private static void DrawImage(SKCanvas canvas, FileInfo imgFile, int areaX, int areaY)
        {
            int x = areaX;
            int y = areaY + fontSize;
            using FileStream fileStream = File.OpenRead(imgFile.FullName);
            using SKBitmap originBitmap = SKBitmap.Decode(fileStream);
            double widthScale = Convert.ToDouble(maxWidth) / originBitmap.Width;
            double heightScale = Convert.ToDouble(maxHeight) / originBitmap.Height;
            double scale = Math.Min(widthScale, heightScale);
            int imgWidth = (int)(originBitmap.Width * scale);
            int imgHeight = (int)(originBitmap.Height * scale);
            using SKBitmap resizeBitmap = originBitmap.Resize(new SKImageInfo(imgWidth, imgHeight), SKFilterQuality.Low);
            canvas.DrawBitmap(resizeBitmap, new SKPoint(x, y));
        }

    }
}
