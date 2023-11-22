using System.Text;
using TheresaBot.Main.Model.Base;
using TheresaBot.Main.Model.Pixiv;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.Saucenao
{
    public record SaucenaoItem : BaseSourceItem
    {
        public decimal Similarity { get; set; }

        public PixivWorkInfo PixivWorkInfo { get; set; }

        public SaucenaoItem(SetuSourceType sourceType, string sourceUrl, string sourceId, decimal similarity)
        {
            this.SourceType = sourceType;
            this.SourceUrl = sourceUrl;
            this.SourceId = sourceId;
            this.Similarity = similarity;
        }

        public string GetSourceContent()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"相似度：{Similarity}%，来源：{SourceType}");
            if (SourceType == SetuSourceType.Pixiv)
            {
                builder.Append($"，PixivId：{PixivWorkInfo?.illustId}");
            }
            return builder.ToString();
        }

        public string GetSimpleContent()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"相似度：{Similarity}%，来源：{SourceType}");
            if (SourceType == SetuSourceType.Pixiv)
            {
                builder.Append($"，PixivId：{PixivWorkInfo?.illustId}");
            }
            builder.Append($"，链接：{SourceUrl}");
            return builder.ToString();
        }


    }
}
