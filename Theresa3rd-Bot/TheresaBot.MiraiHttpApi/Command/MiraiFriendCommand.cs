using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Models;
using TheresaBot.Main.Command;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Invoker;
using TheresaBot.Main.Type;
using TheresaBot.MiraiHttpApi.Helper;

namespace TheresaBot.MiraiHttpApi.Command
{
    public class MiraiFriendCommand : FriendCommand
    {
        private IFriendMessageEventArgs Args;
        private IMiraiHttpSession Session;
        public override PlatformType PlatformType { get; } = PlatformType.Mirai;

        public MiraiFriendCommand(CommandHandler<FriendCommand> invoker, IMiraiHttpSession session, IFriendMessageEventArgs args, string instruction, string command, long memberId)
            : base(invoker, args.GetMessageId(), instruction, command, memberId)
        {
            this.Args = args;
            this.Session = session;
        }

        public override List<string> GetImageUrls()
        {
            return Args.Chain.OfType<ImageMessage>().Select(o => o.Url).ToList();
        }

        public override async Task<long?> ReplyFriendTemplateAsync(string template, string defaultmsg)
        {
            if (string.IsNullOrWhiteSpace(template)) template = defaultmsg;
            if (string.IsNullOrWhiteSpace(template)) return 0;
            IChatMessage[] msgList = await template.SplitToChainAsync().ToMiraiMessageAsync(UploadTarget.Friend);
            return await Session.SendFriendMessageAsync(MemberId, msgList);
        }

        public override async Task<long?> ReplyFriendMessageAsync(string message)
        {
            return await Session.SendFriendMessageAsync(MemberId, new PlainMessage(message));
        }

        public override async Task<long?> ReplyFriendMessageAsync(List<BaseContent> contents)
        {
            IChatMessage[] msgList = await contents.ToMiraiMessageAsync(UploadTarget.Friend);
            return await Session.SendFriendMessageAsync(MemberId, msgList);
        }

    }
}
