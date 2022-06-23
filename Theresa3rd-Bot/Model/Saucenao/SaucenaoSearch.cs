using System;
using Theresa3rd_Bot.Model.Pixiv;
using Theresa3rd_Bot.Type;

namespace Theresa3rd_Bot.Model.Saucenao
{
    public class SaucenaoSearch
    {
        public SaucenaoSourceType SourceType { get; set; }

        public string SourceUrl { get; set; }

        public string SourceId { get; set; }

        public string Similarity { get; set; }

        public int MatchCount { get; set; }

        public DateTime StartDateTime { get; set; }

        public PixivWorkInfoDto pixivWorkInfoDto { get; set; }

        public SaucenaoSearch() { }

        public SaucenaoSearch(SaucenaoSourceType sourceType, DateTime startDateTime, string sourceUrl, string sourceId, string similarity, int matchCount)
        {
            this.SourceType = sourceType;
            this.SourceUrl = sourceUrl;
            this.SourceId = sourceId;
            this.Similarity = similarity;
            this.StartDateTime = startDateTime;
            this.MatchCount = matchCount;
        }

    }
}
