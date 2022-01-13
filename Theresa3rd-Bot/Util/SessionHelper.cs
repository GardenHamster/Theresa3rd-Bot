using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Util
{
    public static class SessionHelper
    {
        public static async Task SendMessageWithAtAsync(this IMiraiHttpSession session, GroupMessageEventArgs args, params IChatMessage[] chain)
        {
            List<IChatMessage> chailList = new List<IChatMessage>(chain);
            chailList.Insert(0, new AtMessage(args.Sender.Id, ""));
            await session.SendGroupMessageAsync(args.Sender.Group.Id, chailList.ToArray());
        }

    }
}
