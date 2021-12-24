using Mirai.CSharp.Handlers;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Event
{
    public class GroupMessageEvent : IMiraiMessageHandler<IMiraiSession, IGroupMessageEventArgs>
    {
        public Task HandleMessageAsync(IMiraiSession client, IGroupMessageEventArgs message)
        {
            throw new NotImplementedException();
        }


    }
}
