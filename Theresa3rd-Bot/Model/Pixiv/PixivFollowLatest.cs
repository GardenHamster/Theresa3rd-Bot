using System.Collections.Generic;

namespace Theresa3rd_Bot.Model.Pixiv
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
