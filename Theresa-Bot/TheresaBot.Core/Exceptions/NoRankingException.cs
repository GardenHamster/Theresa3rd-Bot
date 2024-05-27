namespace TheresaBot.Core.Exceptions
{
    public class NoRankingException : ApiException
    {
        public NoRankingException(string message) : base(message) { }
    }
}
