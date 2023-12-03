using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Parsers.Attributes;
using Mirai.CSharp.HttpApi.Parsers;
using Mirai.CSharp.HttpApi.Handlers;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Models;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;

namespace TheresaBot.MiraiHttpApi.Event
{
    [RegisterMiraiHttpParser(typeof(DefaultMappableMiraiHttpMessageParser<INewFriendApplyEventArgs, NewFriendApplyEventArgs>))]
    public class FriendApplyEvent : BaseEvent, IMiraiHttpMessageHandler<INewFriendApplyEventArgs>
    {
        public async Task HandleMessageAsync(IMiraiHttpSession session, INewFriendApplyEventArgs args)
        {
            try
            {
                long memberId = args.FromQQ;
                if (memberId == BotConfig.BotQQ) return;
                if (memberId.IsBanMember()) return; //黑名单成员
                if (BotConfig.GeneralConfig.AcceptFriendRequest == false) return;
                await Task.Delay(3000);
                await session.HandleNewFriendApplyAsync(args, FriendApplyAction.Allow);
                args.BlockRemainingHandlers = false;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "好友申请事件异常");
                await baseReporter.SendError(ex, "好友申请事件异常");
            }
        }

    }
}
