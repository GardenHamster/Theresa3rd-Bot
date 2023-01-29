using System.Threading.Tasks;
using Theresa3rd_Bot.Model.Invoker;

namespace Theresa3rd_Bot.BotPlatform.Base.Command
{
    public abstract class FriendCommand : BaseCommand
    {
        public CommandHandler<FriendCommand> HandlerInvoker { get; init; }

        public FriendCommand(CommandHandler<FriendCommand> invoker, string[] keyWords, string instruction, long memberId) : base(keyWords, invoker.CommandType, instruction,memberId)
        {
            this.HandlerInvoker = invoker;
            this.MemberId = memberId;
        }

        public abstract Task<int> ReplyFriendTemplateAsync(string template, string defaultmsg);

        public override async Task<bool> InvokeAsync()
        {
            return await HandlerInvoker.HandleMethod.Invoke(this);
        }

    }
}
