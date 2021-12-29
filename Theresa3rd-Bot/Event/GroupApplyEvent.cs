using Mirai.CSharp.HttpApi.Handlers;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Parsers;
using Mirai.CSharp.HttpApi.Parsers.Attributes;
using Mirai.CSharp.HttpApi.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Event
{
    [RegisterMiraiHttpParser(typeof(DefaultMappableMiraiHttpMessageParser<IGroupApplyEventArgs, GroupApplyEventArgs>))]
    public class GroupApplyEvent : IMiraiHttpMessageHandler<IGroupApplyEventArgs>
    {
        public Task HandleMessageAsync(IMiraiHttpSession client, IGroupApplyEventArgs message)
        {
            throw new NotImplementedException();
        }
    }
}
