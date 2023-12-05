using Mirai.CSharp.HttpApi.Handlers;
using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Parsers;
using Mirai.CSharp.HttpApi.Parsers.Attributes;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Models;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Config;
using TheresaBot.MiraiHttpApi.Helper;

namespace TheresaBot.MiraiHttpApi.Event
{
    [RegisterMiraiHttpParser(typeof(DefaultMappableMiraiHttpMessageParser<IGroupMemberJoinedEventArgs, GroupMemberJoinedEventArgs>))]
    public class GroupMemberJoinedEvent : BaseEvent, IMiraiHttpMessageHandler<IGroupMemberJoinedEventArgs>
    {
        public async Task HandleMessageAsync(IMiraiHttpSession session, IGroupMemberJoinedEventArgs message)
        {
            try
            {
                long memberId = message.Member.Id;
                long groupId = message.Member.Group.Id;
                if (groupId.IsAuthorized() == false) return;
                if (memberId == BotConfig.BotQQ) return;
                WelcomeConfig welcomeConfig = BotConfig.WelcomeConfig;
                if (welcomeConfig is null || welcomeConfig.Enable == false) return;
                string template = welcomeConfig.Template;
                WelcomeSpecial welcomeSpecial = welcomeConfig.GetSpecial(groupId);
                if (welcomeSpecial is not null) template = welcomeSpecial.Template;
                if (string.IsNullOrEmpty(template)) return;
                List<IChatMessage> welcomeMsgs = new List<IChatMessage>();
                welcomeMsgs.Add(new AtMessage(memberId));
                welcomeMsgs.Add(new PlainMessage("\r\n"));
                welcomeMsgs.AddRange(await template.SplitToChainAsync().ToMiraiMessageAsync(UploadTarget.Group));
                await session.SendGroupMessageAsync(groupId, welcomeMsgs.ToArray());
                message.BlockRemainingHandlers = true;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "入群事件异常");
                await BaseReporter.SendError(ex, "入群事件异常");
            }
        }

    }
}
