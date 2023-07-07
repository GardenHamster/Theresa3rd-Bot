using EleCho.GoCqHttpSdk.Action;
using TheresaBot.Main.Result;

namespace TheresaBot.GoCqHttp.Result
{
    public class CQResult : BaseResult
    {
        public CQResult(long msgId, bool isFailed, string errorMsg) : base(msgId, isFailed, errorMsg)
        {
        }

        public CQResult(CqSendMessageActionResult actionResult) : base(actionResult.MessageId, actionResult.Status == CqActionStatus.Failed, actionResult.ErrorMsg ?? String.Empty)
        {
        }

        public CQResult(CqSendGroupForwardMessageActionResult actionResult) : base(actionResult.MessageId, actionResult.Status == CqActionStatus.Failed, actionResult.ErrorMsg ?? String.Empty)
        {
        }

    }
}
