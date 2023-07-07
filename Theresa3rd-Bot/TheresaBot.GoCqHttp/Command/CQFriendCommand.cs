using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;
using EleCho.GoCqHttpSdk.Post;
using TheresaBot.GoCqHttp.Helper;
using TheresaBot.GoCqHttp.Result;
using TheresaBot.Main.Command;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Invoker;
using TheresaBot.Main.Result;
using TheresaBot.Main.Type;

namespace TheresaBot.GoCqHttp.Command
{
    public class CQFriendCommand : FriendCommand
    {
        private ICqActionSession Session { get; init; }

        private CqPrivateMessagePostContext Args { get; init; }

        public override PlatformType PlatformType { get; } = PlatformType.GoCQHttp;

        public override long MsgId => Args.MessageId;

        public override long MemberId => Args.Sender.UserId;

        public CQFriendCommand(CommandHandler<FriendCommand> invoker, ICqActionSession session, CqPrivateMessagePostContext args, string instruction, string command)
            : base(invoker, instruction, command)
        {
            Args = args;
            Session = session;
        }

        public override List<string> GetImageUrls()
        {
            return Args.Message.OfType<CqImageMsg>().Select(o => o.Image).ToList();
        }

        public override async Task<BaseResult> ReplyFriendMessageAsync(string message)
        {
            var result = await Session.SendPrivateMessageAsync(MemberId, new CqMessage(message));
            return new CQResult(result);
        }

        public override async Task<BaseResult> ReplyFriendMessageAsync(List<BaseContent> contents)
        {
            CqMsg[] msgList = contents.ToCQMessageAsync();
            var result = await Session.SendPrivateMessageAsync(MemberId, new CqMessage(msgList));
            return new CQResult(result);
        }

    }
}
