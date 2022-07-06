using System;

namespace Theresa3rd_Bot.Model.Error
{
    public class SendError
    {
        public string Message { get; set; }

        public string InnerMessage { get; set; }

        public Exception Exception { get; set; }

        public Exception InnerException { get; set; }

        public int SendTimes { get; set; }

        public DateTime LastSendTime { get; set; }

        public SendError(Exception exception)
        {
            this.Exception = exception;
            this.InnerException = exception.InnerException;
            this.Message = exception.Message;
            this.InnerMessage = exception.InnerException?.Message;
            this.SendTimes = 1;
            this.LastSendTime = DateTime.Now;
        }

    }
}
