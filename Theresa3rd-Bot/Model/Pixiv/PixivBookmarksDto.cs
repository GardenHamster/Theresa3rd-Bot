using System.Collections.Generic;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Model.Pixiv
{
    public class PixivBookmarksDto
    {
        public bool error { get; set; }
        public string message { get; set; }
        public PixivBookmarksBody body { get; set; }
    }

    public class PixivBookmarksBody
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
        public bool isR18()
        {
            return tags != null && tags.IsR18();
        }
    }


}

