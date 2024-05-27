using TheresaBot.Core.Command;
using TheresaBot.Core.Reporter;
using TheresaBot.Core.Session;
using TheresaBot.Core.Type;

namespace TheresaBot.Core.Model.Invoker
{
    public class CommandHandler<T> where T : BaseCommand
    {
        public List<string> Commands { get; set; }

        public CommandType CommandType { get; set; }

        public Func<T, BaseSession, BaseReporter, Task<bool>> HandleMethod { get; set; }

        public CommandHandler(List<string> commands, CommandType commandType, Func<T, BaseSession, BaseReporter, Task<bool>> handleMethod)
        {
            Commands = commands?.Select(o => o.Trim()).Where(o => o.Length > 0).ToList() ?? new();
            CommandType = commandType;
            HandleMethod = handleMethod;
        }

    }
}
