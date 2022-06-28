using System.Collections.Generic;

namespace Theresa3rd_Bot.Model.Pixiv
{
    public class PixivFollowLatestDto
    {
        public bool error { get; set; }
        public string message { get; set; }
        public PixivFollowLatestBody body { get; set; }
    }

    public class PixivFollowLatestBody
    {
        public PixivFollowLatestPage page { get; set; }
    }

    public class PixivFollowLatestPage
    {
        public List<string> ids { get; set; }
    }
}
