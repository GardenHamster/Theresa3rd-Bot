namespace TheresaBot.Main.Exceptions
{
    public class HandleException : Exception
    {
        public string RemindMessage { get; init; }

        public HandleException(string message) : base(message)
        {
            this.RemindMessage = message;
        }
    }
}
