using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheresaBot.Main.Command;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Invoker;
using TheresaBot.MiraiHttpApi.Helper;

namespace TheresaBot.MiraiHttpApi.Command
{
    public class MiraiGroupQuoteCommand : GroupQuoteCommand
    {
        private IMiraiHttpSession Session;
        private IGroupMessageEventArgs Args;
        private MiraiGroupCommand BaseCommand;

        public MiraiGroupQuoteCommand(CommandHandler<GroupQuoteCommand> invoker, IMiraiHttpSession session, IGroupMessageEventArgs args, string instruction, string command, long groupId, long memberId)
            : base(invoker, args.GetMessageId(), instruction, command, groupId, memberId)
        {
            this.Args = args;
            this.Session = session;
            this.BaseCommand = new(session, args, invoker.CommandType, instruction, command, groupId, memberId);
        }

        public override List<string> GetImageUrls() => BaseCommand.GetImageUrls();

        public override long GetQuoteMessageId() => BaseCommand.GetQuoteMessageId();

        public override Task<long> ReplyGroupMessageAsync(string message, bool isAt = false) => BaseCommand.ReplyGroupMessageAsync(message, isAt);

        public override Task<long> ReplyGroupMessageAsync(List<BaseContent> contentList, bool isAt = false) => BaseCommand.ReplyGroupMessageAsync(contentList, isAt);

        public override Task<long> ReplyGroupMessageWithAtAsync(string plainMsg) => BaseCommand.ReplyGroupMessageWithAtAsync(plainMsg);

        public override Task<long> ReplyGroupMessageWithAtAsync(params BaseContent[] contentArr) => BaseCommand.ReplyGroupMessageWithAtAsync(contentArr);

        public override Task<long> ReplyGroupMessageWithAtAsync(List<BaseContent> contentList) => BaseCommand.ReplyGroupMessageWithAtAsync(contentList);

        public override Task<long> ReplyGroupTemplateWithAtAsync(string template, string defaultmsg = "") => BaseCommand.ReplyGroupTemplateWithAtAsync(template, defaultmsg);

        public override Task RevokeGroupMessageAsync(long messageId, long groupId, int revokeInterval = 0) => BaseCommand.RevokeGroupMessageAsync(messageId, groupId, revokeInterval);

        public override Task<long> SendTempMessageAsync(List<BaseContent> contentList) => BaseCommand.SendTempMessageAsync(contentList);

    }
}
