using TheresaBot.Core.Model.Result;

namespace TheresaBot.MiraiHttpApi.Result
{
    public class MiraiResult : BaseResult
    {
        private int _messageId { get; init; }
        public override long MessageId => _messageId;
        public override bool IsFailed => _messageId < 0;
        public override bool IsSuccess => _messageId > 0;
        public override string ErrorMsg => string.Empty;

        public MiraiResult() { }

        public MiraiResult(int messageId)
        {
            this._messageId = messageId;
        }

    }
}
