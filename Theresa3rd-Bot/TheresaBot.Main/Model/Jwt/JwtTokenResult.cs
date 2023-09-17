namespace TheresaBot.Main.Model.Jwt
{
    public class JwtTokenResult
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public long CreateAt { get; set; }
        public long ExpiredAt { get; set; }
        public long ExpiredSeconds { get; set; }
    }
}
