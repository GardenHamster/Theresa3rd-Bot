namespace TheresaBot.Core.Model.Mys
{
    public class MysUserDataDto
    {
        public MysUserInfo user_info { get; set; }
    }

    public class MysUserInfo
    {
        public string nickname { get; set; }

        public string introduce { get; set; }

        public string uid { get; set; }

        public string avatar_url { get; set; }
    }

}
