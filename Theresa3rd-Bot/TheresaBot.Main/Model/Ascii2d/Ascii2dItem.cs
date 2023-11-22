using System.Text;
using TheresaBot.Main.Model.Base;
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

        public string GetSimpleContent()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"来源：{SourceType}");
            if (SourceType == SetuSourceType.Pixiv)
            {
                builder.Append($"，PixivId：{PixivWorkInfo?.illustId}");
            }
            builder.Append($"，链接：{SourceUrl}");
            return builder.ToString();
        }

    }
}
