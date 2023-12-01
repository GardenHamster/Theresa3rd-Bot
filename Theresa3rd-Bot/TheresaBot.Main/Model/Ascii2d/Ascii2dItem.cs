using System.Text;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Base;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Pixiv;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.Ascii2d
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
                builder.Append($"，PixivId：{PixivWorkInfo?.illustId}");
            }
            else if (SourceType == SetuSourceType.Pixiv)
            {
                builder.Append($"，PixivId：{SourceId}");
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
                builder.AppendLine($"来源：Pixiv，标题：{PixivWorkInfo.illustTitle}，pid：{PixivWorkInfo.illustId}，链接：{PixivWorkInfo.urls.original.ToOpenProxyLink()}");
            }
            else if (SourceType == SetuSourceType.Pixiv)
            {
                builder.AppendLine($"来源：Pixiv，pid：{SourceId}");
            }
            else if (SourceType == SetuSourceType.Twitter)
            {
                builder.AppendLine($"来源：Twitter，链接：{SourceUrl}");
            }
            else
            {
                builder.AppendLine($"来源：{SourceType}，id：{SourceId}");
            }
            return new PlainContent(builder.ToString());
        }



    }
}
