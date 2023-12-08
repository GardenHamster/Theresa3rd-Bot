using System.Text;
using TheresaBot.Main.Model.Base;
using TheresaBot.Main.Model.Content;
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

        public BaseContent GetSimpleContent()
        {
            return new PlainContent($"相似度：{Similarity}%，来源：{SourceType}，Id：{SourceId}");
        }

        public BaseContent GetSourceContent()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"相似度：{Similarity}%，来源：{SourceType}");
            if (SourceType == SetuSourceType.Pixiv && PixivWorkInfo is not null)
            {
                builder.Append($"，PixivId：{PixivWorkInfo.illustId}");
            }
            else if (SourceType == SetuSourceType.Pixiv)
            {
                builder.Append($"，PixivId：{SourceId}");
            }
            else if (SourceType == SetuSourceType.Twitter)
            {
                builder.Append($"，Id：{SourceId}，链接：{SourceUrl}");
            }
            else
            {
                builder.Append($"，Id：{SourceId}");
            }
            return new PlainContent(builder.ToString());
        }


    }
}
