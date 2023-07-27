using TheresaBot.Main.Model.Invoker;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Command
{
    public abstract class FriendCommand : BaseCommand
    {
        private CommandHandler<FriendCommand> HandlerInvoker { get; init; }

        public FriendCommand(BaseSession baseSession, CommandHandler<FriendCommand> invoker, string instruction, string command, string prefix)
            : base(baseSession, invoker.CommandType, instruction, command, prefix)
        {
            this.HandlerInvoker = invoker;
        }

        public abstract List<string> GetImageUrls();

        public virtual async Task Test(FriendCommand command) => await Task.CompletedTask;

        public override async Task<bool> InvokeAsync(BaseSession session, BaseReporter reporter)
        {
            return await HandlerInvoker.HandleMethod.Invoke(this, session, reporter);
        }

    }
}
