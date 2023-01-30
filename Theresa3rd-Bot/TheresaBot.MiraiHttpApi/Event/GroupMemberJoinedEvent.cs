using Mirai.CSharp.HttpApi.Handlers;
using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Parsers;
using Mirai.CSharp.HttpApi.Parsers.Attributes;
using Mirai.CSharp.HttpApi.Session;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Type;
using TheresaBot.MiraiHttpApi.Helper;

namespace TheresaBot.MiraiHttpApi.Event
{
    [RegisterMiraiHttpParser(typeof(DefaultMappableMiraiHttpMessageParser<IGroupMemberJoinedEventArgs, GroupMemberJoinedEventArgs>))]
    public class GroupMemberJoinedEvent : IMiraiHttpMessageHandler<IGroupMemberJoinedEventArgs>
    {
        public async Task HandleMessageAsync(IMiraiHttpSession session, IGroupMemberJoinedEventArgs message)
        {
            long memberId = message.Member.Id;
            long groupId = message.Member.Group.Id;
            if (!BusinessHelper.IsHandleMessage(groupId)) return;
            if (memberId == BotConfig.MiraiConfig.BotQQ) return;

            WelcomeConfig welcomeConfig = BotConfig.WelcomeConfig;
            if (welcomeConfig is null || welcomeConfig.Enable == false) return;
            string template = welcomeConfig.Template;
            WelcomeSpecial welcomeSpecial = welcomeConfig.Special?.Where(m => m.GroupId == groupId).FirstOrDefault();
            if (welcomeSpecial != null) template = welcomeSpecial.Template;
            if (string.IsNullOrEmpty(template)) return;
            List<IChatMessage> templateList = await BusinessHelper.SplitToChainAsync(template, SendTarget.Group).ToMiraiMessageAsync();
            List<IChatMessage> atList = new List<IChatMessage>()
            {
                new AtMessage(memberId),new PlainMessage("\n")
            };
            List<IChatMessage> msgList = atList.Concat(templateList).ToList();
            await session.SendGroupMessageAsync(groupId, msgList.ToArray());
            message.BlockRemainingHandlers = true;
        }
    }
}
