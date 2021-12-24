using Mirai.CSharp.HttpApi.Handlers;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using System;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Event
{
    public class DisconnectedEvent : IMiraiHttpMessageHandler<IDisconnectedEventArgs>
    {
        public Task HandleMessageAsync(IMiraiHttpSession client, IDisconnectedEventArgs message)
        {
            throw new NotImplementedException();
        }
    }
}
