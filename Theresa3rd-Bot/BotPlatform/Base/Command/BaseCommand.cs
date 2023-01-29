using System.Threading.Tasks;
using Theresa3rd_Bot.Type;

namespace Theresa3rd_Bot.BotPlatform.Base.Command
{
    public abstract class BaseCommand
    {
        public long MemberId { get; init; }
        public string Instruction { get; init; }
        public string[] KeyWords { get; init; }
        public CommandType CommandType { get; init; }
        public string KeyWord
        {
            get { return KeyWords is not null && KeyWords.Length > 0 ? KeyWords[0].Trim() : string.Empty; }
        }

        public BaseCommand(string[] keyWords, CommandType commandType, string instruction, long memberId)
        {
            this.KeyWords = keyWords;
            this.Instruction = instruction;
            this.CommandType = commandType;
            this.MemberId = memberId;
        }

        public abstract Task<bool> InvokeAsync();

    }
}
