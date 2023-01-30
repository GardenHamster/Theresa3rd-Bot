namespace TheresaBot.Main.Model.Pixiv
{
    public class PixivFollow
    {
        public int total { get; set; }
        public List<PixivFollowUser> users { get; set; }
    }

    public class PixivFollowUser
    {
        public string userId { get; set; }

        public string userName { get; set; }

        public string userComment { get; set; }
    }
}
