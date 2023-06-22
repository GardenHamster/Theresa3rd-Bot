using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using System.Linq;
using TheresaBot.Main.Common;
using TheresaBot.Main.Invoker;
using TheresaBot.MiraiHttpApi.Command;
using TheresaBot.MiraiHttpApi.Helper;

namespace TheresaBot.MiraiHttpApi.Event
{
    public abstract class BaseEvent
    {
        public static MiraiGroupCommand GetGroupCommand(IMiraiHttpSession session, IGroupMessageEventArgs args, string message, long groupId, long memberId)
        {
            foreach (var invoker in HandlerInvokers.GroupCommands)
            {
                MiraiGroupCommand command = message.CheckCommand(invoker, session, args, groupId, memberId);
                if (command is not null) return command;
            }
            return null;
        }

        public static MiraiFriendCommand GetFriendCommand(IMiraiHttpSession session, IFriendMessageEventArgs args, string message, long memberId)
        {
            foreach (var invoker in HandlerInvokers.FriendCommands)
            {
                MiraiFriendCommand command = message.CheckCommand(invoker, session, args, memberId);
                if (command is not null) return command;
            }
            return null;
        }

        public static MiraiGroupQuoteCommand GetGroupQuoteCommand(IMiraiHttpSession session, IGroupMessageEventArgs args, string message, long groupId, long memberId)
        {
            foreach (var invoker in HandlerInvokers.GroupQuoteCommands)
            {
                MiraiGroupQuoteCommand command = message.CheckCommand(invoker, session, args, groupId, memberId);
                if (command is not null) return command;
            }
            return null;
        }

        /// <summary>
        /// 匹配对应的指令前缀
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string MatchPrefix(string message)
        {
            var prefixs = BotConfig.GeneralConfig.Prefixs;
            if (prefixs is null || prefixs.Count == 0) return string.Empty;
            message = message?.Trim() ?? string.Empty;
            var prefix = prefixs.Where(o => message.StartsWith(o)).FirstOrDefault();
            return string.IsNullOrWhiteSpace(prefix) ? string.Empty : prefix;
        }


    }
}
