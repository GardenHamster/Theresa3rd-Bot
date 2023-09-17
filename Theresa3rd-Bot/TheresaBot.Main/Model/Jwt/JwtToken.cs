namespace TheresaBot.Main.Model.Jwt
{
    public class JwtToken
    {
        public string AccessToken { get; set; }

        public string TokenType { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime ExpiredTime { get; set; }

        public long ExpiredSeconds { get; set; }
    }
}
