using TheresaBot.Core.Datas;
using TheresaBot.Core.Helper;
using TheresaBot.Core.Model.Base;

namespace TheresaBot.Core.Model.Pixiv
{
    public class PixivWorkInfo : BaseWorkInfo
    {
        public int RelevantCount { get; set; }
        public int bookmarkCount { get; set; }
        public int viewCount { get; set; }
        public int likeCount { get; set; }
        public int illustId { get; set; }
        public string illustTitle { get; set; }
        public int illustType { get; set; }
        public int userId { get; set; }
        public string userName { get; set; }
        public DateTime createDate { get; set; }
        public PixivUrls urls { get; set; }
        public int pageCount { get; set; }
        public PixivTags tags { get; set; }
        public int xRestrict { get; set; }
        public int aiType { get; set; }
        public double likeRate => Convert.ToDouble(likeCount) / viewCount;
        public double bookmarkRate => Convert.ToDouble(bookmarkCount) / viewCount;
        public bool IsIllust => illustType == 0;
        public bool IsExpired(int shelfLife) => shelfLife > 0 && createDate.AddSeconds(shelfLife) < DateTime.Now;
        public override bool IsR18 => xRestrict > 0 || GetTags().IsR18();
        public override bool IsGif => illustType == 2;
        public override bool IsAI => aiType > 1 || GetTags().IsAI();
        public override bool IsImproper => xRestrict > 1 || GetTags().IsImproper();
        public override int PixivId => illustId;
        public override int UserId => userId;
        public override string Title => illustTitle;
        public override string UserName => userName;
        public override List<string> GetTags() => tags?.getTags() ?? new List<string>();
        public override List<string> HavingBanTags() => GetTags().HavingBanTags();
        public override List<string> GetOriginalUrls()
        {
            if (urls is null) return new List<string>();
            List<string> urlList = new List<string>();
            for (int i = 0; i < pageCount; i++)
            {
                urlList.Add(urls.original.Replace("_p0.", $"_p{i}."));
            }
            return urlList;
        }
    }

    public class PixivUrls
    {
        public string original { get; set; }
        public string regular { get; set; }
        public string small { get; set; }
        public string thumb { get; set; }
    }

}
