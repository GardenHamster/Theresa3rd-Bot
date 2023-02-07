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
        private const int margin = 20;
        private const int fontSize = 16;
        private const int lineHeight = 20;
        private const int maxColumn = 4;
        private const int maxWidth = 280;
        private const int maxHeight = 400;
        private const int areaWidth = maxWidth;
        private const int areaHeight = maxHeight + lineHeight * 2;

        public static FileInfo DrawPreview(List<PixivRankingPreview> datas, string savePath)
        {
            int imgNum = datas.Count;
            int maxRow = MathHelper.getMaxPage(imgNum, maxColumn);
            int canvasWidth = maxColumn * areaWidth + (maxColumn + 1) * margin;
            int canvasHeight = maxRow * areaHeight + (maxRow + 1) * margin;

            var imgInfo = new SKImageInfo(canvasWidth, canvasHeight);
            using SKSurface surface = SKSurface.Create(imgInfo);
            SKCanvas canvas = surface.Canvas;
            canvas.Clear(SKColors.White);

            for (int i = 0; i < datas.Count; i++)
            {
                int row = 1 + (i / maxColumn);
                int column = (i + 1) % maxColumn;
                int areaX = margin * column + areaWidth * (column - 1);
                int areaY = margin * row + areaHeight * (column - 1);
                FileInfo imgFile = datas[i].Image;
                DrawRankText(canvas, datas[i].Content, areaX, areaY);
                DrawPidText(canvas, datas[i].Content, areaX, areaY);
                if (imgFile is not null) DrawImage(canvas, imgFile, areaX, areaY);
                DrawTitleText(canvas, datas[i].Content, areaX, areaY);
            }

            using SKImage image = surface.Snapshot();
            using SKData data = image.Encode(SKEncodedImageFormat.Jpeg, 100);
            using FileStream outputStream = File.OpenWrite(savePath);
            data.SaveTo(outputStream);
            return new FileInfo(savePath);
        }

        private static void DrawRankText(SKCanvas canvas, PixivRankingContent content, int areaX, int areaY)
        {
            int x = areaX;
            int y = areaY + (lineHeight - fontSize) / 2;
            var paint = new SKPaint
            {
                FakeBoldText = true,
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                TextAlign = SKTextAlign.Center,
                TextSize = fontSize,
                Typeface = SKTypeface.FromFamilyName("SimSun")
            };
            canvas.DrawText($"#{content.rank}", new SKPoint(x, y), paint);
        }

        private static void DrawPidText(SKCanvas canvas, PixivRankingContent content, int areaX, int areaY)
        {
            int x = areaX + fontSize * 4;
            int y = areaY + (lineHeight - fontSize) / 2;
            var paint = new SKPaint
            {
                FakeBoldText = true,
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                TextAlign = SKTextAlign.Center,
                TextSize = fontSize,
                Typeface = SKTypeface.FromFamilyName("SimSun")
            };
            canvas.DrawText($"PID：{content.illust_id}", new SKPoint(x, y), paint);
        }

        private static void DrawImage(SKCanvas canvas, FileInfo imgFile, int areaX, int areaY)
        {
            int x = areaX;
            int y = areaY + lineHeight;
            using FileStream fileStream = File.OpenRead(imgFile.FullName);
            using SKBitmap originBitmap = SKBitmap.Decode(fileStream);
            double widthScale = Convert.ToDouble(maxWidth) / originBitmap.Width;
            double heightScale = Convert.ToDouble(maxHeight) / originBitmap.Height;
            double scale = Math.Min(widthScale, heightScale);
            int imgWidth = (int)(originBitmap.Width * scale);
            int imgHeight = (int)(originBitmap.Height * scale);
            using SKBitmap resizeBitmap = originBitmap.Resize(new SKImageInfo(imgWidth, imgHeight), SKFilterQuality.High);
            canvas.DrawBitmap(resizeBitmap, new SKPoint(x, y));
        }

        private static void DrawTitleText(SKCanvas canvas, PixivRankingContent content, int areaX, int areaY)
        {
            int x = areaX;
            int y = areaY + lineHeight + maxHeight;
            var paint = new SKPaint
            {
                FakeBoldText = true,
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                TextAlign = SKTextAlign.Center,
                TextSize = fontSize,
                Typeface = SKTypeface.FromFamilyName("SimSun")
            };
            canvas.DrawText($"{content.title.cutString(10)}", new SKPoint(x, y), paint);
        }

    }
}
