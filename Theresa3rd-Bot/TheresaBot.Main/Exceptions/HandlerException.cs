namespace TheresaBot.Main.Exceptions
{
    public class HandlerException : Exception
    {
        public string RemindMessage { get; private set; }

        public HandlerException(string message) : base(message)
        {
            RemindMessage = message;
        }

    }
}
