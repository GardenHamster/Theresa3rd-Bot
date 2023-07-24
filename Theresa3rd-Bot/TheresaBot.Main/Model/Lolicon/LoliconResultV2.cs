using TheresaBot.Main.Datas;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Base;

namespace TheresaBot.Main.Model.Lolicon
{
    public class LoliconResultV2
    {
        public string error { get; set; }

        public List<LoliconDataV2> data { get; set; }
    }

    public class LoliconDataV2 : BaseWorkInfo
    {
        public int pid { get; set; }
        public int p { get; set; }
        public int uid { get; set; }
        public string title { get; set; }
        public string author { get; set; }
        public bool r18 { get; set; }
        public int aiType { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public List<string> tags { get; set; }
        public string ext { get; set; }
        public long uploadDate { get; set; }
        public LoliconUrlsV2 urls { get; set; }

        public override bool IsR18 => r18;
        public override bool IsGif => tags != null && tags.IsGif();
        public override bool IsAI => aiType > 1;
        public override bool IsImproper => tags != null && tags.IsImproper();
        public override int PixivId => pid;
        public override int UserId => uid;
        public override string Title => title;
        public override string UserName => author;

        public override List<string> GetTags() => tags ?? new List<string>();

        public override List<string> HavingBanTags() => GetTags().HavingBanTags();

        public override List<string> GetOriginalUrls()
        {
            if (urls is null) return new List<string>();
            return new List<string>() { urls.original };
        }
    }

    public class LoliconUrlsV2
    {
        public string original { get; set; }
    }
}
