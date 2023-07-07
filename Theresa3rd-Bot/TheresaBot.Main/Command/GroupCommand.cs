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
        public abstract long GroupId { get; }

        private CommandHandler<GroupCommand> HandlerInvoker { get; init; }

        public GroupCommand(CommandType commandType, string instruction, string command)
            : base(commandType, instruction, command)
        {
        }

        public GroupCommand(CommandHandler<GroupCommand> invoker, string instruction, string command)
            : base(invoker.CommandType, instruction, command)
        {
            this.HandlerInvoker = invoker;
        }

        public abstract List<string> GetImageUrls();

        public abstract long GetQuoteMessageId();

        public abstract Task<BaseResult> ReplyGroupMessageAsync(string message, bool isAt = false);

        public abstract Task<BaseResult> ReplyGroupMessageAsync(List<BaseContent> contentList, bool isAt = false);

        public abstract Task<BaseResult> ReplyGroupMessageWithAtAsync(string plainMsg);

        public abstract Task<BaseResult> ReplyGroupMessageWithAtAsync(params BaseContent[] contentArr);

        public abstract Task<BaseResult> ReplyGroupMessageWithAtAsync(List<BaseContent> contentList);

        public abstract Task<BaseResult> SendTempMessageAsync(List<BaseContent> contentList);

        public abstract Task RevokeGroupMessageAsync(long msgId, long groupId);

        public override async Task<bool> InvokeAsync(BaseSession session, BaseReporter reporter)
        {
            return await HandlerInvoker.HandleMethod.Invoke(this, session, reporter);
        }

    }
}
