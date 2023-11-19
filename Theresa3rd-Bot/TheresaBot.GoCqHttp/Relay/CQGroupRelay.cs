using EleCho.GoCqHttpSdk.Message;
using EleCho.GoCqHttpSdk.Post;
using TheresaBot.Main.Relay;

namespace TheresaBot.GoCqHttp.Relay
{
    public class CQGroupRelay : GroupRelay
    {
        public CqGroupMessagePostContext Args { get; set; }

        public override long QuoteMsgId => Args.Message.OfType<CqReplyMsg>().FirstOrDefault()?.Id ?? 0;

        public CQGroupRelay(CqGroupMessagePostContext args, long msgId, string message, long groupId, long memberId, bool isAt, bool isQuote, bool isInstruct)
            : base(msgId, message, groupId, memberId, isAt, isQuote, isInstruct)
        {
            Args = args;
        }

        public override List<string> GetImageUrls()
        {
            return Args.Message.OfType<CqImageMsg>().Select(o => o.Url?.ToString()).Where(o => !string.IsNullOrWhiteSpace(o)).ToList();
        }

    }
}
