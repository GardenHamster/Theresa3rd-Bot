namespace TheresaBot.Main.Exceptions
{
    public class GameException : Exception
    {
        public string RemindMessage { get; init; }

        public GameException(string message) : base(message)
        {
            this.RemindMessage = message;
        }

    }
}
