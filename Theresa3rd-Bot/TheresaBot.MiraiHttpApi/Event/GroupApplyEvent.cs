using Mirai.CSharp.HttpApi.Handlers;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Parsers;
using Mirai.CSharp.HttpApi.Parsers.Attributes;
using Mirai.CSharp.HttpApi.Session;

namespace TheresaBot.MiraiHttpApi.Event
{
    [RegisterMiraiHttpParser(typeof(DefaultMappableMiraiHttpMessageParser<IGroupApplyEventArgs, GroupApplyEventArgs>))]
    public class GroupApplyEvent : IMiraiHttpMessageHandler<IGroupApplyEventArgs>
    {
        public Task HandleMessageAsync(IMiraiHttpSession client, IGroupApplyEventArgs message)
        {
            return Task.CompletedTask;
        }
    }
}
