using Mirai.CSharp.HttpApi.Models.EventArgs;
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

        public MiraiGroupCommand GetGroupCommand(IGroupMessageEventArgs args, string instruction)
        {
            foreach (var invoker in HandlerInvokers.GroupCommands)
            {
                string commandStr = instruction.CheckCommand(invoker);
                if (string.IsNullOrWhiteSpace(commandStr)) continue;
                return new MiraiGroupCommand(baseSession, invoker, args, instruction, commandStr);
            }
            return null;
        }

        public MiraiFriendCommand GetFriendCommand(IFriendMessageEventArgs args, string instruction)
        {
            foreach (var invoker in HandlerInvokers.FriendCommands)
            {
                string commandStr = instruction.CheckCommand(invoker);
                if (string.IsNullOrWhiteSpace(commandStr)) continue;
                return new MiraiFriendCommand(baseSession, invoker, args, instruction, commandStr);
            }
            return null;
        }

        public MiraiGroupQuoteCommand GetGroupQuoteCommand(IGroupMessageEventArgs args, string instruction)
        {
            foreach (var invoker in HandlerInvokers.GroupQuoteCommands)
            {
                string commandStr = instruction.CheckCommand(invoker);
                if (string.IsNullOrWhiteSpace(commandStr)) continue;
                return new MiraiGroupQuoteCommand(baseSession, invoker, args, instruction, commandStr);
            }
            return null;
        }

    }
}
