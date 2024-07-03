using TheresaBot.Core.Datas;
using TheresaBot.Core.Model.Invoker;
using TheresaBot.Core.Reporter;
using TheresaBot.Core.Session;
using TheresaBot.Core.Type;

namespace TheresaBot.Core.Command
{
    public abstract class GroupCommand : BaseCommand
    {
        public abstract long GroupId { get; }

        public abstract string MemberNick { get; }

        private CommandHandler<GroupCommand> HandlerInvoker { get; init; }

        public GroupCommand(BaseSession baseSession, CommandType commandType, string message, string instruction, string command, string prefix)
            : base(baseSession, commandType, message, instruction, command, prefix)
        {
        }

        public GroupCommand(BaseSession baseSession, CommandHandler<GroupCommand> invoker, string message, string instruction, string command, string prefix)
            : base(baseSession, invoker.CommandType, message, instruction, command, prefix)
        {
            this.HandlerInvoker = invoker;
        }

        public virtual async Task Test(GroupCommand command) => await Task.CompletedTask;

        public abstract List<string> GetImageUrls();

        public abstract long GetQuoteMessageId();

        public override async Task<bool> InvokeAsync(BaseSession session, BaseReporter reporter)
        {
            CountDatas.AddHandleTimes();
            return await HandlerInvoker.HandleMethod.Invoke(this, session, reporter);
        }

    }
}
