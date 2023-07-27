using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;
using EleCho.GoCqHttpSdk.Post;
using System.Text;
using TheresaBot.GoCqHttp.Command;
using TheresaBot.GoCqHttp.Reporter;
using TheresaBot.GoCqHttp.Session;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Invoker;

namespace TheresaBot.GoCqHttp.Plugin
{
    public class BasePlugin : CqPostPlugin
    {
        protected CQSession baseSession { get; init; }

        protected CQReporter baseReporter { get; init; }

        public BasePlugin()
        {
            this.baseSession = new CQSession();
            this.baseReporter = new CQReporter();
        }

        public CQGroupCommand GetGroupCommand(CqGroupMessagePostContext args, string instruction, string prefix)
        {
            foreach (var invoker in HandlerInvokers.GroupCommands)
            {
                string commandStr = instruction.CheckCommand(invoker);
                if (string.IsNullOrWhiteSpace(commandStr)) continue;
                return new CQGroupCommand(baseSession, invoker, args, instruction, commandStr, prefix);
            }
            return null;
        }

        public CQFriendCommand GetFriendCommand(CqPrivateMessagePostContext args, string instruction, string prefix)
        {
            foreach (var invoker in HandlerInvokers.FriendCommands)
            {
                string commandStr = instruction.CheckCommand(invoker);
                if (string.IsNullOrWhiteSpace(commandStr)) continue;
                return new CQFriendCommand(baseSession, invoker, args, instruction, commandStr, prefix);
            }
            return null;
        }

        public CQGroupQuoteCommand GetGroupQuoteCommand(CqGroupMessagePostContext args, string instruction, string prefix)
        {
            foreach (var invoker in HandlerInvokers.GroupQuoteCommands)
            {
                string commandStr = instruction.CheckCommand(invoker);
                if (string.IsNullOrWhiteSpace(commandStr)) continue;
                return new CQGroupQuoteCommand(baseSession, invoker, args, instruction, commandStr, prefix);
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
