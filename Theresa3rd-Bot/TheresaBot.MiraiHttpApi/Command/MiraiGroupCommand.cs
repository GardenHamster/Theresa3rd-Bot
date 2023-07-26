using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using TheresaBot.Main.Command;
using TheresaBot.Main.Model.Invoker;
using TheresaBot.Main.Session;
using TheresaBot.MiraiHttpApi.Helper;

namespace TheresaBot.MiraiHttpApi.Command
{
    public class MiraiGroupCommand : GroupCommand
    {
        private IGroupMessageEventArgs Args { get; init; }

        public override long MessageId => Args.GetMessageId();

        public override long GroupId => Args.Sender.Group.Id;

        public override long MemberId => Args.Sender.Id;

        public MiraiGroupCommand(BaseSession baseSession, CommandHandler<GroupCommand> invoker, IGroupMessageEventArgs args, string instruction, string command)
            : base(baseSession, invoker, instruction, command)
        {
            this.Args = args;
        }

        public override List<string> GetImageUrls()
        {
            return Args.Chain.OfType<ImageMessage>().Select(o => o.Url).Where(o => !string.IsNullOrWhiteSpace(o)).ToList();
        }

        public override long GetQuoteMessageId()
        {
            var quoteMessage = Args.Chain.Where(v => v is QuoteMessage).FirstOrDefault();
            return quoteMessage is null ? 0 : ((QuoteMessage)quoteMessage).Id;
        }

    }
}
