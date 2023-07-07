using TheresaBot.Main.Model.Invoker;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Command
{
    public abstract class GroupCommand : BaseCommand
    {
        public abstract long GroupId { get; }

        private CommandHandler<GroupCommand> HandlerInvoker { get; init; }

        public GroupCommand(BaseSession baseSession, CommandType commandType, string instruction, string command)
            : base(baseSession, commandType, instruction, command)
        {
        }

        public GroupCommand(BaseSession baseSession, CommandHandler<GroupCommand> invoker, string instruction, string command)
            : base(baseSession, invoker.CommandType, instruction, command)
        {
            this.HandlerInvoker = invoker;
        }

        public abstract List<string> GetImageUrls();

        public abstract long GetQuoteMessageId();

        public override async Task<bool> InvokeAsync(BaseSession session, BaseReporter reporter)
        {
            return await HandlerInvoker.HandleMethod.Invoke(this, session, reporter);
        }

    }
}
