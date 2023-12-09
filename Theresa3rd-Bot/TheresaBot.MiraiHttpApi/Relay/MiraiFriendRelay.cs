using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using TheresaBot.Main.Relay;
using TheresaBot.MiraiHttpApi.Helper;

namespace TheresaBot.MiraiHttpApi.Relay
{
    public class MiraiFriendRelay : FriendRelay
    {
        public IFriendMessageEventArgs Args { get; set; }

        public override long MsgId => Args.GetMessageId();

        public override long MemberId => Args.Sender.Id;

        public MiraiFriendRelay(IFriendMessageEventArgs args, string message, bool isInstruct) : base(message, isInstruct)
        {
            this.Args = args;
        }

        public override List<string> GetImageUrls()
        {
            return Args.Chain.OfType<ImageMessage>().Select(o => o.Url).Where(o => !string.IsNullOrWhiteSpace(o)).ToList();
        }

    }
}
