using TheresaBot.Core.Helper;
using TheresaBot.Core.Type;

namespace TheresaBot.Core.Model.Logger
{
    public class LogRecord
    {
        public string Remind { get; set; }

        public Exception Exception { get; set; }

        public DateTime CreateTime { get; set; }

        public long CreateAt {  get; set; }

        public LogLevel Level { get; set; }

        public LogRecord(Exception exception, string remind, LogLevel level)
        {
            Level = level;
            Remind = remind;
            Exception = exception;
            CreateTime = DateTime.Now;
            CreateAt = DateTime.Now.ToTimeStamp();
        }
    }
}
