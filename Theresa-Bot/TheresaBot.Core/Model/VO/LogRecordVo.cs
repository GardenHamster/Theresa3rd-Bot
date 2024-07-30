using TheresaBot.Core.Type;

namespace TheresaBot.Core.Model.VO
{
    public record LogRecordVo
    {
        public string Remind { get; set; }

        public string Message { get; set; }

        public string InnerMessage { get; set; }

        public string StackTrace { get; set; }

        public long CreateAt { get; set; }

        public LogLevel Level { get; set; }

        public LogRecordVo(string remind, string message, string innerMessage, string stackTrace, long createAt, LogLevel level)
        {
            Remind = remind;
            Message = message;
            InnerMessage = innerMessage;
            StackTrace = stackTrace;
            CreateAt = createAt;
            Level = level;
        }

    }
}
