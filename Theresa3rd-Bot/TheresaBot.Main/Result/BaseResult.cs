namespace TheresaBot.Main.Result
{
    public abstract class BaseResult
    {
        public abstract long MessageId { get; }
        public abstract bool IsFailed { get; }
        public abstract bool IsSuccess { get; }
        public abstract string ErrorMsg { get; }
        public static BaseResult Undo => new UndoResult();
        protected BaseResult() { }
    }

}
