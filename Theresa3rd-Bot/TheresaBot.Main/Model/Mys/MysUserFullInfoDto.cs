using System.Collections.Generic;

namespace TheresaBot.Main.Model.Mys
{
    public class MysUserFullInfoDto
    {
        public MysUserFullInfo user_info { get; set; }
    }

    public class MysUserFullInfo
    {
        public string nickname { get; set; }

        public string introduce { get; set; }

        public string uid { get; set; }

        public string avatar_url { get; set; }
    }

}
