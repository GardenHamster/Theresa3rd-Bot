using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using TheresaBot.Main.Invoker;
using TheresaBot.MiraiHttpApi.Command;
using TheresaBot.MiraiHttpApi.Helper;

namespace TheresaBot.MiraiHttpApi.Event
{
    public abstract class BaseEvent
    {

        public static MiraiGroupCommand GetGroupHandlerInvoker(IMiraiHttpSession session, IGroupMessageEventArgs args, string message, long groupId, long memberId)
        {
            foreach (var invoker in HandlerInvokers.GroupCommandInvokers)
            {
                MiraiGroupCommand command = message.CheckCommand(invoker, session, args, groupId, memberId);
                if (command is not null) return command;
            }
            return null;
        }

        public static MiraiFriendCommand GetFriendHandlerInvoker(IMiraiHttpSession session, IFriendMessageEventArgs args, string message, long memberId)
        {
            foreach (var invoker in HandlerInvokers.FriendCommandInvokers)
            {
                MiraiFriendCommand command = message.CheckCommand(invoker, session, args, memberId);
                if (command is not null) return command;
            }
            return null;
        }


    }
}
