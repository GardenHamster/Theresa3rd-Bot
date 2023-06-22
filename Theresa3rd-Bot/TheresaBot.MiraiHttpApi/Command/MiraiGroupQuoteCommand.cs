using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using TheresaBot.Main.Command;
using TheresaBot.Main.Model.Invoker;

namespace TheresaBot.MiraiHttpApi.Command
{
    public class MiraiGroupQuoteCommand : MiraiGroupCommand
    {
        public MiraiGroupQuoteCommand(CommandHandler<GroupQuoteCommand> invoker, IMiraiHttpSession session, IGroupMessageEventArgs args, string instruction, string command, long groupId, long memberId)
            : base(invoker.CommandType, session, args, instruction, command, groupId, memberId)
        {
        }

    }
}
