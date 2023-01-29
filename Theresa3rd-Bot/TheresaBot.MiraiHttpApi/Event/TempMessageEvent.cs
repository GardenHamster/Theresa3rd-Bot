using Mirai.CSharp.HttpApi.Handlers;
using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Parsers;
using Mirai.CSharp.HttpApi.Parsers.Attributes;
using Mirai.CSharp.HttpApi.Session;
using System.Threading.Tasks;
using TheresaBot.Main.Common;

namespace TheresaBot.MiraiHttpApi.Event
{
    [RegisterMiraiHttpParser(typeof(DefaultMappableMiraiHttpMessageParser<ITempMessageEventArgs, TempMessageEventArgs>))]
    public class TempMessageEvent : IMiraiHttpMessageHandler<ITempMessageEventArgs>
    {
        public async Task HandleMessageAsync(IMiraiHttpSession session, ITempMessageEventArgs args)
        {
            long memberId = args.Sender.Id;
            if (memberId == BotConfig.MiraiConfig.BotQQ) return;
            await session.SendTempMessageAsync(args.Sender.Id, args.Sender.Group.Id, new PlainMessage("٩(๑òωó๑)۶"));
        }

    }
}
