using Theresa3rd_Bot.Model.Pixiv;
using Theresa3rd_Bot.Type;

namespace Theresa3rd_Bot.Model.Saucenao
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
