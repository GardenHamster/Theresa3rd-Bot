namespace TheresaBot.Core.Exceptions
{
    public class BaseException : Exception
    {
        public BaseException(string message) : base(message) { }

        public BaseException(Exception innerException, string message) : base(message, innerException) { }

    }
}
