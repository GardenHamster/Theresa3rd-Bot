using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Models;
using TheresaBot.Main.Command;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Invoker;
using TheresaBot.Main.Result;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;
using TheresaBot.MiraiHttpApi.Helper;
using TheresaBot.MiraiHttpApi.Result;

namespace TheresaBot.MiraiHttpApi.Command
{
    public class MiraiFriendCommand : FriendCommand
    {
        private IFriendMessageEventArgs Args { get; init; }

        private IMiraiHttpSession MiraiSession { get; init; }

        public override PlatformType PlatformType { get; } = PlatformType.Mirai;

        public override long MsgId => Args.GetMessageId();

        public override long MemberId => Args.Sender.Id;

        public MiraiFriendCommand(BaseSession baseSession, CommandHandler<FriendCommand> invoker, IMiraiHttpSession miariSession, IFriendMessageEventArgs args, string instruction, string command)
            : base(baseSession, invoker, instruction, command)
        {
            this.Args = args;
            this.MiraiSession = miariSession;
        }

        public override List<string> GetImageUrls()
        {
            return Args.Chain.OfType<ImageMessage>().Select(o => o.Url).ToList();
        }




    }
}
