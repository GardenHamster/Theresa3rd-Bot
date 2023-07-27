using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using System.Text;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Invoker;
using TheresaBot.MiraiHttpApi.Command;
using TheresaBot.MiraiHttpApi.Reporter;
using TheresaBot.MiraiHttpApi.Session;

namespace TheresaBot.MiraiHttpApi.Event
{
    public abstract class BaseEvent
    {
        protected MiraiSession baseSession { get; init; }

        protected MiraiReporter baseReporter { get; init; }

        public BaseEvent()
        {
            this.baseSession = new MiraiSession();
            this.baseReporter = new MiraiReporter();
        }

        public MiraiGroupCommand GetGroupCommand(IGroupMessageEventArgs args, string instruction, string prefix)
        {
            foreach (var invoker in HandlerInvokers.GroupCommands)
            {
                string commandStr = instruction.CheckCommand(invoker);
                if (string.IsNullOrWhiteSpace(commandStr)) continue;
                return new MiraiGroupCommand(baseSession, invoker, args, instruction, commandStr, prefix);
            }
            return null;
        }

        public MiraiFriendCommand GetFriendCommand(IFriendMessageEventArgs args, string instruction, string prefix)
        {
            foreach (var invoker in HandlerInvokers.FriendCommands)
            {
                string commandStr = instruction.CheckCommand(invoker);
                if (string.IsNullOrWhiteSpace(commandStr)) continue;
                return new MiraiFriendCommand(baseSession, invoker, args, instruction, commandStr, prefix);
            }
            return null;
        }

        public MiraiGroupQuoteCommand GetGroupQuoteCommand(IGroupMessageEventArgs args, string instruction, string prefix)
        {
            foreach (var invoker in HandlerInvokers.GroupQuoteCommands)
            {
                string commandStr = instruction.CheckCommand(invoker);
                if (string.IsNullOrWhiteSpace(commandStr)) continue;
                return new MiraiGroupQuoteCommand(baseSession, invoker, args, instruction, commandStr, prefix);
            }
            return null;
        }

        public string GetSimpleSendContent(IGroupMessageEventArgs args)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var message in args.Chain)
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
                    builder.Append(message.ToString());
                }
            }
            return builder.ToString().Trim();
        }

    }
}
