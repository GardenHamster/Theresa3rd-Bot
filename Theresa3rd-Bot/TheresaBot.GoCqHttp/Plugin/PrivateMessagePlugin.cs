using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;
using EleCho.GoCqHttpSdk.Post;
using TheresaBot.GoCqHttp.Command;
using TheresaBot.GoCqHttp.Common;
using TheresaBot.GoCqHttp.Helper;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;

namespace TheresaBot.GoCqHttp.Plugin
{
    public class PrivateMessagePlugin : BasePlugin
    {
        public override async void OnPrivateMessageReceived(CqPrivateMessagePostContext args)
        {
            try
            {
                long msgId = args.MessageId;
                long memberId = args.Sender.UserId;
                if (args.Session is not ICqActionSession session) return;
                if (memberId == CQConfig.BotQQ) return;

                List<string> plainList = args.Message.OfType<CqTextMsg>().Select(m => m.ToString().Trim()).Where(o => !string.IsNullOrEmpty(o)).ToList();
                if (plainList is null || plainList.Count == 0) return;

                string instruction = plainList.FirstOrDefault()?.Trim() ?? "";
                string message = args.Message.Text;
                if (string.IsNullOrWhiteSpace(message)) return;//空消息
                if (string.IsNullOrWhiteSpace(instruction)) return;//空指令

                string prefix = prefix = instruction.MatchPrefix();
                bool isInstruct = prefix.Length > 0 || BotConfig.GeneralConfig.Prefixs.Count == 0;//可以不设置任何指令前缀
                if (isInstruct) instruction = instruction.Remove(0, prefix.Length).Trim();

                CQFriendCommand botCommand = GetFriendCommand(session, args, instruction);
                if (botCommand is not null)
                {
                    await botCommand.InvokeAsync(cqSession, cqReporter);
                    return;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "私聊指令异常");
                await ReplyFriendErrorAsync(ex, args);
                await Task.Delay(1000);
                await cqReporter.SendError(ex, "私聊指令异常");
            }
        }

        private async Task ReplyFriendErrorAsync(Exception exception, CqPrivateMessagePostContext args)
        {
            try
            {
                List<BaseContent> contents = exception.GetErrorContents();
                CqMsg[] msgList = contents.ToCQMessageAsync();
                await CQHelper.Session.SendPrivateMessageAsync(args.Sender.UserId, new CqMessage(msgList));
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "ReplyFriendErrorAsync异常");
            }
        }

    }
}
