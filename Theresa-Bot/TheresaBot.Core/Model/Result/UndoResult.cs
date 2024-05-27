namespace TheresaBot.Core.Model.Result
{
    public sealed class UndoResult : BaseResult
    {
        public override long MessageId => 0;

        public override bool IsFailed => false;

        public override bool IsSuccess => false;

        public override string ErrorMsg => string.Empty;
    }

}
