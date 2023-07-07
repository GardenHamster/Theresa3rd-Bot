using TheresaBot.Main.Result;

namespace TheresaBot.MiraiHttpApi.Result
{
    public class MiraiResult : BaseResult
    {
        public MiraiResult(long msgId) : base(msgId, msgId < 0, String.Empty)
        {
        }

        public MiraiResult(long msgId, bool isFailed, string errorMsg) : base(msgId, isFailed, errorMsg)
        {
        }

    }
}
