using SkiaSharp;
using TheresaBot.Core.Common;

namespace TheresaBot.Core.Helper
{
    public static class ImageHelper
    {
        /// <summary>
        /// 高斯模糊处理图片
        /// </summary>
        /// <param name="fileInfos"></param>
        /// <param name="sigma"></param>
        /// <returns></returns>
        public static List<FileInfo> Blur(this List<FileInfo> fileInfos, float sigma)
        {
            return fileInfos.Select(o => Blur(o, sigma)).ToList();
        }

        /// <summary>
        /// 高斯模糊处理图片
        /// </summary>
        /// <param name="fileInfos"></param>
        /// <param name="sigma"></param>
        /// <returns></returns>
        public static List<FileInfo> Blur(this List<FileInfo> fileInfos, float sigma, string saveDirPath = "")
        {
            return fileInfos.Select(o => Blur(o, sigma)).ToList();
        }

        /// <summary>
        /// 压缩图片
        /// </summary>
        /// <param name="fileInfos"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static List<FileInfo> Reduce(this List<FileInfo> fileInfos, int width)
        {
            return fileInfos.Select(o => Reduce(o, width)).ToList();
        }

        /// <summary>
        /// 压缩并模糊处理图片
        /// </summary>
        /// <param name="fileInfos"></param>
        /// <param name="sigma"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static List<FileInfo> ReduceAndBlur(this List<FileInfo> fileInfos, float sigma, int width)
        {
            return fileInfos.Select(o => ReduceAndBlur(o, width, sigma)).ToList();
        }

        /// <summary>
        /// 图片旋转180度
        /// </summary>
        /// <param name="fileInfos"></param>
        /// <returns></returns>
        public static List<FileInfo> Rotate180(this List<FileInfo> fileInfos)
        {
            return fileInfos.Select(o => Rotate180(o)).ToList();
        }

        /// <summary>
        /// 高斯模糊处理图片
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="sigma"></param>
        /// <returns></returns>
        public static FileInfo Blur(this FileInfo fileInfo, float sigma)
        {
            if (fileInfo == null) return null;
            if (sigma <= 0) sigma = 5;
            string fullFileName = $"{fileInfo.GetFileName()}_blur_{sigma}.jpg";
            string fullSavePath = FilePath.GetTempImgSavePath(fullFileName);
            if (File.Exists(fullSavePath)) return new FileInfo(fullSavePath);
            using FileStream originStream = File.OpenRead(fileInfo.FullName);
            using SKBitmap originBitmap = SKBitmap.Decode(originStream);
            using SKImage image = Blur(originBitmap, sigma);
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
        /// <returns></returns>
        public static FileInfo Reduce(this FileInfo fileInfo, int width)
        {
            if (fileInfo == null) return null;
            string fullFileName = $"{fileInfo.GetFileName()}_reduce_{width}.jpg";
            string fullSavePath = FilePath.GetTempImgSavePath(fullFileName);
            if (File.Exists(fullSavePath)) return new FileInfo(fullSavePath);
            using FileStream originStream = File.OpenRead(fileInfo.FullName);
            using SKBitmap originBitmap = SKBitmap.Decode(originStream);
            if (originBitmap.Width <= width) return fileInfo;
            using SKImage image = Reduce(originBitmap, width);
            using SKData data = image.Encode(SKEncodedImageFormat.Jpeg, 100);
            using FileStream outputStream = File.OpenWrite(fullSavePath);
            data.SaveTo(outputStream);
            return new FileInfo(fullSavePath);
        }

        /// <summary>
        /// 压缩并模糊处理图片
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="width"></param>
        /// <param name="sigma"></param>
        /// <returns></returns>
        public static FileInfo ReduceAndBlur(this FileInfo fileInfo, int width, float sigma)
        {
            if (fileInfo == null) return null;
            if (sigma <= 0) sigma = 5;
            string fullFileName = $"{fileInfo.GetFileName()}_reduce_{width}_blur_{sigma}.jpg";
            string fullSavePath = FilePath.GetTempImgSavePath(fullFileName);
            if (File.Exists(fullSavePath)) return new FileInfo(fullSavePath);
            using FileStream fileStream = File.OpenRead(fileInfo.FullName);
            using SKBitmap originBitmap = SKBitmap.Decode(fileStream);
            using SKImage reduceImg = Reduce(originBitmap, width);
            using SKBitmap reduceBitmap = SKBitmap.FromImage(reduceImg);
            using SKImage blurImg = Blur(reduceBitmap, sigma);
            using SKData data = blurImg.Encode(SKEncodedImageFormat.Jpeg, 100);
            using FileStream outputStream = File.OpenWrite(fullSavePath);
            data.SaveTo(outputStream);
            return new FileInfo(fullSavePath);
        }

        /// <summary>
        /// 图片旋转180度
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public static FileInfo Rotate180(this FileInfo fileInfo)
        {
            if (fileInfo == null) return null;
            string fullFileName = $"{fileInfo.GetFileName()}_rotate_180.jpg";
            string fullSavePath = FilePath.GetTempImgSavePath(fullFileName);
            if (File.Exists(fullSavePath)) return new FileInfo(fullSavePath);
            using FileStream originStream = File.OpenRead(fileInfo.FullName);
            using SKBitmap originBitmap = SKBitmap.Decode(originStream);
            using SKImage image = Rotate180(originBitmap);
            using SKData data = image.Encode(SKEncodedImageFormat.Jpeg, 100);
            using FileStream outputStream = File.OpenWrite(fullSavePath);
            data.SaveTo(outputStream);
            return new FileInfo(fullSavePath);
        }

        /// <summary>
        /// 高斯模糊处理图片
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="sigma"></param>
        /// <returns></returns>
        public static SKImage Blur(this SKBitmap bitmap, float sigma)
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
        public static SKImage Reduce(this SKBitmap bitmap, int width)
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

        /// <summary>
        /// 图片旋转180度
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static SKImage Rotate180(this SKBitmap bitmap)
        {
            var imgInfo = new SKImageInfo(bitmap.Width, bitmap.Height);
            using SKSurface surface = SKSurface.Create(imgInfo);
            using SKCanvas canvas = surface.Canvas;
            canvas.Clear(SKColors.Transparent);

            SKRect rect = new SKRect(0, 0, bitmap.Width, bitmap.Height);
            canvas.RotateDegrees(180, bitmap.Width / 2, bitmap.Height / 2);
            canvas.DrawBitmap(bitmap, rect);
            return surface.Snapshot();
        }



    }
}
