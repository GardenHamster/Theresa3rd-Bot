using TheresaBot.Main.Datas;
using TheresaBot.Main.Model.Invoker;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Command
{
    public abstract class PrivateCommand : BaseCommand
    {
        private CommandHandler<PrivateCommand> HandlerInvoker { get; init; }

        public PrivateCommand(BaseSession baseSession, CommandHandler<PrivateCommand> invoker, string instruction, string command, string prefix)
            : base(baseSession, invoker.CommandType, instruction, command, prefix)
        {
            this.HandlerInvoker = invoker;
        }

        public abstract List<string> GetImageUrls();

        public virtual async Task Test(PrivateCommand command) => await Task.CompletedTask;

        public override async Task<bool> InvokeAsync(BaseSession session, BaseReporter reporter)
        {
            CountDatas.AddHandleTimes();
            return await HandlerInvoker.HandleMethod.Invoke(this, session, reporter);
        }

    }
}
