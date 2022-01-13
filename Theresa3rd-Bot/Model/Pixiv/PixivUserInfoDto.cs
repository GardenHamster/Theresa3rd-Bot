using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Model.Pixiv
{
    public class PixivUserInfoDto
    {
        public bool error { get; set; }
        public string message { get; set; }
        public PixivUserInfo body { get; set; }
    }

    public class PixivUserInfo
    {
        public PixivUserExtraData extraData { get; set; }
    }

    public class PixivUserExtraData
    {
        public PixivUserMeta meta { get; set; }
    }

    public class PixivUserMeta
    {
        public string title { get; set; }
    }

}
