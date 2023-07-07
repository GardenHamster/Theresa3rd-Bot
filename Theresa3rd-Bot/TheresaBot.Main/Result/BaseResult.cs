namespace TheresaBot.Main.Result
{
    public abstract class BaseResult
    {
        public long MsgId { get; init; }
        public bool IsFailed { get; init; }
        public string ErrorMsg { get; init; }

        protected BaseResult(long msgId, bool isFailed, string errorMsg)
        {
            this.MsgId = msgId;
            this.IsFailed = isFailed;
            this.ErrorMsg = errorMsg;
        }

    }
}
