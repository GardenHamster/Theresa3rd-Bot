using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Invoker;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Command
{
    public abstract class GroupCommand : BaseCommand
    {
        public long GroupId { get; init; }

        private CommandHandler<GroupCommand> HandlerInvoker { get; init; }

        public GroupCommand(CommandType commandType, int msgId, string instruction, string command, long groupId, long memberId)
            : base(commandType, msgId, instruction, command, memberId)
        {
            this.MemberId = memberId;
            this.GroupId = groupId;
        }

        public GroupCommand(CommandHandler<GroupCommand> invoker, int msgId, string instruction, string command, long groupId, long memberId)
            : base(invoker.CommandType, msgId, instruction, command, memberId)
        {
            this.HandlerInvoker = invoker;
            this.MemberId = memberId;
            this.GroupId = groupId;
        }

        public abstract List<string> GetImageUrls();

        public abstract Task<int> ReplyGroupMessageAsync(string message, bool isAt = false);

        public abstract Task<int> ReplyGroupMessageAsync(List<BaseContent> contentList, bool isAt = false);

        public abstract Task<int> ReplyGroupMessageWithAtAsync(string plainMsg);

        public abstract Task<int> ReplyGroupMessageWithAtAsync(params BaseContent[] contentArr);

        public abstract Task<int> ReplyGroupMessageWithAtAsync(List<BaseContent> contentList);

        public abstract Task<int> ReplyGroupTemplateWithAtAsync(string template, string defaultmsg = "");

        public abstract Task<int> SendTempMessageAsync(List<BaseContent> contentList);

        public abstract Task RevokeGroupMessageAsync(int messageId, long groupId, int revokeInterval = 0);

        public override async Task ReplyError(Exception ex, string message = "")
        {
            List<BaseContent> contents = ex.GetErrorContents(message);
            await ReplyGroupMessageWithAtAsync(contents);
        }

        public override async Task<bool> InvokeAsync(BaseSession session, BaseReporter reporter)
        {
            return await HandlerInvoker.HandleMethod.Invoke(this, session, reporter);
        }

    }
}
