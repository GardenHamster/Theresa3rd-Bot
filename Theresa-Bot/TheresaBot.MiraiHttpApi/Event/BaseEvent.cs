using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using System.Text;
using TheresaBot.Core.Helper;
using TheresaBot.Core.Invoker;
using TheresaBot.MiraiHttpApi.Command;
using TheresaBot.MiraiHttpApi.Reporter;
using TheresaBot.MiraiHttpApi.Session;

namespace TheresaBot.MiraiHttpApi.Event
{
    public abstract class BaseEvent
    {
        protected MiraiSession BaseSession { get; init; }

        protected MiraiReporter BaseReporter { get; init; }

        public BaseEvent()
        {
            this.BaseSession = new MiraiSession();
            this.BaseReporter = new MiraiReporter();
        }

        public MiraiGroupCommand GetGroupCommand(IGroupMessageEventArgs args, string message, string instruction, string prefix)
        {
            foreach (var invoker in HandlerInvokers.GroupCommands)
            {
                string commandStr = instruction.CheckCommand(invoker);
                if (string.IsNullOrWhiteSpace(commandStr)) continue;
                return new MiraiGroupCommand(BaseSession, invoker, args, message, instruction, commandStr, prefix);
            }
            return null;
        }

        public MiraiFriendCommand GetFriendCommand(IFriendMessageEventArgs args, string message, string instruction, string prefix)
        {
            foreach (var invoker in HandlerInvokers.FriendCommands)
            {
                string commandStr = instruction.CheckCommand(invoker);
                if (string.IsNullOrWhiteSpace(commandStr)) continue;
                return new MiraiFriendCommand(BaseSession, invoker, args, message, instruction, commandStr, prefix);
            }
            return null;
        }

        public MiraiGroupQuoteCommand GetGroupQuoteCommand(IGroupMessageEventArgs args, string message, string instruction, string prefix)
        {
            foreach (var invoker in HandlerInvokers.GroupQuoteCommands)
            {
                string commandStr = instruction.CheckCommand(invoker);
                if (string.IsNullOrWhiteSpace(commandStr)) continue;
                return new MiraiGroupQuoteCommand(BaseSession, invoker, args, message, instruction, commandStr, prefix);
            }
            return null;
        }

        public string GetSimpleSendContent(IGroupMessageEventArgs args)
        {
            StringBuilder builder = new StringBuilder();
            var chainList = args.Chain.Skip(1).ToList();
            foreach (var message in chainList)
            {
                if (builder.Length > 0) builder.Append(" ");
                if (message is PlainMessage plainMsg)
                {
                    builder.Append(plainMsg.Message);
                }
                else if (message is ImageMessage imgMsg)
                {
                    builder.Append(imgMsg.ImageId + ".image");
                }
                else
                {
                    builder.Append(message.Rawdata.ToString());
                }
            }
            return builder.ToString().Trim();
        }

    }
}
