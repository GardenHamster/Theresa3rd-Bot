using TheresaBot.Core.Datas;
using TheresaBot.Core.Helper;

namespace TheresaBot.Core.Model.Pixiv
{
    public class PixivBookmarks
    {
        public int total { get; set; }
        public List<PixivBookmarksWork> works { get; set; }
    }

    public class PixivBookmarksWork
    {
        public string id { get; set; }
        public int illustType { get; set; }
        public int pageCount { get; set; }
        public string title { get; set; }
        public List<string> tags { get; set; }
        public string userId { get; set; }
        public string userName { get; set; }
        public int xRestrict { get; set; }
        public int aiType { get; set; }

        public bool IsImproper() => xRestrict > 1 || (tags != null && tags.IsImproper());

        public bool IsR18() => xRestrict > 0 || (tags != null && tags.IsR18());

        public bool IsAI() => aiType > 1 || (tags != null && tags.IsAI());

        public List<string> GetTags() => tags ?? new();

        public List<string> HavingBanTags() => GetTags().HavingBanTags();

    }


}

