using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;
using EleCho.GoCqHttpSdk.Post;
using TheresaBot.GoCqHttp.Helper;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Config;

namespace TheresaBot.GoCqHttp.Plugin
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
                long memberId = args.UserId;
                long groupId = args.GroupId;
                if (args.Session is not ICqActionSession session) return;
                if (groupId.IsAuthorized() == false) return;
                if (memberId == BotConfig.BotQQ) return;
                WelcomeConfig welcomeConfig = BotConfig.WelcomeConfig;
                if (welcomeConfig is null || welcomeConfig.Enable == false) return;
                string template = welcomeConfig.Template;
                WelcomeSpecial welcomeSpecial = welcomeConfig.Specials?.Where(m => m.Groups.Contains(groupId)).FirstOrDefault();
                if (welcomeSpecial != null) template = welcomeSpecial.Template;
                if (string.IsNullOrEmpty(template)) return;
                List<CqMsg> welcomeMsgs = new List<CqMsg>();
                welcomeMsgs.Add(new CqAtMsg(memberId));
                welcomeMsgs.Add(new CqTextMsg("\r\n"));
                welcomeMsgs.AddRange(template.SplitToChainAsync().ToCQMessageAsync());
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
