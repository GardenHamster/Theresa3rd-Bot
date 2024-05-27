using System.Text;
using TheresaBot.Core.Model.Base;
using TheresaBot.Core.Model.Content;
using TheresaBot.Core.Model.Pixiv;
using TheresaBot.Core.Type;

namespace TheresaBot.Core.Model.Saucenao
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
            var builder = new StringBuilder($"相似度：{Similarity}%，来源：{SourceType}");
            if (SourceType == SetuSourceType.Pixiv && PixivWorkInfo is not null)
            {
                builder.Append($"，Id：{PixivWorkInfo.illustId}");
            }
            else if (SourceType == SetuSourceType.Pixiv)
            {
                builder.Append($"，Id：{SourceId}");
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
