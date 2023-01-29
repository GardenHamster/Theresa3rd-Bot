using TheresaBot.Main.Command;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.Invoker
{
    public class CommandHandler<T> where T : BaseCommand
    {
        public List<string> Commands { get; set; }

        public CommandType CommandType { get; set; }

        public Func<T, BaseSession, Task<bool>> HandleMethod { get; set; }

        public CommandHandler(List<string> commands, CommandType commandType, Func<T, BaseSession, Task<bool>> handleMethod)
        {
            Commands = commands;
            CommandType = commandType;
            HandleMethod = handleMethod;
        }

    }
}
