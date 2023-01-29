using System.Threading.Tasks;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Command
{
    public abstract class BaseCommand
    {
        public int MsgId { get; set; }
        public long MemberId { get; init; }
        public string Instruction { get; init; }
        public string[] KeyWords { get; init; }
        public CommandType CommandType { get; init; }
        public string KeyWord
        {
            get { return KeyWords is not null && KeyWords.Length > 0 ? KeyWords[0].Trim() : string.Empty; }
        }

        public BaseCommand(int msgId, string[] keyWords, CommandType commandType, string instruction, long memberId)
        {
            this.MsgId = msgId;
            this.KeyWords = keyWords;
            this.Instruction = instruction;
            this.CommandType = commandType;
            this.MemberId = memberId;
        }

        public abstract Task<bool> InvokeAsync();

    }
}
