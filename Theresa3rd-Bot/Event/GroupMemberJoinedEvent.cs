using Mirai.CSharp.HttpApi.Handlers;
using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Parsers;
using Mirai.CSharp.HttpApi.Parsers.Attributes;
using Mirai.CSharp.HttpApi.Session;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Model.Config;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Event
{
    [RegisterMiraiHttpParser(typeof(DefaultMappableMiraiHttpMessageParser<IGroupMemberJoinedEventArgs, GroupMemberJoinedEventArgs>))]
    public class GroupMemberJoinedEvent : IMiraiHttpMessageHandler<IGroupMemberJoinedEventArgs>
    {
        public async Task HandleMessageAsync(IMiraiHttpSession client, IGroupMemberJoinedEventArgs message)
        {
            long memberId = message.Member.Id;
            long groupId = message.Member.Group.Id;
            if (!BusinessHelper.IsHandleMessage(groupId)) return;

            WelcomeConfig welcomeConfig = BotConfig.WelcomeConfig;
            if (welcomeConfig == null || welcomeConfig.Enable == false) return;
            string template = welcomeConfig.Template;
            WelcomeSpecial welcomeSpecial = welcomeConfig.Special?.Where(m => m.GroupId == groupId).FirstOrDefault();
            if (welcomeSpecial != null) template = welcomeSpecial.Template;
            if (string.IsNullOrEmpty(template)) return;
            List<IChatMessage> atList = new List<IChatMessage>()
            {
                new AtMessage(memberId, ""),
                new PlainMessage("\n")
            };
            List<IChatMessage> templateList = BusinessHelper.SplitToChainAsync(client, template).Result;
            await client.SendGroupMessageAsync(groupId, atList.Concat(templateList).ToArray()); // 自己填群号, 一般由 IGroupMessageEventArgs 提供
            message.BlockRemainingHandlers = true; // 不阻断消息传递。如需阻断请返回true
        }
    }
}
