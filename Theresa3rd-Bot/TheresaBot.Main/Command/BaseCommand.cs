using TheresaBot.Main.Helper;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Command
{
    public abstract class BaseCommand
    {
        public long MsgId { get; set; }
        public long MemberId { get; init; }
        public string Instruction { get; init; }
        public string Command { get; set; }
        public string[] Params { get; init; }
        public string KeyWord { get; set; }
        public CommandType CommandType { get; init; }

        public BaseCommand(CommandType commandType, long msgId, string instruction, string command, long memberId)
        {
            this.MsgId = msgId;
            this.Instruction = instruction;
            this.CommandType = commandType;
            this.MemberId = memberId;
            this.Command = command;
            this.KeyWord = instruction.splitKeyWord(command);
            this.Params = instruction.splitKeyParams(command);
        }

        public abstract Task<bool> InvokeAsync(BaseSession session, BaseReporter reporter);

        public abstract Task ReplyError(Exception ex, string message = "");

    }
}
