using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Post;
using TheresaBot.Core.Common;
using TheresaBot.Core.Helper;

namespace TheresaBot.GoCqHttp.Plugin
{
    public class FriendApplyPlugin : BasePlugin
    {
        public override async void OnFriendRequest(CqFriendRequestPostContext args)
        {
            Task task = HandleMessageAsync(args);
            await Task.CompletedTask;
        }

        public async Task HandleMessageAsync(CqFriendRequestPostContext args)
        {
            try
            {
                long memberId = args.UserId;
                if (args.Session is not ICqActionSession session) return;
                if (memberId == BotConfig.BotQQ) return;
                if (memberId.IsBanMember()) return; //黑名单成员
                if (BotConfig.GeneralConfig.AcceptFriendRequest == false) return;
                await Task.Delay(3000);
                await session.ApproveFriendRequestAsync(args.Flag, "");
                await Task.Delay(1000);
                var remindmsg = $"QQ用户{args.UserId}已添加Bot为好友";
                await BaseSession.SendFriendMessageAsync(BotConfig.SuperManagers, remindmsg);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "好友申请事件异常");
                await baseReporter.SendError(ex, "好友申请事件异常");
            }
        }


    }
}
