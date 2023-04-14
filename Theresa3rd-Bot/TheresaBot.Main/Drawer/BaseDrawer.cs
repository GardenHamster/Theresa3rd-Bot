using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;

namespace TheresaBot.Main.Drawer
{
    internal abstract class BaseDrawer
    {
        protected const string FontDir = "Font";

        protected readonly SKTypeface DefaultTypeface;

        public BaseDrawer()
        {
            DefaultTypeface = GetTypeface();
        }

        protected SKTypeface GetTypeface()
        {
            try
            {
                if (Directory.Exists(FontDir) == false) return null;
                var fontDirectory = new DirectoryInfo(FontDir);
                var fontFiles = fontDirectory.GetFiles();
                if (fontFiles.Length == 0) return null;
                var firstFont = fontFiles.First();
                return SKTypeface.FromFile(firstFont.FullName);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return null;
            }
        }

        protected bool IsHorizontal(SKBitmap originBitmap)
        {
            return Convert.ToDouble(originBitmap.Width) / originBitmap.Height > 1.2;
        }

        protected SKBitmap Blur(SKBitmap bitmap)
        {
            using SKImage blurImage = bitmap.Blur(BotConfig.PixivConfig.R18ImgBlur);
            return SKBitmap.FromImage(blurImage);
        }

    }
}
