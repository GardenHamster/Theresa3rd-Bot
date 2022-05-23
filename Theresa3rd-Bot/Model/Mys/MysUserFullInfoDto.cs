using System.Collections.Generic;

namespace Theresa3rd_Bot.Model.Mys
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
    }

}
