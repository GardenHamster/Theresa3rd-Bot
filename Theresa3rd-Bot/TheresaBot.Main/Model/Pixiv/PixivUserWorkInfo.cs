using TheresaBot.Main.Helper;

namespace TheresaBot.Main.Model.Pixiv
{
    public class PixivUserWorkInfo
    {
        public string id { get; set; }
        public string title { get; set; }
        public int illustType { get; set; }
        public int userId { get; set; }
        public string userName { get; set; }
        public int pageCount { get; set; }
        public string url { get; set; }
        public List<string> tags { get; set; }
        public DateTime createDate { get; set; }
        public int xRestrict { get; set; }
        public int aiType { get; set; }
        public bool IsImproper => xRestrict > 1 || getTags().IsImproper();
        public bool IsR18 => xRestrict > 0 || getTags().IsR18();
        public bool IsAI => aiType > 1 || getTags().IsAI();
        public string hasBanTag() => getTags().hasBanTags();
        public List<string> getTags() => tags ?? new List<string>();
    }
}
