using SkiaSharp;

namespace TheresaBot.Main.Helper
{
    public static class ImageHelper
    {
        /// <summary>
        /// 高斯模糊处理图片
        /// </summary>
        /// <param name="fileInfos"></param>
        /// <param name="sigma"></param>
        /// <param name="fullSavePath"></param>
        /// <returns></returns>
        public static List<FileInfo> Blur(this List<FileInfo> fileInfos, float sigma, string fullSavePath)
        {
            return fileInfos.Select(o => Blur(o, sigma, fullSavePath)).ToList();
        }

        /// <summary>
        /// 压缩图片
        /// </summary>
        /// <param name="fileInfos"></param>
        /// <param name="width"></param>
        /// <param name="fullSavePath"></param>
        /// <returns></returns>
        public static List<FileInfo> Reduce(this List<FileInfo> fileInfos, int width, string fullSavePath)
        {
            return fileInfos.Select(o => Reduce(o, width, fullSavePath)).ToList();
        }

        /// <summary>
        /// 压缩并模糊处理图片
        /// </summary>
        /// <param name="fileInfos"></param>
        /// <param name="sigma"></param>
        /// <param name="width"></param>
        /// <param name="fullSavePath"></param>
        /// <returns></returns>
        public static List<FileInfo> ReduceAndBlur(this List<FileInfo> fileInfos, float sigma, int width, string fullSavePath)
        {
            return fileInfos.Select(o => ReduceAndBlur(o, width, sigma, fullSavePath)).ToList();
        }

        /// <summary>
        /// 高斯模糊处理图片
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="sigma"></param>
        /// <param name="fullSavePath"></param>
        /// <returns></returns>
        public static FileInfo Blur(this FileInfo fileInfo, float sigma, string fullSavePath)
        {
            if (fileInfo == null) return null;
            if (sigma <= 0) return fileInfo;
            using FileStream orginStream = File.OpenRead(fileInfo.FullName);
            using SKBitmap orginBitmap = SKBitmap.Decode(orginStream);
            using SKImage image = Blur(orginBitmap, sigma);
            using SKData data = image.Encode(SKEncodedImageFormat.Jpeg, 100);
            using FileStream outputStream = File.OpenWrite(fullSavePath);
            data.SaveTo(outputStream);
            return new FileInfo(fullSavePath);
        }

        /// <summary>
        /// 压缩图片
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="width"></param>
        /// <param name="fullSavePath"></param>
        /// <returns></returns>
        public static FileInfo Reduce(this FileInfo fileInfo, int width, string fullSavePath)
        {
            if (fileInfo == null) return null;
            using FileStream orginStream = File.OpenRead(fileInfo.FullName);
            using SKBitmap orginBitmap = SKBitmap.Decode(orginStream);
            if (orginBitmap.Width <= width) return fileInfo;
            using SKImage image = Reduce(orginBitmap, width);
            using SKData data = image.Encode(SKEncodedImageFormat.Jpeg, 100);
            using FileStream outputStream = File.OpenWrite(fullSavePath);
            data.SaveTo(outputStream);
            return new FileInfo(fullSavePath);
        }

        /// <summary>
        /// 压缩并模糊处理图片
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="sigma"></param>
        /// <param name="width"></param>
        /// <param name="fullSavePath"></param>
        /// <returns></returns>
        public static FileInfo ReduceAndBlur(this FileInfo fileInfo, int width, float sigma, string fullSavePath)
        {
            if (fileInfo == null) return null;
            if (sigma <= 0) return fileInfo;
            using FileStream fileStream = File.OpenRead(fileInfo.FullName);
            using SKBitmap orginBitmap = SKBitmap.Decode(fileStream);
            using SKImage reduceImg = Reduce(orginBitmap, width);
            using SKBitmap reduceBitmap = SKBitmap.FromImage(reduceImg);
            using SKImage blurImg = Blur(reduceBitmap, sigma);
            using SKData data = blurImg.Encode(SKEncodedImageFormat.Jpeg, 100);
            using FileStream outputStream = File.OpenWrite(fullSavePath);
            data.SaveTo(outputStream);
            return new FileInfo(fullSavePath);
        }

        /// <summary>
        /// 高斯模糊处理图片
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        private static SKImage Blur(this SKBitmap bitmap, float sigma)
        {
            using SKPaint paint = new SKPaint();
            paint.ImageFilter = SKImageFilter.CreateBlur(sigma, sigma);

            var imgInfo = new SKImageInfo(bitmap.Width, bitmap.Height);
            using SKSurface surface = SKSurface.Create(imgInfo);
            using SKCanvas canvas = surface.Canvas;
            canvas.Clear(SKColors.White);

            SKRect rect = new SKRect(0, 0, bitmap.Width, bitmap.Height);
            canvas.DrawBitmap(bitmap, rect, rect, paint);
            return surface.Snapshot();
        }

        /// <summary>
        /// 压缩图片
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        private static SKImage Reduce(this SKBitmap bitmap, int width)
        {
            if (bitmap.Width < width) return SKImage.FromBitmap(bitmap);
            int height = (int)((Convert.ToDouble(width) / bitmap.Width) * bitmap.Height);
            var imgInfo = new SKImageInfo(width, height);
            using SKSurface surface = SKSurface.Create(imgInfo);
            using SKCanvas canvas = surface.Canvas;
            canvas.Clear(SKColors.Transparent);

            SKRect rect = new SKRect(0, 0, width, height);
            canvas.DrawBitmap(bitmap, rect);
            return surface.Snapshot();
        }

    }
}
