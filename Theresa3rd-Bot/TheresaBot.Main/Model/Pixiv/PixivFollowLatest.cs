namespace TheresaBot.Main.Model.Pixiv
{
    public class PixivFollowLatest
    {
        public PixivFollowLatestPage page { get; set; }
    }

    public class PixivFollowLatestPage
    {
        public List<int> ids { get; set; }
    }
}
