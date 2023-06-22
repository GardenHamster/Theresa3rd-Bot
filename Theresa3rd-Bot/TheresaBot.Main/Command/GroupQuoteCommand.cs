using TheresaBot.Main.Model.Invoker;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Command
{
    public abstract class GroupQuoteCommand : GroupCommand
    {
        private CommandHandler<GroupQuoteCommand> HandlerInvoker { get; init; }

        public GroupQuoteCommand(CommandHandler<GroupQuoteCommand> invoker, int msgId, string instruction, string command, long groupId, long memberId)
            : base(invoker.CommandType, msgId, instruction, command, groupId, memberId)
        {
            this.HandlerInvoker = invoker;
        }

        public override async Task<bool> InvokeAsync(BaseSession session, BaseReporter reporter)
        {
            return await HandlerInvoker.HandleMethod.Invoke(this, session, reporter);
        }

    }
}
