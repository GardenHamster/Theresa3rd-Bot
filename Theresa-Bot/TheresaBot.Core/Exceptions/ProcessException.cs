namespace TheresaBot.Core.Exceptions
{
    public class ProcessException : Exception
    {
        public string RemindMessage { get; init; }

        public ProcessException(string message) : base(message)
        {
            this.RemindMessage = message;
        }
    }
}
