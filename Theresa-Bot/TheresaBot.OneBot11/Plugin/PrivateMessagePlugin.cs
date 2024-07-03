using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;
using EleCho.GoCqHttpSdk.Post;
using TheresaBot.OneBot11.Relay;
using TheresaBot.Core.Cache;
using TheresaBot.Core.Common;
using TheresaBot.Core.Helper;

namespace TheresaBot.OneBot11.Plugin
{
    public class PrivateMessagePlugin : BasePlugin
    {

        public override async void OnPrivateMessageReceived(CqPrivateMessagePostContext args)
        {
            Task task = HandleMessageAsync(args);
            await Task.CompletedTask;
        }

        public async Task HandleMessageAsync(CqPrivateMessagePostContext args)
        {
            try
            {
                long msgId = args.MessageId;
                long memberId = args.Sender.UserId;
                if (memberId == BotConfig.BotQQ) return;
                if (memberId.IsBanMember()) return; //黑名单成员
                if (args.Session is not ICqActionSession session) return;

                var message = args.Message.Text;
                var plainList = args.Message.OfType<CqTextMsg>().Where(o => string.IsNullOrWhiteSpace(o.Text) == false).Select(m => m.Text.Trim()).ToList();
                var instruction = plainList.FirstOrDefault()?.Trim() ?? string.Empty;
                var prefix = instruction.MatchPrefix();
                var isInstruct = prefix.Length > 0;
                if (prefix.Length > 0) instruction = instruction.Remove(0, prefix.Length).Trim();
                if (prefix.Length > 0) message = message.Remove(0, prefix.Length).Trim();

                var relay = new OBFriendRelay(args, message, isInstruct);
                if (GameCahce.HandleGameMessage(relay)) return; //处理游戏消息
                if (isInstruct == false && ProcessCache.HandleStep(relay)) return; //分步处理
                if (string.IsNullOrWhiteSpace(instruction)) return; //空指令

                var botCommand = GetFriendCommand(args, message, instruction, prefix);
                if (botCommand is not null)
                {
                    await botCommand.InvokeAsync(BaseSession, baseReporter);
                    return;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "私聊指令异常");
                await BaseSession.ReplyFriendErrorAsync(ex, args.Sender.UserId);
                await Task.Delay(1000);
                await baseReporter.SendError(ex, "私聊指令异常");
            }
        }




    }
}
