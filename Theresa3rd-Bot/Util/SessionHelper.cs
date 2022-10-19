using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Util
{
    public static class SessionHelper
    {
        public static async Task<int> SendMessageWithAtAsync(this IMiraiHttpSession session, IGroupMessageEventArgs args, params IChatMessage[] chainArr)
        {
            List<IChatMessage> msgList = new List<IChatMessage>();
            msgList.Add(new AtMessage(args.Sender.Id));
            msgList.AddRange(chainArr);
            return await session.SendGroupMessageAsync(args.Sender.Group.Id, msgList.ToArray());
        }

        public static async Task<int> SendMessageWithAtAsync(this IMiraiHttpSession session, IGroupMessageEventArgs args, List<IChatMessage> chainlList)
        {
            List<IChatMessage> msgList = new List<IChatMessage>();
            msgList.Add(new AtMessage(args.Sender.Id));
            msgList.AddRange(chainlList);
            return await session.SendGroupMessageAsync(args.Sender.Group.Id, msgList.ToArray());
        }

        public static async Task<int> SendTemplateWithAtAsync(this IMiraiHttpSession session, IGroupMessageEventArgs args, string template, string defaultmsg)
        {
            if (string.IsNullOrWhiteSpace(template)) template = defaultmsg;
            if (string.IsNullOrWhiteSpace(template)) return 0;
            List<IChatMessage> chatList = session.SplitToChainAsync(template).Result;
            return await session.SendMessageWithAtAsync(args, chatList);
        }

        public static async Task<int> SendTemplateAsync(this IMiraiHttpSession session, IFriendMessageEventArgs args, string template, string defaultmsg)
        {
            if (string.IsNullOrWhiteSpace(template)) template = defaultmsg;
            if (string.IsNullOrWhiteSpace(template)) return 0;
            List<IChatMessage> chatList = session.SplitToChainAsync(template).Result;
            return await session.SendFriendMessageAsync(args.Sender.Id, chatList.ToArray());
        }

        public static async Task RevokeMessageAsync(this IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            try
            {
                SourceMessage sourceMessage = (SourceMessage)args.Chain.First();
                await session.RevokeMessageAsync(sourceMessage.Id, args.Sender.Group.Id);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "群消息撤回失败");
            }
        }

    }
}
