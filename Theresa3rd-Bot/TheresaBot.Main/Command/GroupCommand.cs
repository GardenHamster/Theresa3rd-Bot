using TheresaBot.Main.Datas;
using TheresaBot.Main.Model.Invoker;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Command
{
    public abstract class GroupCommand : BaseCommand
    {
        public abstract long GroupId { get; }

        public abstract string MemberNick { get; }

        private CommandHandler<GroupCommand> HandlerInvoker { get; init; }

        public GroupCommand(BaseSession baseSession, CommandType commandType, string instruction, string command, string prefix)
            : base(baseSession, commandType, instruction, command, prefix)
        {
        }

        public GroupCommand(BaseSession baseSession, CommandHandler<GroupCommand> invoker, string instruction, string command, string prefix)
            : base(baseSession, invoker.CommandType, instruction, command, prefix)
        {
            this.HandlerInvoker = invoker;
        }

        public virtual async Task Test(GroupCommand command) => await Task.CompletedTask;

        public abstract List<string> GetImageUrls();

        public abstract long GetQuoteMessageId();

        public override async Task<bool> InvokeAsync(BaseSession session, BaseReporter reporter)
        {
            RunningDatas.AddHandleTimes();
            return await HandlerInvoker.HandleMethod.Invoke(this, session, reporter);
        }

    }
}
