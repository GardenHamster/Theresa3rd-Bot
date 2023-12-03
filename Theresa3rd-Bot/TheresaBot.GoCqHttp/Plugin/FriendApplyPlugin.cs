using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Post;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;

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
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "好友申请事件异常");
                await baseReporter.SendError(ex, "好友申请事件异常");
            }
        }


    }
}
