using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Models;
using TheresaBot.Main.Command;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Invoker;
using TheresaBot.Main.Result;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;
using TheresaBot.MiraiHttpApi.Helper;
using TheresaBot.MiraiHttpApi.Result;

namespace TheresaBot.MiraiHttpApi.Command
{
    public class MiraiGroupCommand : GroupCommand
    {
        private IMiraiHttpSession MiraiSession { get; init; }

        private IGroupMessageEventArgs Args { get; init; }

        public override PlatformType PlatformType { get; } = PlatformType.Mirai;

        public override long MsgId => Args.GetMessageId();

        public override long GroupId => Args.Sender.Group.Id;

        public override long MemberId => Args.Sender.Id;

        public MiraiGroupCommand(BaseSession baseSession, IMiraiHttpSession miariSession, IGroupMessageEventArgs args, CommandType commandType, string instruction, string command)
            : base(baseSession, commandType, instruction, command)
        {
            this.Args = args;
            this.MiraiSession = miariSession;
        }

        public MiraiGroupCommand(BaseSession baseSession, CommandHandler<GroupCommand> invoker, IMiraiHttpSession miariSession, IGroupMessageEventArgs args, string instruction, string command)
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
