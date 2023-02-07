using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheresaBot.Main.Helper;

namespace TheresaBot.Main.Model.PixivRanking
{
    public class PixivRankingData
    {
        public List<PixivRankingContent> contents { get; set; }
        public string mode { get; set; }
        public string content { get; set; }
        public int page { get; set; }
        public bool prev { get; set; }
        public int next { get; set; }
        public string date { get; set; }
        public string prev_date { get; set; }
        public bool next_date { get; set; }
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
        public long illust_upload_timestamp { get; set; }

        public double rating_rate => Convert.ToDouble(rating_count) / view_count;
        public bool isIllust() => illust_type == "0";
        public bool isR18() => getTags().IsR18();
        public bool isGif() => getTags().IsGif();
        public bool isImproper() => getTags().IsImproper();
        public bool hasBanTag() => getTags()?.hasBanTags() is not null;
        public List<string> getTags() => tags ?? new List<string>();
    }

}
