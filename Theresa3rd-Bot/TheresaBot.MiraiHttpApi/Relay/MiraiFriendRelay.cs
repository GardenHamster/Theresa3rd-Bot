using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using TheresaBot.Main.Relay;

namespace TheresaBot.MiraiHttpApi.Relay
{
    public class MiraiFriendRelay : FriendRelay
    {
        public IFriendMessageEventArgs Args { get; set; }

        public MiraiFriendRelay(IFriendMessageEventArgs args, int msgId, string message, long memberId) : base(msgId, message, memberId)
        {
            this.Args = args;
        }

        public override List<string> GetImageUrls()
        {
            return Args.Chain.Where(o => o is ImageMessage).Select(o => ((ImageMessage)o).Url).Where(o => !string.IsNullOrWhiteSpace(o)).ToList();
        }

    }
}
