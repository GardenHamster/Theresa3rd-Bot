using TheresaBot.Main.Common;
using TheresaBot.Main.Model.Config;

namespace TheresaBot.Main.Drawer
{
    internal class WordCloudDrawer : BaseDrawer
    {
        public async Task<FileInfo> DrawWordCloud(List<string> words, WordCloudMask maskItem)
        {
            var fontFile = GetWordCloudFontFile() ?? GetDefaultFont();
            if (fontFile is null) throw new Exception("未指定用于绘制词云的字体");
            var wordCloud = new WordCloud.WordCloud(fontFile, true);
            var fullImageSavePath = FilePath.GetWordCloudImgSavePath();
            var maskFile = GetMaskFile(maskItem);
            if (maskItem is null || maskFile is null)
            {
                var width = BotConfig.WordCloudConfig.DefaultWidth;
                var height = BotConfig.WordCloudConfig.DefaultHeitht;
                return await wordCloud.Draw(words, width, height, fullImageSavePath);
            }
            else
            {
                return await wordCloud.Draw(words, maskFile, maskItem.Width, fullImageSavePath);
            }
        }

        protected FileInfo GetWordCloudFontFile()
        {
            String fontPath = BotConfig.WordCloudConfig.FontPath;
            if (File.Exists(fontPath) == false) return null;
            return new FileInfo(fontPath);
        }

        protected FileInfo GetMaskFile(WordCloudMask maskItem)
        {
            if (maskItem is null) return null;
            if (string.IsNullOrWhiteSpace(maskItem.Path)) return null;
            if (File.Exists(maskItem.Path) == false) return null;
            return new FileInfo(maskItem.Path);
        }


    }
}
