using TheresaBot.Main.Helper;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Command
{
    public abstract class BaseCommand
    {
        public abstract long MessageId { get; }
        public abstract long MemberId { get; }
        public string Instruction { get; init; }
        public string Command { get; set; }
        public string Prefix { get; set; }
        public string[] Params { get; init; }
        public string KeyWord { get; set; }
        public CommandType CommandType { get; init; }
        public BaseSession Session { get; init; }

        public BaseCommand(BaseSession baseSession, CommandType commandType, string instruction, string command, string prefix)
        {
            this.Prefix = prefix;
            this.Command = command;
            this.Instruction = instruction;
            this.CommandType = commandType;
            this.Session = baseSession;
            this.KeyWord = instruction.SplitKeyWord(command);
            this.Params = instruction.splitKeyParams(command);
        }

        public abstract Task<bool> InvokeAsync(BaseSession session, BaseReporter reporter);

    }
}
