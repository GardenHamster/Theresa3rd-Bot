using System.Threading.Tasks;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Invoker;

namespace TheresaBot.Main.Command
{
    public abstract class FriendCommand : BaseCommand
    {
        public CommandHandler<FriendCommand> HandlerInvoker { get; init; }

        public FriendCommand(CommandHandler<FriendCommand> invoker, string[] keyWords, string instruction, long memberId) : base(keyWords, invoker.CommandType, instruction,memberId)
        {
            this.HandlerInvoker = invoker;
            this.MemberId = memberId;
        }

        public abstract Task<int> ReplyFriendMessageAsync(string message);

        public abstract Task<int> ReplyFriendMessageAsync(List<BaseContent> contents);

        public abstract Task<int> ReplyFriendTemplateAsync(string template, string defaultmsg);

        public override async Task<bool> InvokeAsync()
        {
            return await HandlerInvoker.HandleMethod.Invoke(this);
        }

    }
}
