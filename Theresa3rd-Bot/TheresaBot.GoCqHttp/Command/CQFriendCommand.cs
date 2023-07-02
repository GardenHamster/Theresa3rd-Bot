using EleCho.GoCqHttpSdk.Post;
using EleCho.GoCqHttpSdk;
using TheresaBot.Main.Command;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Invoker;
using EleCho.GoCqHttpSdk.Message;
using TheresaBot.GoCqHttp.Helper;
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

        public override async Task<long?> ReplyFriendTemplateAsync(string template, string defaultmsg)
        {
            if (string.IsNullOrWhiteSpace(template)) template = defaultmsg;
            if (string.IsNullOrWhiteSpace(template)) return 0;
            CqMsg[] msgList = template.SplitToChainAsync().ToCQMessageAsync();
            var result = await Session.SendPrivateMessageAsync(MemberId, new CqMessage(msgList));
            return result is null ? 0 : result.MessageId;
        }

        public override async Task<long?> ReplyFriendMessageAsync(string message)
        {
            var result = await Session.SendPrivateMessageAsync(MemberId, new CqMessage(message));
            return result is null ? 0 : result.MessageId;
        }

        public override async Task<long?> ReplyFriendMessageAsync(List<BaseContent> contents)
        {
            CqMsg[] msgList = contents.ToCQMessageAsync();
            var result = await Session.SendPrivateMessageAsync(MemberId, new CqMessage(msgList));
            return result is null ? 0 : result.MessageId;
        }

    }
}
