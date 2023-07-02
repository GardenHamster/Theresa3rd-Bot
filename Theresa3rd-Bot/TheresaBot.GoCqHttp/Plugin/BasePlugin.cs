using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Post;
using TheresaBot.GoCqHttp.Command;
using TheresaBot.GoCqHttp.Helper;
using TheresaBot.Main.Invoker;

namespace TheresaBot.GoCqHttp.Plugin
{
    public class BasePlugin : CqPostPlugin
    {
        public CQGroupCommand GetGroupCommand(ICqActionSession session, CqGroupMessagePostContext args, string message, long groupId, long memberId)
        {
            foreach (var invoker in HandlerInvokers.GroupCommands)
            {
                CQGroupCommand command = message.CheckCommand(invoker, session, args, groupId, memberId);
                if (command is not null) return command;
            }
            return null;
        }

        public CQFriendCommand GetFriendCommand(ICqActionSession session, CqPrivateMessagePostContext args, string message, long memberId)
        {
            foreach (var invoker in HandlerInvokers.FriendCommands)
            {
                CQFriendCommand command = message.CheckCommand(invoker, session, args, memberId);
                if (command is not null) return command;
            }
            return null;
        }

        public CQGroupQuoteCommand GetGroupQuoteCommand(ICqActionSession session, CqGroupMessagePostContext args, string message, long groupId, long memberId)
        {
            foreach (var invoker in HandlerInvokers.GroupQuoteCommands)
            {
                CQGroupQuoteCommand command = message.CheckCommand(invoker, session, args, groupId, memberId);
                if (command is not null) return command;
            }
            return null;
        }

    }
}
