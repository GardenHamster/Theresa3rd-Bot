using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheresaBot.Main.Command;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Invoker;
using TheresaBot.MiraiHttpApi.Helper;

namespace TheresaBot.MiraiHttpApi.Command
{
    public class MiraiFriendCommand : FriendCommand
    {
        public IFriendMessageEventArgs Args { get; set; }
        public IMiraiHttpSession Session { get; set; }

        public MiraiFriendCommand(CommandHandler<FriendCommand> invoker, IMiraiHttpSession session, IFriendMessageEventArgs args, string instruction, string command, long memberId)
            : base(invoker, args.GetMessageId(), instruction, command, memberId)
        {
            this.Args = args;
            this.Session = session;
        }

        public override List<string> GetImageUrls()
        {
            return Args.Chain.Where(o => o is ImageMessage).Select(o => ((ImageMessage)o).Url).ToList();
        }

        public override async Task<int> ReplyFriendTemplateAsync(string template, string defaultmsg)
        {
            if (string.IsNullOrWhiteSpace(template)) template = defaultmsg;
            if (string.IsNullOrWhiteSpace(template)) return 0;
            IChatMessage[] msgList = await template.SplitToChainAsync().ToMiraiMessageAsync();
            return await Session.SendFriendMessageAsync(MemberId, msgList);
        }

        public override async Task<int> ReplyFriendMessageAsync(string message)
        {
            return await Session.SendFriendMessageAsync(MemberId, new PlainMessage(message));
        }

        public override async Task<int> ReplyFriendMessageAsync(List<BaseContent> contents)
        {
            IChatMessage[] msgList = await contents.ToMiraiMessageAsync();
            return await Session.SendFriendMessageAsync(MemberId, msgList);
        }

    }
}
