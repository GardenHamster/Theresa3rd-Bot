namespace TheresaBot.Main.Model.Jwt
{
    public class JwtTokenVo
    {
        public string AccessToken => $"{Header} {Token}";
        public string Token { get; set; }
        public string Header { get; set; }
        public long CreateAt { get; set; }
        public long ExpiredAt { get; set; }
        public long ExpiredSeconds { get; set; }
    }
}
