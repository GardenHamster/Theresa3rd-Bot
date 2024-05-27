using TheresaBot.Core.Datas;
using TheresaBot.Core.Model.Invoker;
using TheresaBot.Core.Reporter;
using TheresaBot.Core.Session;

namespace TheresaBot.Core.Command
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
            CountDatas.AddHandleTimes();
            return await HandlerInvoker.HandleMethod.Invoke(this, session, reporter);
        }

    }
}
