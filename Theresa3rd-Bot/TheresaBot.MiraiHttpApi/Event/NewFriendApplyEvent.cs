using Mirai.CSharp.HttpApi.Handlers;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Parsers;
using Mirai.CSharp.HttpApi.Parsers.Attributes;
using Mirai.CSharp.HttpApi.Session;
using System.Threading.Tasks;

namespace TheresaBot.MiraiHttpApi.Event
{
    [RegisterMiraiHttpParser(typeof(DefaultMappableMiraiHttpMessageParser<INewFriendApplyEventArgs, NewFriendApplyEventArgs>))]
    public class NewFriendApplyEvent : IMiraiHttpMessageHandler<INewFriendApplyEventArgs>
    {
        public Task HandleMessageAsync(IMiraiHttpSession client, INewFriendApplyEventArgs message)
        {
            return Task.CompletedTask;
        }
    }
}
