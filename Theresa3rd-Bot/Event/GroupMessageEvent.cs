using Mirai.CSharp.HttpApi.Handlers;
using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Parsers;
using Mirai.CSharp.HttpApi.Parsers.Attributes;
using Mirai.CSharp.HttpApi.Session;
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
            IChatMessage[] chain = new IChatMessage[]
            {
                new PlainMessage($"收到了来自{message.Sender.Name}[{message.Sender.Id}]{{{message.Sender.Permission}}}的群消息:{string.Join(null, (IEnumerable<IChatMessage>)message.Chain)}")
                //                          / 发送者群名片 /  / 发送者QQ号 /   /   发送者在群内权限   /                                                       / 消息链 /
                // 你还可以在这里边加入更多的 IMessageBase
            };
            await client.SendGroupMessageAsync(message.Sender.Group.Id, chain); // 向消息来源群异步发送由以上chain表示的消息
            message.BlockRemainingHandlers = false; // 不阻断消息传递。如需阻断请返回true
        }


    }
}
