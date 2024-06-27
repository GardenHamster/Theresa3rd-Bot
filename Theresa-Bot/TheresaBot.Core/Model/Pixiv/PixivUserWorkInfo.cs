﻿using TheresaBot.Core.Datas;
using TheresaBot.Core.Helper;

namespace TheresaBot.Core.Model.Pixiv
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
        public bool IsImproper => xRestrict > 1 || GetTags().IsImproper();
        public bool IsR18 => xRestrict > 0 || GetTags().IsR18();
        public bool IsAI => aiType > 1 || GetTags().IsAI();
        public bool IsExpired(int shelfLife) => shelfLife > 0 && createDate.AddSeconds(shelfLife) < DateTime.Now;
        public List<string> GetTags() => tags ?? new List<string>();
        public List<string> HavingBanTags() => GetTags().HavingBanTags();
    }
}