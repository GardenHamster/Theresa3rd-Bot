namespace TheresaBot.Core.Model.Jwt
{
    public class JwtTokenVo
    {
        public string Token { get; set; }
        public long CreateAt { get; set; }
        public long ExpiredAt { get; set; }
        public long ExpiredSeconds { get; set; }
    }
}
