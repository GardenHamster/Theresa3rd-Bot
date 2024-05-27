using TheresaBot.Core.Datas;
using TheresaBot.Core.Helper;

namespace TheresaBot.Core.Model.Pixiv
{
    public class PixivRankingData
    {
        public List<PixivRankingContent> contents { get; set; }
        public string mode { get; set; }
        public string content { get; set; }
        public int page { get; set; }
        public string date { get; set; }
        public int rank_total { get; set; }
    }

    public class PixivRankingContent
    {
        public string title { get; set; }
        public string date { get; set; }
        public List<string> tags { get; set; }
        public string url { get; set; }
        public string illust_type { get; set; }
        public string illust_page_count { get; set; }
        public string user_name { get; set; }
        public int illust_id { get; set; }
        public int user_id { get; set; }
        public int rank { get; set; }
        public int rating_count { get; set; }
        public int view_count { get; set; }
        public double Rating_rate => Convert.ToDouble(rating_count) / view_count;
        public bool IsIllust() => illust_type == "0";
        public bool IsR18() => GetTags().IsR18();
        public bool IsGif() => GetTags().IsGif();
        public bool IsImproper() => GetTags().IsImproper();
        public List<string> GetTags() => tags ?? new List<string>();
        public List<string> HavingBanTags() => GetTags().HavingBanTags();
    }

}
