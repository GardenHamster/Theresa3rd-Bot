using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;

namespace TheresaBot.Main.Drawer
{
    internal class WordCloudDrawer : BaseDrawer
    {
        public async Task<FileInfo> DrawWordCloud(List<string> words)
        {
            var fontFile = GetWordCloudFontFile();
            if (fontFile is null) fontFile = GetDefaultFont();
            var maskFilePath = BotConfig.WordCloudConfig.MaskPaths?.RandomItem();
            var maskFile = string.IsNullOrWhiteSpace(maskFilePath) || File.Exists(maskFilePath) == false ? null : new FileInfo(maskFilePath);
            var wordCloud = new WordCloud.WordCloud(fontFile, true);
            var fullImageSavePath = FilePath.GetFullTempImgSavePath();
            var width = BotConfig.WordCloudConfig.ImgWidth;
            var height = BotConfig.WordCloudConfig.ImgHeight;
            if (maskFile is null)
            {
                return await wordCloud.Draw(words, width, height, fullImageSavePath);
            }
            else
            {
                return await wordCloud.Draw(words, maskFile, width, fullImageSavePath);
            }
        }

        protected FileInfo GetWordCloudFontFile()
        {
            try
            {
                String fontPath = BotConfig.WordCloudConfig.FontPath;
                if (File.Exists(fontPath) == false) return null;
                return new FileInfo(fontPath);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return null;
            }
        }


    }
}
