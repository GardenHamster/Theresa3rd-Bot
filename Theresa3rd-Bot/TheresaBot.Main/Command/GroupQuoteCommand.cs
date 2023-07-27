using TheresaBot.Main.Model.Invoker;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Command
{
    public abstract class GroupQuoteCommand : GroupCommand
    {
        protected CommandHandler<GroupQuoteCommand> HandlerInvoker { get; init; }

        public GroupQuoteCommand(BaseSession baseSession, CommandHandler<GroupQuoteCommand> invoker, string instruction, string command, string prefix)
            : base(baseSession, invoker.CommandType, instruction, command, prefix)
        {
            this.HandlerInvoker = invoker;
        }

        public override async Task<bool> InvokeAsync(BaseSession session, BaseReporter reporter)
        {
            return await HandlerInvoker.HandleMethod.Invoke(this, session, reporter);
        }

    }
}
