using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using TheresaBot.Main.Invoker;
using TheresaBot.MiraiHttpApi.Command;
using TheresaBot.MiraiHttpApi.Helper;

namespace TheresaBot.MiraiHttpApi.Event
{
    public abstract class BaseEvent
    {
        public MiraiGroupCommand GetGroupCommand(IMiraiHttpSession session, IGroupMessageEventArgs args, string instruction)
        {
            foreach (var invoker in HandlerInvokers.GroupCommands)
            {
                MiraiGroupCommand command = instruction.CheckCommand(invoker, session, args);
                if (command is not null) return command;
            }
            return null;
        }

        public MiraiFriendCommand GetFriendCommand(IMiraiHttpSession session, IFriendMessageEventArgs args, string instruction)
        {
            foreach (var invoker in HandlerInvokers.FriendCommands)
            {
                MiraiFriendCommand command = instruction.CheckCommand(invoker, session, args);
                if (command is not null) return command;
            }
            return null;
        }

        public MiraiGroupQuoteCommand GetGroupQuoteCommand(IMiraiHttpSession session, IGroupMessageEventArgs args, string instruction)
        {
            foreach (var invoker in HandlerInvokers.GroupQuoteCommands)
            {
                MiraiGroupQuoteCommand command = instruction.CheckCommand(invoker, session, args);
                if (command is not null) return command;
            }
            return null;
        }

    }
}
