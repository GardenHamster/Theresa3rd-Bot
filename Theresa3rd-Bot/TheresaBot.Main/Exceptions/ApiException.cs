namespace TheresaBot.Main.Exceptions
{
    public class ApiException : BaseException
    {
        public ApiException(string message) : base(message) { }

        public ApiException(Exception innerException, string message) : base(innerException, message) { }

    }
}
