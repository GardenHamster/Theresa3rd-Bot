using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Base;

namespace TheresaBot.Main.Model.Pixiv
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
        public string illustComment { get; set; }
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

        public override bool IsR18 => xRestrict > 0 || getTags().IsR18();
        public override bool IsGif => illustType == 2;
        public override bool IsAI => aiType > 1 || getTags().IsAI();
        public override bool IsImproper => xRestrict > 1 || getTags().IsImproper();
        public override int PixivId => illustId;
        public override int UserId => userId;
        public override string Title => illustTitle;
        public override string UserName => userName;

        public override List<string> getTags() => tags?.getTags() ?? new List<string>();

        public override string hasBanTag() => tags?.getFullTags()?.hasBanTags();

        public override List<string> getOriginalUrls()
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
