using TheresaBot.Main.Model.Pixiv;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.Saucenao
{
    public class Ascii2dItem
    {
        public SetuSourceType SourceType { get; set; }

        public string SourceUrl { get; set; }

        public string SourceId { get; set; }

        public PixivWorkInfo PixivWorkInfo { get; set; }

        public Ascii2dItem(SetuSourceType sourceType, string sourceUrl, string sourceId)
        {
            this.SourceType = sourceType;
            this.SourceUrl = sourceUrl;
            this.SourceId = sourceId;
        }


    }
}
