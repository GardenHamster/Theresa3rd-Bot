using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using TheresaBot.Core.Relay;
using TheresaBot.MiraiHttpApi.Helper;

namespace TheresaBot.MiraiHttpApi.Relay
{
    public class MiraiGroupRelay : GroupRelay
    {
        public IGroupMessageEventArgs Args { get; set; }

        public override long MsgId => Args.GetMessageId();

        public override long QuoteMsgId => Args.Chain.OfType<QuoteMessage>().FirstOrDefault()?.Id ?? 0;

        public override long GroupId => Args.Sender.Group.Id;

        public override long MemberId => Args.Sender.Id;

        public MiraiGroupRelay(IGroupMessageEventArgs args, string message, bool isAt, bool isQuote, bool isInstruct) : base(message, isAt, isQuote, isInstruct)
        {
            this.Args = args;
        }

        public override List<string> GetImageUrls()
        {
            return Args.Chain.OfType<ImageMessage>().Select(o => o.Url).Where(o => !string.IsNullOrWhiteSpace(o)).ToList();
        }

    }
}
