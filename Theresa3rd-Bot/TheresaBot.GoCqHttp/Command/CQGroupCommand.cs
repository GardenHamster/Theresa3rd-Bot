using EleCho.GoCqHttpSdk.Message;
using EleCho.GoCqHttpSdk.Post;
using TheresaBot.Main.Command;
using TheresaBot.Main.Model.Invoker;
using TheresaBot.Main.Session;

namespace TheresaBot.GoCqHttp.Command
{
    public class CQGroupCommand : GroupCommand
    {
        private CqGroupMessagePostContext Args { get; init; }

        public override long MsgId => Args.MessageId;

        public override long GroupId => Args.GroupId;

        public override long MemberId => Args.Sender.UserId;

        public CQGroupCommand(BaseSession baseSession, CommandHandler<GroupCommand> invoker, CqGroupMessagePostContext args, string instruction, string command)
            : base(baseSession, invoker, instruction, command)
        {
            Args = args;
        }

        public override List<string> GetImageUrls()
        {
            return Args.Message.OfType<CqImageMsg>().Select(o => o.Image).ToList();
        }

        public override long GetQuoteMessageId()
        {
            return Args.Message.OfType<CqReplyMsg>().FirstOrDefault()?.Id ?? 0;
        }

        public override async Task Test()
        {
            await Task.CompletedTask;
        }


    }
}
