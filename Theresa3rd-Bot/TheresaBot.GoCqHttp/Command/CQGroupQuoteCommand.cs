using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Post;
using TheresaBot.Main.Command;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Invoker;
using TheresaBot.Main.Type;

namespace TheresaBot.GoCqHttp.Command
{
    public class CQGroupQuoteCommand : GroupQuoteCommand
    {
        private ICqActionSession Session;
        private CqGroupMessagePostContext Args;
        private CQGroupCommand BaseCommand;
        public override PlatformType PlatformType { get; } = PlatformType.GoCQHttp;

        public CQGroupQuoteCommand(CommandHandler<GroupQuoteCommand> invoker, ICqActionSession session, CqGroupMessagePostContext args, string instruction, string command, long groupId, long memberId)
            : base(invoker, args.MessageId, instruction, command, groupId, memberId)
        {
            Args = args;
            Session = session;
            BaseCommand = new(session, args, invoker.CommandType, instruction, command, groupId, memberId);
        }

        public override List<string> GetImageUrls() => BaseCommand.GetImageUrls();

        public override long GetQuoteMessageId() => BaseCommand.GetQuoteMessageId();

        public override Task<long?> ReplyGroupMessageAsync(string message, bool isAt = false) => BaseCommand.ReplyGroupMessageAsync(message, isAt);

        public override Task<long?> ReplyGroupMessageAsync(List<BaseContent> contentList, bool isAt = false) => BaseCommand.ReplyGroupMessageAsync(contentList, isAt);

        public override Task<long?> ReplyGroupMessageWithAtAsync(string plainMsg) => BaseCommand.ReplyGroupMessageWithAtAsync(plainMsg);

        public override Task<long?> ReplyGroupMessageWithAtAsync(params BaseContent[] contentArr) => BaseCommand.ReplyGroupMessageWithAtAsync(contentArr);

        public override Task<long?> ReplyGroupMessageWithAtAsync(List<BaseContent> contentList) => BaseCommand.ReplyGroupMessageWithAtAsync(contentList);

        public override Task<long?> ReplyGroupTemplateWithAtAsync(string template, string defaultmsg = "") => BaseCommand.ReplyGroupTemplateWithAtAsync(template, defaultmsg);

        public override Task RevokeGroupMessageAsync(long? msgId, long groupId, int revokeInterval = 0) => BaseCommand.RevokeGroupMessageAsync(msgId, groupId, revokeInterval);

        public override Task<long?> SendTempMessageAsync(List<BaseContent> contentList) => BaseCommand.SendTempMessageAsync(contentList);

    }
}
