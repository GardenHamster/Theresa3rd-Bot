namespace TheresaBot.Core.Model.Jwt
{
    public class JwtToken
    {
        public string Token { get; set; }

        public string Header { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime ExpiredTime { get; set; }

        public long ExpiredSeconds { get; set; }
    }
}
