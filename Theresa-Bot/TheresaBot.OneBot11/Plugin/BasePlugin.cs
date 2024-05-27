using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;
using EleCho.GoCqHttpSdk.Post;
using System.Text;
using TheresaBot.OneBot11.Command;
using TheresaBot.OneBot11.Reporter;
using TheresaBot.OneBot11.Session;
using TheresaBot.Core.Helper;
using TheresaBot.Core.Invoker;

namespace TheresaBot.OneBot11.Plugin
{
    public class BasePlugin : CqPostPlugin
    {
        protected OBSession BaseSession { get; init; }

        protected OBReporter baseReporter { get; init; }

        public BasePlugin()
        {
            this.BaseSession = new OBSession();
            this.baseReporter = new OBReporter();
        }

        public OBGroupCommand GetGroupCommand(CqGroupMessagePostContext args, string instruction, string prefix)
        {
            foreach (var invoker in HandlerInvokers.GroupCommands)
            {
                string commandStr = instruction.CheckCommand(invoker);
                if (string.IsNullOrWhiteSpace(commandStr)) continue;
                return new OBGroupCommand(BaseSession, invoker, args, instruction, commandStr, prefix);
            }
            return null;
        }

        public OBFriendCommand GetFriendCommand(CqPrivateMessagePostContext args, string instruction, string prefix)
        {
            foreach (var invoker in HandlerInvokers.FriendCommands)
            {
                string commandStr = instruction.CheckCommand(invoker);
                if (string.IsNullOrWhiteSpace(commandStr)) continue;
                return new OBFriendCommand(BaseSession, invoker, args, instruction, commandStr, prefix);
            }
            return null;
        }

        public OBGroupQuoteCommand GetGroupQuoteCommand(CqGroupMessagePostContext args, string instruction, string prefix)
        {
            foreach (var invoker in HandlerInvokers.GroupQuoteCommands)
            {
                string commandStr = instruction.CheckCommand(invoker);
                if (string.IsNullOrWhiteSpace(commandStr)) continue;
                return new OBGroupQuoteCommand(BaseSession, invoker, args, instruction, commandStr, prefix);
            }
            return null;
        }

        public string GetSimpleSendContent(CqGroupMessagePostContext args)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var message in args.Message)
            {
                if (builder.Length > 0) builder.Append(" ");
                if (message is CqTextMsg textMsg)
                {
                    builder.Append(textMsg.Text);
                }
                else if (message is CqImageMsg imgMsg)
                {
                    builder.Append(imgMsg.Image);
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
