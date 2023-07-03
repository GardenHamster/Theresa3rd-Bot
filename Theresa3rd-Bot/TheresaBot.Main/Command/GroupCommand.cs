using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Invoker;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Result;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Command
{
    public abstract class GroupCommand : BaseCommand
    {
        public long GroupId { get; init; }

        private CommandHandler<GroupCommand> HandlerInvoker { get; init; }

        public GroupCommand(CommandType commandType, long msgId, string instruction, string command, long groupId, long memberId)
            : base(commandType, msgId, instruction, command, memberId)
        {
            this.MemberId = memberId;
            this.GroupId = groupId;
        }

        public GroupCommand(CommandHandler<GroupCommand> invoker, long msgId, string instruction, string command, long groupId, long memberId)
            : base(invoker.CommandType, msgId, instruction, command, memberId)
        {
            this.HandlerInvoker = invoker;
            this.MemberId = memberId;
            this.GroupId = groupId;
        }

        public abstract List<string> GetImageUrls();

        public abstract long GetQuoteMessageId();

        public abstract Task<BaseResult> ReplyGroupMessageAsync(string message, bool isAt = false);

        public abstract Task<BaseResult> ReplyGroupMessageAsync(List<BaseContent> contentList, bool isAt = false);

        public abstract Task<BaseResult> ReplyGroupMessageWithAtAsync(string plainMsg);

        public abstract Task<BaseResult> ReplyGroupMessageWithAtAsync(params BaseContent[] contentArr);

        public abstract Task<BaseResult> ReplyGroupMessageWithAtAsync(List<BaseContent> contentList);

        public abstract Task<BaseResult> ReplyGroupTemplateWithAtAsync(string template, string defaultmsg = "");

        public abstract Task<BaseResult> SendTempMessageAsync(List<BaseContent> contentList);

        public abstract Task RevokeGroupMessageAsync(long msgId, long groupId);

        public override async Task<BaseResult> ReplyError(Exception ex, string message = "")
        {
            List<BaseContent> contents = ex.GetErrorContents(message);
            return await ReplyGroupMessageWithAtAsync(contents);
        }

        public override async Task<bool> InvokeAsync(BaseSession session, BaseReporter reporter)
        {
            return await HandlerInvoker.HandleMethod.Invoke(this, session, reporter);
        }

    }
}
