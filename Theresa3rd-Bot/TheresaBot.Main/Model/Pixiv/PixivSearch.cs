namespace TheresaBot.Main.Model.Pixiv
{
    public class PixivSearch
    {
        public PixivSearchIllustManga illust { get; set; }

        public PixivSearchIllustManga illustManga { get; set; }

        public PixivSearchIllustManga getIllust()
        {
            if (illust != null) return illust;
            if (illustManga != null) return illustManga;
            return illust;
        }
    }

    public class PixivSearchIllustManga
    {
        public int total { get; set; }
        public List<PixivIllust> data { get; set; }
    }

    public class PixivIllust
    {
        public string id { get; set; }
        public string illustTitle { get; set; }
        public int pageCount { get; set; }
        public string url { get; set; }
        public string userId { get; set; }
        public string userName { get; set; }
        public DateTime createDate { get; set; }
        public DateTime updateDate { get; set; }

    }
}
