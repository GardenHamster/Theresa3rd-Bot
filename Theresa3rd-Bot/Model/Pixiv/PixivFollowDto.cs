using System.Collections.Generic;

namespace Theresa3rd_Bot.Model.Pixiv
{
    public class PixivFollowDto
    {
        public bool error { get; set; }
        public string message { get; set; }
        public PixivFollowBody body { get; set; }
    }

    public class PixivFollowBody
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
