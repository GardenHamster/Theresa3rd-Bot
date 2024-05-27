namespace TheresaBot.Core.Model.Error
{
    public class ErrorRecord
    {
        public string Message { get; set; }

        public string InnerMessage { get; set; }

        public Exception Exception { get; set; }

        public Exception InnerException { get; set; }

        public ErrorRecord(Exception exception)
        {
            this.Exception = exception;
            this.InnerException = exception.InnerException;
            this.Message = exception.Message;
            this.InnerMessage = exception.InnerException?.Message;
        }

    }
}
