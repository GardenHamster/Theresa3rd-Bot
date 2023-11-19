using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using TheresaBot.Main.Relay;

namespace TheresaBot.MiraiHttpApi.Relay
{
    public class MiraiGroupRelay : GroupRelay
    {
        public IGroupMessageEventArgs Args { get; set; }

        public override long QuoteMsgId => Args.Chain.OfType<QuoteMessage>().FirstOrDefault()?.Id ?? 0;

        public MiraiGroupRelay(IGroupMessageEventArgs args, long msgId, string message, long groupId, long memberId, bool isAt, bool isQuote, bool isInstruct)
            : base(msgId, message, groupId, memberId, isAt, isQuote, isInstruct)
        {
            this.Args = args;
        }

        public override List<string> GetImageUrls()
        {
            return Args.Chain.Where(o => o is ImageMessage).Select(o => ((ImageMessage)o).Url).Where(o => !string.IsNullOrWhiteSpace(o)).ToList();
        }

    }
}
