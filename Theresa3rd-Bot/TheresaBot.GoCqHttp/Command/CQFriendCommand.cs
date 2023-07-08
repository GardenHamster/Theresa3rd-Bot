using EleCho.GoCqHttpSdk.Message;
using EleCho.GoCqHttpSdk.Post;
using TheresaBot.Main.Command;
using TheresaBot.Main.Model.Invoker;
using TheresaBot.Main.Session;

namespace TheresaBot.GoCqHttp.Command
{
    public class CQFriendCommand : FriendCommand
    {
        private CqPrivateMessagePostContext Args { get; init; }

        public override long MsgId => Args.MessageId;

        public override long MemberId => Args.Sender.UserId;

        public CQFriendCommand(BaseSession baseSession, CommandHandler<FriendCommand> invoker, CqPrivateMessagePostContext args, string instruction, string command)
            : base(baseSession, invoker, instruction, command)
        {
            Args = args;
        }

        public override List<string> GetImageUrls()
        {
            return Args.Message.OfType<CqImageMsg>().Select(o => o.Image).ToList();
        }


    }
}
