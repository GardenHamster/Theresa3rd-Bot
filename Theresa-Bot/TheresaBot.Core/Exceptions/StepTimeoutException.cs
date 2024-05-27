namespace TheresaBot.Core.Exceptions
{
    public class StepTimeoutException : ProcessException
    {
        public StepTimeoutException(string message) : base(message)
        {
        }
    }
}
