using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;
using EleCho.GoCqHttpSdk.Post;
using TheresaBot.OneBot11.Helper;
using TheresaBot.Core.Common;
using TheresaBot.Core.Helper;

namespace TheresaBot.OneBot11.Plugin
{
    public class GroupMemberIncreasePlugin : BasePlugin
    {

        public override async Task OnGroupMemberIncreasedAsync(CqGroupMemberIncreasedPostContext args)
        {
            Task task = HandlemessageAsync(args);
            await Task.CompletedTask;
        }

        public async Task HandlemessageAsync(CqGroupMemberIncreasedPostContext args)
        {
            try
            {
                var memberId = args.UserId;
                var groupId = args.GroupId;
                if (args.Session is not ICqActionSession session) return;
                if (groupId.IsAuthorized() == false) return;
                if (memberId == BotConfig.BotQQ) return;
                var welcomeConfig = BotConfig.WelcomeConfig;
                if (welcomeConfig.Enable == false) return;
                var template = welcomeConfig.Template;
                var welcomeSpecial = welcomeConfig.GetSpecial(groupId);
                if (welcomeSpecial is not null) template = welcomeSpecial.Template;
                if (string.IsNullOrWhiteSpace(template)) return;
                var welcomeMsgs = new List<CqMsg>
                {
                    new CqAtMsg(memberId),
                    new CqTextMsg("\r\n")
                };
                welcomeMsgs.AddRange(template.SplitToChainAsync().ToOBMessageAsync());
                await session.SendGroupMessageAsync(groupId, new CqMessage(welcomeMsgs));
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "入群事件异常");
                await baseReporter.SendError(ex, "入群事件异常");
            }
        }





    }
}
