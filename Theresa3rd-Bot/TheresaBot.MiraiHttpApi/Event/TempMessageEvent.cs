using Mirai.CSharp.HttpApi.Handlers;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Parsers;
using Mirai.CSharp.HttpApi.Parsers.Attributes;
using Mirai.CSharp.HttpApi.Session;

namespace TheresaBot.MiraiHttpApi.Event
{
    [RegisterMiraiHttpParser(typeof(DefaultMappableMiraiHttpMessageParser<ITempMessageEventArgs, TempMessageEventArgs>))]
    public class TempMessageEvent : IMiraiHttpMessageHandler<ITempMessageEventArgs>
    {
        public Task HandleMessageAsync(IMiraiHttpSession session, ITempMessageEventArgs args)
        {
            return Task.CompletedTask;
        }

    }
}
