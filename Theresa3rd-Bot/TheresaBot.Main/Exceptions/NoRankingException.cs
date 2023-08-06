namespace TheresaBot.Main.Exceptions
{
    public class NoRankingException : ApiException
    {
        public NoRankingException(string message) : base(message) { }
    }
}
