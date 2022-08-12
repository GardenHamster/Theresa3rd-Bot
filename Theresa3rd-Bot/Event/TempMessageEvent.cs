using Mirai.CSharp.HttpApi.Handlers;
using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Parsers;
using Mirai.CSharp.HttpApi.Parsers.Attributes;
using Mirai.CSharp.HttpApi.Session;
using System;
using System.Threading.Tasks;
using Theresa3rd_Bot.Business;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Model.PO;
using Theresa3rd_Bot.Type;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Event
{
    [RegisterMiraiHttpParser(typeof(DefaultMappableMiraiHttpMessageParser<ITempMessageEventArgs, TempMessageEventArgs>))]
    public class TempMessageEvent : IMiraiHttpMessageHandler<ITempMessageEventArgs>
    {
        public async Task HandleMessageAsync(IMiraiHttpSession session, ITempMessageEventArgs args)
        {
            await Task.Delay(1000);
            await session.SendTempMessageAsync(args.Sender.Id, args.Sender.Group.Id, new PlainMessage("٩(๑òωó๑)۶"));
        }

    }
}
