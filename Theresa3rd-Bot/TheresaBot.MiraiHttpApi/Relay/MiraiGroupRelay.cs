using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using System.Collections.Generic;
using System.Linq;
using TheresaBot.Main.Relay;

namespace TheresaBot.MiraiHttpApi.Relay
{
    public class MiraiGroupRelay : GroupRelay
    {
        public IGroupMessageEventArgs Args { get; set; }

        public MiraiGroupRelay(IGroupMessageEventArgs args, int msgId, string message, long groupId, long memberId) : base(msgId, message, groupId, memberId)
        {
            this.Args = args;
        }

        public override List<string> GetReplyImageUrls()
        {
            return Args.Chain.Where(o => o is ImageMessage).Select(o => ((ImageMessage)o).Url).ToList();
        }

    }
}
