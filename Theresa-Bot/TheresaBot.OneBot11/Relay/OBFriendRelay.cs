using EleCho.GoCqHttpSdk.Message;
using EleCho.GoCqHttpSdk.Post;
using TheresaBot.Core.Relay;

namespace TheresaBot.OneBot11.Relay
{
    public class OBFriendRelay : PrivateRelay
    {
        public CqPrivateMessagePostContext Args { get; set; }

        public override long MsgId => Args.MessageId;

        public override long MemberId => Args.Sender.UserId;

        public OBFriendRelay(CqPrivateMessagePostContext args, string message, bool isInstruct) : base(message, isInstruct)
        {
            Args = args;
        }

        public override List<string> GetImageUrls()
        {
            return Args.Message.OfType<CqImageMsg>().Select(o => o.Url?.ToString()).Where(o => !string.IsNullOrWhiteSpace(o)).ToList();
        }

    }
}
