using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Util
{
    public static class SessionHelper
    {
        public static async Task<int> SendMessageWithAtAsync(this IMiraiHttpSession session, IGroupMessageEventArgs args, params IChatMessage[] chain)
        {
            List<IChatMessage> chailList = new List<IChatMessage>(chain);
            chailList.Insert(0, new AtMessage(args.Sender.Id, ""));
            return await session.SendGroupMessageAsync(args.Sender.Group.Id, chailList.ToArray());
        }

        public static async Task<int> SendMessageWithAtAsync(this IMiraiHttpSession session, IGroupMessageEventArgs args,List<IChatMessage> chailList)
        {
            chailList.Insert(0, new AtMessage(args.Sender.Id, ""));
            return await session.SendGroupMessageAsync(args.Sender.Group.Id, chailList.ToArray());
        }

        public static async Task<int> SendTemplateWithAtAsync(this IMiraiHttpSession session, IGroupMessageEventArgs args, string template, string defaultmsg)
        {
            if (string.IsNullOrWhiteSpace(template)) template = defaultmsg;
            List<IChatMessage> chatList = session.SplitToChainAsync(template).Result;
            return await session.SendMessageWithAtAsync(args, chatList);
        }

    }
}
