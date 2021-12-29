using Mirai.CSharp.HttpApi.Handlers;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Parsers;
using Mirai.CSharp.HttpApi.Parsers.Attributes;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Models;
using Mirai.CSharp.Models.ChatMessages;
using Mirai.CSharp.Session;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Event
{
    [RegisterMiraiHttpParser(typeof(DefaultMappableMiraiHttpMessageParser<IGroupMessageEventArgs, GroupMessageEventArgs>))]
    public class GroupMessageEvent : IMiraiHttpMessageHandler<IGroupMessageEventArgs>
    {
        public async Task HandleMessageAsync(IMiraiHttpSession client, IGroupMessageEventArgs message)
        {
            IImageMessage msg = await client.UploadPictureAsync(UploadTarget.Group, "D:\\other\\2c75dbfe43b28a55.jpg");
            IChatMessage[] chain = new IChatMessage[] { new Mirai.CSharp.HttpApi.Models.ChatMessages.PlainMessage("[mirai:atall]emmmmm"), msg }; // 数组里边可以加上更多的 IMessageBase, 以此达到例如图文并发的情况
            await client.SendGroupMessageAsync(message.Sender.Group.Id, chain); // 自己填群号, 一般由 IGroupMessageEventArgs 提供
            message.BlockRemainingHandlers = false; // 不阻断消息传递。如需阻断请返回true
        }


    }
}
