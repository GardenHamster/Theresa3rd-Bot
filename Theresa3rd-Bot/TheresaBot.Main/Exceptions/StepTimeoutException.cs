namespace TheresaBot.Main.Exceptions
{
    public class StepTimeoutException : ProcessException
    {
        public StepTimeoutException(string message) : base(message)
        {
        }
    }
}
