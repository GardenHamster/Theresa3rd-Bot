using System.Text;
using TheresaBot.Core.Helper;
using TheresaBot.Core.Model.Base;
using TheresaBot.Core.Model.Content;
using TheresaBot.Core.Model.Pixiv;
using TheresaBot.Core.Type;

namespace TheresaBot.Core.Model.Ascii2d
{
    public record Ascii2dItem : BaseSourceItem
    {
        public PixivWorkInfo PixivWorkInfo { get; set; }

        public Ascii2dItem(SetuSourceType sourceType, string sourceUrl, string sourceId)
        {
            SourceType = sourceType;
            SourceUrl = sourceUrl;
            SourceId = sourceId;
        }

        public PlainContent GetSimpleContent()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"来源：{SourceType}");
            if (SourceType == SetuSourceType.Pixiv && PixivWorkInfo is not null)
            {
                builder.Append($"，Id：{PixivWorkInfo?.illustId}");
            }
            else if (SourceType == SetuSourceType.Pixiv)
            {
                builder.Append($"，Id：{SourceId}");
            }
            else
            {
                builder.Append($"，Id：{SourceId}");
            }
            return new PlainContent(builder.ToString());
        }

        public BaseContent GetSourceContent()
        {
            StringBuilder builder = new StringBuilder();
            if (SourceType == SetuSourceType.Pixiv && PixivWorkInfo is not null)
            {
                builder.AppendLine($"来源：Pixiv，Id：{PixivWorkInfo.illustId}，链接：{PixivWorkInfo.urls.original.ToOpenProxyLink()}");
            }
            else if (SourceType == SetuSourceType.Pixiv)
            {
                builder.AppendLine($"来源：Pixiv，Id：{SourceId}，链接获取失败");
            }
            else if (SourceType == SetuSourceType.Twitter)
            {
                builder.AppendLine($"来源：Twitter，Id：{SourceId}，链接：{SourceUrl}");
            }
            else
            {
                builder.AppendLine($"来源：{SourceType}，Id：{SourceId}");
            }
            return new PlainContent(builder.ToString());
        }



    }
}
