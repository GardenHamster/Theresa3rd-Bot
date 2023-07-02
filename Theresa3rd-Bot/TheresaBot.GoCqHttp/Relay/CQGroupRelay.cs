using EleCho.GoCqHttpSdk.Message;
using EleCho.GoCqHttpSdk.Post;
using TheresaBot.Main.Relay;

namespace TheresaBot.GoCqHttp.Relay
{
    public class CQGroupRelay : GroupRelay
    {
        public CqGroupMessagePostContext Args { get; set; }

        public CQGroupRelay(CqGroupMessagePostContext args, long msgId, string message, long groupId, long memberId) : base(msgId, message, groupId, memberId)
        {
            Args = args;
        }

        public override List<string> GetImageUrls()
        {
            return Args.Message.OfType<CqImageMsg>().Select(o => o.Image).ToList();
        }

    }
}
