using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Post;
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

        public CQGroupCommand GetGroupCommand(CqGroupMessagePostContext args, string instruction)
        {
            foreach (var invoker in HandlerInvokers.GroupCommands)
            {
                string commandStr = instruction.CheckCommand(invoker);
                if (string.IsNullOrWhiteSpace(commandStr)) continue;
                return new CQGroupCommand(baseSession, invoker, args, instruction, commandStr);
            }
            return null;
        }

        public CQFriendCommand GetFriendCommand(CqPrivateMessagePostContext args, string instruction)
        {
            foreach (var invoker in HandlerInvokers.FriendCommands)
            {
                string commandStr = instruction.CheckCommand(invoker);
                if (string.IsNullOrWhiteSpace(commandStr)) continue;
                return new CQFriendCommand(baseSession, invoker, args, instruction, commandStr);
            }
            return null;
        }

        public CQGroupQuoteCommand GetGroupQuoteCommand(CqGroupMessagePostContext args, string instruction)
        {
            foreach (var invoker in HandlerInvokers.GroupQuoteCommands)
            {
                string commandStr = instruction.CheckCommand(invoker);
                if (string.IsNullOrWhiteSpace(commandStr)) continue;
                return new CQGroupQuoteCommand(baseSession, invoker, args, instruction, commandStr);
            }
            return null;
        }

    }
}
