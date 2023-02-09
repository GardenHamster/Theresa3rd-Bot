using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Invoker;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Command
{
    public abstract class FriendCommand : BaseCommand
    {
        public CommandHandler<FriendCommand> HandlerInvoker { get; init; }

        public FriendCommand(CommandHandler<FriendCommand> invoker, int msgId, string instruction, string command, long memberId)
            : base(msgId, invoker.CommandType, instruction, command, memberId)
        {
            this.HandlerInvoker = invoker;
            this.MemberId = memberId;
        }

        public abstract List<string> GetImageUrls();

        public abstract Task<int> ReplyFriendMessageAsync(string message);

        public abstract Task<int> ReplyFriendMessageAsync(List<BaseContent> contents);

        public abstract Task<int> ReplyFriendTemplateAsync(string template, string defaultmsg);

        public override async Task<bool> InvokeAsync(BaseSession session, BaseReporter reporter)
        {
            return await HandlerInvoker.HandleMethod.Invoke(this, session, reporter);
        }

        public override async Task ReplyError(Exception ex, string message = "")
        {
            List<BaseContent> contents = ex.GetErrorContents( SendTarget.Friend, message);
            await ReplyFriendMessageAsync(contents);
        }

    }
}
