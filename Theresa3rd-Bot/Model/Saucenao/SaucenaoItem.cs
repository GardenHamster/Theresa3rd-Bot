using Theresa3rd_Bot.Model.Pixiv;
using Theresa3rd_Bot.Type;

namespace Theresa3rd_Bot.Model.Saucenao
{
    public class SaucenaoItem
    {
        public SetuSourceType SourceType { get; set; }

        public string SourceUrl { get; set; }

        public string SourceId { get; set; }

        public decimal Similarity { get; set; }

        public PixivWorkInfo PixivWorkInfo { get; set; }

        public SaucenaoItem(SetuSourceType sourceType, string sourceUrl, string sourceId, decimal similarity)
        {
            this.SourceType = sourceType;
            this.SourceUrl = sourceUrl;
            this.SourceId = sourceId;
            this.Similarity = similarity;
        }


    }
}
