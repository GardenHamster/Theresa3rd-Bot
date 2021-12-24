using Mirai.CSharp.HttpApi.Handlers;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Session;
using System;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Event
{
    public class GroupMessageEvent : IMiraiHttpMessageHandler<IGroupMessageEventArgs>
    {
        public Task HandleMessageAsync(IMiraiHttpSession client, IGroupMessageEventArgs message)
        {
            throw new NotImplementedException();
        }


    }
}
