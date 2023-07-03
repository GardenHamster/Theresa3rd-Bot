using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;
using EleCho.GoCqHttpSdk.Post;
using TheresaBot.GoCqHttp.Helper;
using TheresaBot.GoCqHttp.Result;
using TheresaBot.Main.Command;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Invoker;
using TheresaBot.Main.Result;
using TheresaBot.Main.Type;

namespace TheresaBot.GoCqHttp.Command
{
    public class CQFriendCommand : FriendCommand
    {
        private ICqActionSession Session;
        private CqPrivateMessagePostContext Args;
        public override PlatformType PlatformType { get; } = PlatformType.GoCQHttp;

        public CQFriendCommand(CommandHandler<FriendCommand> invoker, ICqActionSession session, CqPrivateMessagePostContext args, string instruction, string command, long memberId)
            : base(invoker, args.MessageId, instruction, command, memberId)
        {
            Args = args;
            Session = session;
        }

        public override List<string> GetImageUrls()
        {
            return Args.Message.OfType<CqImageMsg>().Select(o => o.Image).ToList();
        }

        public override async Task<BaseResult> ReplyFriendTemplateAsync(string template, string defaultmsg)
        {
            if (string.IsNullOrWhiteSpace(template)) template = defaultmsg;
            if (string.IsNullOrWhiteSpace(template)) return CQResult.Undo;
            CqMsg[] msgList = template.SplitToChainAsync().ToCQMessageAsync();
            var result = await Session.SendPrivateMessageAsync(MemberId, new CqMessage(msgList));
            return new CQResult(result);
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
