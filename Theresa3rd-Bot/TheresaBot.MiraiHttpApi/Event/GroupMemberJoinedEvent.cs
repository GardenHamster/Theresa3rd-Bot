using Mirai.CSharp.HttpApi.Handlers;
using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Parsers;
using Mirai.CSharp.HttpApi.Parsers.Attributes;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Type;
using TheresaBot.MiraiHttpApi.Common;
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
            if (memberId == MiraiConfig.MiraiBotQQ) return;

            WelcomeConfig welcomeConfig = BotConfig.WelcomeConfig;
            if (welcomeConfig is null || welcomeConfig.Enable == false) return;
            string template = welcomeConfig.Template;
            WelcomeSpecial welcomeSpecial = welcomeConfig.Special?.Where(m => m.GroupId == groupId).FirstOrDefault();
            if (welcomeSpecial != null) template = welcomeSpecial.Template;
            if (string.IsNullOrEmpty(template)) return;
            List<IChatMessage> welcomeMsgs = new List<IChatMessage>();
            welcomeMsgs.Add(new AtMessage(memberId));
            welcomeMsgs.Add(new PlainMessage("\r\n"));
            welcomeMsgs.AddRange(await template.SplitToChainAsync().ToMiraiMessageAsync(UploadTarget.Group));
            await session.SendGroupMessageAsync(groupId, welcomeMsgs.ToArray());
            message.BlockRemainingHandlers = true;
        }
    }
}
