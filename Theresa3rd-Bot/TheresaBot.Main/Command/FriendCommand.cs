using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Invoker;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Command
{
    public abstract class FriendCommand : BaseCommand
    {
        private CommandHandler<FriendCommand> HandlerInvoker { get; init; }

        public FriendCommand(CommandHandler<FriendCommand> invoker, long msgId, string instruction, string command, long memberId)
            : base(invoker.CommandType, msgId, instruction, command, memberId)
        {
            this.HandlerInvoker = invoker;
            this.MemberId = memberId;
        }

        public abstract List<string> GetImageUrls();

        public abstract Task<long?> ReplyFriendMessageAsync(string message);

        public abstract Task<long?> ReplyFriendMessageAsync(List<BaseContent> contents);

        public abstract Task<long?> ReplyFriendTemplateAsync(string template, string defaultmsg);

        public override async Task ReplyError(Exception ex, string message = "")
        {
            List<BaseContent> contents = ex.GetErrorContents(message);
            await ReplyFriendMessageAsync(contents);
        }

        public override async Task<bool> InvokeAsync(BaseSession session, BaseReporter reporter)
        {
            return await HandlerInvoker.HandleMethod.Invoke(this, session, reporter);
        }

    }
}
