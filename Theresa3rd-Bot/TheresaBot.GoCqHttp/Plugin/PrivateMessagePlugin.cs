using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;
using EleCho.GoCqHttpSdk.Post;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TheresaBot.GoCqHttp.Command;
using TheresaBot.GoCqHttp.Relay;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;

namespace TheresaBot.GoCqHttp.Plugin
{
    public class PrivateMessagePlugin : BasePlugin
    {

        public override async void OnPrivateMessageReceived(CqPrivateMessagePostContext args)
        {
            Task task = HandlemessageAsync(args);
            await Task.CompletedTask;
        }

        public async Task HandlemessageAsync(CqPrivateMessagePostContext args)
        {
            try
            {
                long msgId = args.MessageId;
                long memberId = args.Sender.UserId;
                if (args.Session is not ICqActionSession session) return;
                if (memberId == BotConfig.BotQQ) return;

                List<string> plainList = args.Message.OfType<CqTextMsg>().Select(m => m.Text.Trim()).Where(o => !string.IsNullOrEmpty(o)).ToList();
                if (plainList is null || plainList.Count == 0) return;

                string instruction = plainList.FirstOrDefault()?.Trim() ?? "";
                string message = args.Message.Text;
                if (string.IsNullOrWhiteSpace(message)) return;//空消息
                if (string.IsNullOrWhiteSpace(instruction)) return;//空指令

                string prefix = prefix = instruction.MatchPrefix();
                bool isInstruct = prefix.Length > 0 || BotConfig.GeneralConfig.Prefixs.Count == 0;//可以不设置任何指令前缀
                var relay = new CQFriendRelay(args, msgId, message, memberId, isInstruct);

                if (isInstruct) instruction = instruction.Remove(0, prefix.Length).Trim();
                if (ProcessCache.HandleStep(relay)) return; //分步处理

                CQFriendCommand botCommand = GetFriendCommand(args, instruction, prefix);
                if (botCommand is not null)
                {
                    await botCommand.InvokeAsync(baseSession, baseReporter);
                    return;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "私聊指令异常");
                await baseSession.ReplyFriendErrorAsync(ex, args.Sender.UserId);
                await Task.Delay(1000);
                await baseReporter.SendError(ex, "私聊指令异常");
            }
        }




    }
}
