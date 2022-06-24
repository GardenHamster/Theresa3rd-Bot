using Theresa3rd_Bot.Model.Pixiv;
using Theresa3rd_Bot.Type;

namespace Theresa3rd_Bot.Model.Saucenao
{
    public class SaucenaoItem
    {
        public SaucenaoSourceType SourceType { get; set; }

        public string SourceUrl { get; set; }

        public string SourceId { get; set; }

        public decimal Similarity { get; set; }

        public PixivWorkInfoDto PixivWorkInfo { get; set; }

        public SaucenaoItem(SaucenaoSourceType sourceType, string sourceUrl, string sourceId, decimal similarity)
        {
            this.SourceType = sourceType;
            this.SourceUrl = sourceUrl;
            this.SourceId = sourceId;
            this.Similarity = similarity;
        }


    }
}
