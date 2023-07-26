using EleCho.GoCqHttpSdk.Message;
using EleCho.GoCqHttpSdk.Post;
using TheresaBot.Main.Command;
using TheresaBot.Main.Model.Invoker;
using TheresaBot.Main.Session;

namespace TheresaBot.GoCqHttp.Command
{
    public class CQGroupQuoteCommand : GroupQuoteCommand
    {
        private CqGroupMessagePostContext Args { get; init; }

        public override long MessageId => Args.MessageId;

        public override long GroupId => Args.GroupId;

        public override long MemberId => Args.Sender.UserId;

        public CQGroupQuoteCommand(BaseSession baseSession, CommandHandler<GroupQuoteCommand> invoker, CqGroupMessagePostContext args, string instruction, string command)
            : base(baseSession, invoker, instruction, command)
        {
            Args = args;
        }

        public override List<string> GetImageUrls()
        {
            return Args.Message.OfType<CqImageMsg>().Select(o => o.Url?.ToString()).Where(o => !string.IsNullOrWhiteSpace(o)).ToList();
        }

        public override long GetQuoteMessageId()
        {
            return Args.Message.OfType<CqReplyMsg>().FirstOrDefault()?.Id ?? 0;
        }

    }
}
