using SkiaSharp;
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
            DefaultTypeface = GetDefaultTypeface();
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

        protected SKPaint getDefaultPaint(SKColor color, int fontSize)
        {
            return new SKPaint
            {
                FakeBoldText = true,
                Color = color,
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                TextAlign = SKTextAlign.Left,
                TextSize = fontSize,
                Typeface = DefaultTypeface
            };
        }

        protected SKPaint getDefaultPaint(SKColor color, int fontSize, bool bold)
        {
            return new SKPaint
            {
                FakeBoldText = bold,
                Color = color,
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                TextAlign = SKTextAlign.Left,
                TextSize = fontSize,
                Typeface = DefaultTypeface
            };
        }

        protected FileInfo GetDefaultFont()
        {
            try
            {
                String fontPath = BotConfig.GeneralConfig.DefaultFontPath;
                if (File.Exists(fontPath) == false) return null;
                return new FileInfo(fontPath);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return null;
            }
        }

        protected SKTypeface GetDefaultTypeface()
        {
            try
            {
                var defaultFont = GetDefaultFont();
                if (defaultFont is null) return null;
                return SKTypeface.FromFile(defaultFont.FullName);
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
