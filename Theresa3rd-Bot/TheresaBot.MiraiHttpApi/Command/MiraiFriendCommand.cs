using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Models;
using TheresaBot.Main.Command;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Invoker;
using TheresaBot.Main.Result;
using TheresaBot.Main.Type;
using TheresaBot.MiraiHttpApi.Helper;
using TheresaBot.MiraiHttpApi.Result;

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

        public override async Task<BaseResult> ReplyFriendTemplateAsync(string template, string defaultmsg)
        {
            if (string.IsNullOrWhiteSpace(template)) template = defaultmsg;
            if (string.IsNullOrWhiteSpace(template)) return MiraiResult.Undo;
            IChatMessage[] msgList = await template.SplitToChainAsync().ToMiraiMessageAsync(UploadTarget.Friend);
            var msgId = await Session.SendFriendMessageAsync(MemberId, msgList);
            return new MiraiResult(msgId);
        }

        public override async Task<BaseResult> ReplyFriendMessageAsync(string message)
        {
            var msgId = await Session.SendFriendMessageAsync(MemberId, new PlainMessage(message));
            return new MiraiResult(msgId);
        }

        public override async Task<BaseResult> ReplyFriendMessageAsync(List<BaseContent> contents)
        {
            IChatMessage[] msgList = await contents.ToMiraiMessageAsync(UploadTarget.Friend);
            var msgId = await Session.SendFriendMessageAsync(MemberId, msgList);
            return new MiraiResult(msgId);
        }

    }
}
