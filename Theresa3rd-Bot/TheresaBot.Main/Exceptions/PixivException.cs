namespace TheresaBot.Main.Exceptions
{
    public class PixivException : ApiException
    {
        public PixivException(string message) : base(message) { }

        public PixivException(Exception innerException, string message) : base(innerException, message) { }

    }
}
