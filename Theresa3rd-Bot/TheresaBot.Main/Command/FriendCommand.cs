using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Invoker;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Result;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Command
{
    public abstract class FriendCommand : BaseCommand
    {
        private CommandHandler<FriendCommand> HandlerInvoker { get; init; }

        public FriendCommand(CommandHandler<FriendCommand> invoker, string instruction, string command)
            : base(invoker.CommandType, instruction, command)
        {
            this.HandlerInvoker = invoker;
        }

        public abstract List<string> GetImageUrls();

        public abstract Task<BaseResult> ReplyFriendMessageAsync(string message);

        public abstract Task<BaseResult> ReplyFriendMessageAsync(List<BaseContent> contents);

        public override async Task<bool> InvokeAsync(BaseSession session, BaseReporter reporter)
        {
            return await HandlerInvoker.HandleMethod.Invoke(this, session, reporter);
        }

    }
}
