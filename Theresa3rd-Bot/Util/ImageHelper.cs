using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Theresa3rd_Bot.Util
{
    public static class ImageHelper
    {
        public static FileInfo saveBitmapToJpg(Image bmp,string savePath)
        {
            ImageCodecInfo imageCodecInfo = GetEncoderInfo("image/jpeg");
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            myEncoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
            string fullSavePath = Path.Combine(savePath, StringHelper.get16UUID() + ".jpg");
            bmp.Save(fullSavePath, imageCodecInfo, myEncoderParameters);
            return new FileInfo(fullSavePath);
        }

        public static FileInfo saveBitmapToPng(Image bmp, string savePath)
        {
            ImageCodecInfo imageCodecInfo = GetEncoderInfo("image/png");
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            myEncoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
            string fullSavePath = Path.Combine(savePath, StringHelper.get16UUID() + ".png");
            bmp.Save(fullSavePath, imageCodecInfo, myEncoderParameters);
            return new FileInfo(fullSavePath);
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            foreach (var encoder in encoders)
            {
                if (encoder.MimeType == mimeType) return encoder;
            }
            return null;
        }

        /// <summary>
        /// 按照分辨率压缩图片
        /// </summary>
        /// <param name="originFile"></param>
        /// <param name="savePath"></param>
        /// <param name="size"></param>
        /// <param name="maxCompressTimes"></param>
        /// <returns></returns>
        public static FileInfo compressImage(FileInfo originFile, string savePath, double size, int maxCompressTimes)
        {
            FileInfo compressFileInfo = compressImage(originFile, savePath, size);
            maxCompressTimes--;
            while (compressFileInfo.Length > size && maxCompressTimes > 0)
            {
                compressFileInfo = compressImage(compressFileInfo, savePath, size);
                maxCompressTimes--;
            }
            return compressFileInfo;
        }

        /// <summary>
        /// 按照分辨率压缩图片
        /// </summary>
        /// <param name="originFile"></param>
        /// <param name="savePath"></param>
        /// <param name="size">目标大小</param>
        /// <returns></returns>
        private static FileInfo compressImage(FileInfo originFile, string savePath, double size)
        {
            double originSize = originFile.Length;
            if (originSize < size) return originFile;
            Bitmap originBitmap = new Bitmap(originFile.FullName);
            int originWidth = originBitmap.Size.Width;
            int originHeight = originBitmap.Size.Height;
            double multiple = Math.Floor(size / originSize * 100 - 1) / 100;
            if (multiple < 0) multiple = 0.01;
            double sqrt = Math.Sqrt(multiple);
            int changeWidth = (int)Math.Ceiling(originWidth * sqrt * 0.95);
            int changeHeight = (int)Math.Ceiling(originHeight * sqrt * 0.95);
            Bitmap bitmap = new Bitmap(changeWidth, changeHeight);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.DrawImage(originBitmap, 0, 0, changeWidth, changeHeight);
            graphics.Dispose();
            return saveBitmapToJpg(bitmap, savePath);
        }



    }
}
