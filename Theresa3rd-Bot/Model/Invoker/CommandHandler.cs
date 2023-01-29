using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Theresa3rd_Bot.BotPlatform.Base.Command;
using Theresa3rd_Bot.Type;

namespace Theresa3rd_Bot.Model.Invoker
{
    public class CommandHandler<T> where T : BaseCommand
    {
        public List<string> Commands { get; set; }

        public CommandType CommandType { get; set; }

        public Func<T, Task<bool>> HandleMethod { get; set; }

        public CommandHandler(List<string> commands, CommandType commandType, Func<T, Task<bool>> handleMethod)
        {
            Commands = commands;
            CommandType = commandType;
            HandleMethod = handleMethod;
        }

    }
}
