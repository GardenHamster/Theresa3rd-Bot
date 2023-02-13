namespace TheresaBot.Main.Exceptions
{
    public class PixivException : Exception
    {
        public PixivException(string message) : base(message)
        {
        }

        public PixivException(Exception innerException) : base(String.Empty, innerException)
        {
        }

        public PixivException(Exception innerException, string message) : base(message, innerException)
        {
        }

    }
}
