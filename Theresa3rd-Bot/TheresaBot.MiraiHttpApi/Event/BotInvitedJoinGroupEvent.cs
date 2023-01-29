using Mirai.CSharp.HttpApi.Handlers;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Parsers;
using Mirai.CSharp.HttpApi.Parsers.Attributes;
using Mirai.CSharp.HttpApi.Session;
using System.Threading.Tasks;

namespace TheresaBot.MiraiHttpApi.Event
{
    [RegisterMiraiHttpParser(typeof(DefaultMappableMiraiHttpMessageParser<IBotInvitedJoinGroupEventArgs, BotInvitedJoinGroupEventArgs>))]
    public class BotInvitedJoinGroupEvent : MiraiHttpMessageHandler<IBotInvitedJoinGroupEventArgs>
    {
        public override Task HandleMessageAsync(IMiraiHttpSession client, IBotInvitedJoinGroupEventArgs message)
        {
            return Task.CompletedTask;
        }
    }
}
