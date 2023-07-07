using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using TheresaBot.Main.Command;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Invoker;
using TheresaBot.Main.Result;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;
using TheresaBot.MiraiHttpApi.Helper;

namespace TheresaBot.MiraiHttpApi.Command
{
    public class MiraiGroupQuoteCommand : GroupQuoteCommand
    {
        private IMiraiHttpSession MiraiSession { get; init; }

        private IGroupMessageEventArgs Args { get; init; }

        public override PlatformType PlatformType { get; } = PlatformType.Mirai;

        public override long MsgId => Args.GetMessageId();

        public override long GroupId => Args.Sender.Group.Id;

        public override long MemberId => Args.Sender.Id;

        public MiraiGroupQuoteCommand(BaseSession baseSession, CommandHandler<GroupQuoteCommand> invoker, IMiraiHttpSession miariSession, IGroupMessageEventArgs args, string instruction, string command)
            : base(baseSession, invoker, instruction, command)
        {
            this.Args = args;
            this.MiraiSession = miariSession;
        }

        public override List<string> GetImageUrls()
        {
            return Args.Chain.OfType<ImageMessage>().Select(o => o.Url).ToList();
        }

        public override long GetQuoteMessageId()
        {
            var quoteMessage = Args.Chain.Where(v => v is QuoteMessage).FirstOrDefault();
            return quoteMessage is null ? 0 : ((QuoteMessage)quoteMessage).Id;
        }

    }
}
