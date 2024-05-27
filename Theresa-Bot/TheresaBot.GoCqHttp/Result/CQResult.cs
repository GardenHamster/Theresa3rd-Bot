using EleCho.GoCqHttpSdk.Action;
using TheresaBot.Core.Model.Result;

namespace TheresaBot.GoCqHttp.Result
{
    public class CQResult : BaseResult
    {
        private long _messageId { get; init; }
        public CqActionResult ActionResult { get; init; }
        public override long MessageId => _messageId;
        public override bool IsFailed => _messageId == 0;
        public override bool IsSuccess => _messageId != 0;
        public override string ErrorMsg => ActionResult.ErrorMsg;

        public CQResult() { }

        public CQResult(CqActionResult result, long messageId)
        {
            this.ActionResult = result;
            this._messageId = messageId;
        }

    }
}
