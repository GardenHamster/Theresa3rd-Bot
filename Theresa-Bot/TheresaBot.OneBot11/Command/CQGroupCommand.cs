using EleCho.GoCqHttpSdk.Message;
using EleCho.GoCqHttpSdk.Post;
using TheresaBot.Core.Command;
using TheresaBot.Core.Model.Invoker;
using TheresaBot.Core.Session;

namespace TheresaBot.OneBot11.Command
{
    public class CQGroupCommand : GroupCommand
    {
        private CqGroupMessagePostContext Args { get; init; }

        public override long MessageId => Args.MessageId;

        public override long GroupId => Args.GroupId;

        public override long MemberId => Args.Sender.UserId;

        public override string MemberName => Args.Sender.Nickname;

        public override string MemberNick => Args.Sender.Nickname;

        public CQGroupCommand(BaseSession baseSession, CommandHandler<GroupCommand> invoker, CqGroupMessagePostContext args, string instruction, string command, string prefix)
            : base(baseSession, invoker, instruction, command, prefix)
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

        public override async Task Test(GroupCommand command)
        {
            await Task.CompletedTask;
        }


    }
}
