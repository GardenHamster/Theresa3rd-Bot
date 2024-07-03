using TheresaBot.Core.Helper;
using TheresaBot.Core.Reporter;
using TheresaBot.Core.Session;
using TheresaBot.Core.Type;

namespace TheresaBot.Core.Command
{
    public abstract class BaseCommand
    {
        /// <summary>
        /// 协议返回的消息ID
        /// </summary>
        public abstract long MessageId { get; }
        /// <summary>
        /// 发送请求的成员ID
        /// </summary>
        public abstract long MemberId { get; }
        /// <summary>
        /// 发送请求的成员昵称
        /// </summary>
        public abstract string MemberName { get; }
        /// <summary>
        /// 包含前缀的指令和指令内容内容
        /// </summary>
        public string Instruction { get; init; }
        /// <summary>
        /// 指令
        /// </summary>
        public string Command { get; set; }
        /// <summary>
        /// 指令前缀
        /// </summary>
        public string Prefix { get; set; }
        /// <summary>
        /// 第一行消息内容的中参数内容
        /// </summary>
        public string[] Params { get; init; }
        /// <summary>
        /// 不包含指令前缀和指令的第一行的内容
        /// </summary>
        public string KeyWord { get; set; }
        /// <summary>
        /// 不包含指令前缀和指令的消息内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 指令类型
        /// </summary>
        public CommandType CommandType { get; init; }
        /// <summary>
        /// 
        /// </summary>
        public BaseSession Session { get; init; }

        public BaseCommand(BaseSession baseSession, CommandType commandType, string message, string instruction, string command, string prefix)
        {
            this.Prefix = prefix;
            this.Command = command;
            this.Instruction = instruction;
            this.CommandType = commandType;
            this.Session = baseSession;
            this.Content = message.SplitKeyWord(command);
            this.KeyWord = instruction.SplitKeyWord(command);
            this.Params = instruction.SplitKeyParams(command);
        }

        public abstract Task<bool> InvokeAsync(BaseSession session, BaseReporter reporter);

    }
}
