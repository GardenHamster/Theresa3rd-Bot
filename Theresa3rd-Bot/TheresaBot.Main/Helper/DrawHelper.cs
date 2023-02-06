using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheresaBot.Main.Model.PixivRanking;

namespace TheresaBot.Main.Helper
{
    public class DrawHelper
    {
        public static FileInfo DrawPixivRankingPreview(List<PixivRankingPreview> rankingPreviews)
        {
            int width = 400;
            int height = 400;

            int margin = 20;
            float fontSize = 16;
            int maxImgWidth = 280;
            int maxImgHeight = 400;
            string savePath = @"C:\BotImg\preview.jpg";

            var imgInfo = new SKImageInfo(width, height);
            using SKSurface surface = SKSurface.Create(imgInfo);
            SKCanvas canvas = surface.Canvas;
            canvas.Clear(SKColors.White);

            using FileStream fileStream = File.OpenRead(@"C:\BotImg\94773793_p0.jpg");
            using SKBitmap originalBitmap = SKBitmap.Decode(fileStream);
            using SKBitmap resizedBitmap = originalBitmap.Resize(new SKImageInfo(100, 100), SKFilterQuality.High);

            canvas.DrawBitmap(resizedBitmap, new SKPoint(0, 0));

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
            canvas.DrawText("这里是作品标题", new SKPoint(250, 250), paint);

            using SKImage image = surface.Snapshot();
            using SKData data = image.Encode(SKEncodedImageFormat.Jpeg, 100);
            using FileStream outputStream = File.OpenWrite(savePath);
            data.SaveTo(outputStream);
            return new FileInfo(savePath);
        }





    }
}
