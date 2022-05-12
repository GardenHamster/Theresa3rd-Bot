using System;

namespace Theresa3rd_Bot.Model.Error
{
    public class SendError
    {
        public string Message { get; set; }

        public int SendTimes { get; set; }

        public DateTime LastSendTime { get; set; }

        public SendError(string message)
        {
            this.Message = message;
            this.SendTimes = 1;
            this.LastSendTime = DateTime.Now;
        }

    }
}
