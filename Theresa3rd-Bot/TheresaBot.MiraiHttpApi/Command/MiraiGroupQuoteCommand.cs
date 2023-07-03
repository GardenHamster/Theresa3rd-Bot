using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using TheresaBot.Main.Command;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Invoker;
using TheresaBot.Main.Result;
using TheresaBot.Main.Type;
using TheresaBot.MiraiHttpApi.Helper;

namespace TheresaBot.MiraiHttpApi.Command
{
    public class MiraiGroupQuoteCommand : GroupQuoteCommand
    {
        private IMiraiHttpSession Session;
        private IGroupMessageEventArgs Args;
        private MiraiGroupCommand BaseCommand;
        public override PlatformType PlatformType { get; } = PlatformType.Mirai;

        public MiraiGroupQuoteCommand(CommandHandler<GroupQuoteCommand> invoker, IMiraiHttpSession session, IGroupMessageEventArgs args, string instruction, string command, long groupId, long memberId)
            : base(invoker, args.GetMessageId(), instruction, command, groupId, memberId)
        {
            this.Args = args;
            this.Session = session;
            this.BaseCommand = new(session, args, invoker.CommandType, instruction, command, groupId, memberId);
        }

        public override List<string> GetImageUrls() => BaseCommand.GetImageUrls();

        public override long GetQuoteMessageId() => BaseCommand.GetQuoteMessageId();

        public override Task<BaseResult> ReplyGroupMessageAsync(string message, bool isAt = false) => BaseCommand.ReplyGroupMessageAsync(message, isAt);

        public override Task<BaseResult> ReplyGroupMessageAsync(List<BaseContent> contentList, bool isAt = false) => BaseCommand.ReplyGroupMessageAsync(contentList, isAt);

        public override Task<BaseResult> ReplyGroupMessageWithAtAsync(string plainMsg) => BaseCommand.ReplyGroupMessageWithAtAsync(plainMsg);

        public override Task<BaseResult> ReplyGroupMessageWithAtAsync(params BaseContent[] contentArr) => BaseCommand.ReplyGroupMessageWithAtAsync(contentArr);

        public override Task<BaseResult> ReplyGroupMessageWithAtAsync(List<BaseContent> contentList) => BaseCommand.ReplyGroupMessageWithAtAsync(contentList);

        public override Task<BaseResult> ReplyGroupTemplateWithAtAsync(string template, string defaultmsg = "") => BaseCommand.ReplyGroupTemplateWithAtAsync(template, defaultmsg);

        public override Task RevokeGroupMessageAsync(long msgId, long groupId) => BaseCommand.RevokeGroupMessageAsync(msgId, groupId);

        public override Task<BaseResult> SendTempMessageAsync(List<BaseContent> contentList) => BaseCommand.SendTempMessageAsync(contentList);

    }
}
