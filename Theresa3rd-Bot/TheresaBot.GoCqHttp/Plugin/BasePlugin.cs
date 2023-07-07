using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Post;
using TheresaBot.GoCqHttp.Command;
using TheresaBot.GoCqHttp.Helper;
using TheresaBot.GoCqHttp.Reporter;
using TheresaBot.GoCqHttp.Session;
using TheresaBot.Main.Invoker;

namespace TheresaBot.GoCqHttp.Plugin
{
    public class BasePlugin : CqPostPlugin
    {
        protected CQSession cqSession { get; init; }
        protected CQReporter cqReporter { get; init; }

        public BasePlugin()
        {
            this.cqSession = new CQSession();
            this.cqReporter = new CQReporter();
        }

        public CQGroupCommand GetGroupCommand(ICqActionSession session, CqGroupMessagePostContext args, string message)
        {
            foreach (var invoker in HandlerInvokers.GroupCommands)
            {
                CQGroupCommand command = message.CheckCommand(cqSession, invoker, session, args);
                if (command is not null) return command;
            }
            return null;
        }

        public CQFriendCommand GetFriendCommand(ICqActionSession session, CqPrivateMessagePostContext args, string message)
        {
            foreach (var invoker in HandlerInvokers.FriendCommands)
            {
                CQFriendCommand command = message.CheckCommand(cqSession, invoker, session, args);
                if (command is not null) return command;
            }
            return null;
        }

        public CQGroupQuoteCommand GetGroupQuoteCommand(ICqActionSession session, CqGroupMessagePostContext args, string message)
        {
            foreach (var invoker in HandlerInvokers.GroupQuoteCommands)
            {
                CQGroupQuoteCommand command = message.CheckCommand(cqSession, invoker, session, args);
                if (command is not null) return command;
            }
            return null;
        }

    }
}
