using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;
using EleCho.GoCqHttpSdk.Post;
using TheresaBot.GoCqHttp.Helper;
using TheresaBot.GoCqHttp.Relay;
using TheresaBot.Core.Cache;
using TheresaBot.Core.Command;
using TheresaBot.Core.Common;
using TheresaBot.Core.Datas;
using TheresaBot.Core.Helper;
using TheresaBot.Core.Type;

namespace TheresaBot.GoCqHttp.Plugin
{
    public class GroupMessagePlugin : BasePlugin
    {

        public override async Task OnGroupMessageReceivedAsync(CqGroupMessagePostContext args)
        {
            Task task = HandlemessageAsync(args);
            await Task.CompletedTask;
        }

        private async Task HandlemessageAsync(CqGroupMessagePostContext args)
        {
            try
            {
                long msgId = args.MessageId;
                long groupId = args.GroupId;
                long memberId = args.Sender.UserId;
                if (groupId.IsAuthorized() == false) return;
                if (memberId == BotConfig.BotQQ) return;
                if (memberId.IsBanMember()) return; //黑名单成员
                if (args.Session is not ICqActionSession session) return;

                var message = args.Message.Text;
                var plainList = args.Message.OfType<CqTextMsg>().Where(o => string.IsNullOrWhiteSpace(o.Text) == false).Select(m => m.Text.Trim()).ToList();
                var instruction = plainList.FirstOrDefault()?.Trim() ?? string.Empty;
                var prefix = instruction.MatchPrefix();
                var isInstruct = prefix.Length > 0;
                var isAt = args.Message.Any(v => v is CqAtMsg atMsg && atMsg.Target == BotConfig.BotQQ);
                var isQuote = args.Message.Any(v => v is CqReplyMsg qtMsg) && isAt;
                if (prefix.Length > 0) instruction = instruction.Remove(0, prefix.Length).Trim();
                if (prefix.Length > 0) message = message.Remove(0, prefix.Length).Trim();

                var relay = new CQGroupRelay(args, message, isAt, isQuote, isInstruct);
                if (GameCahce.HandleGameMessage(relay)) return; //处理游戏消息

                if (args.Message.Any(v => v is CqReplyMsg))//引用指令
                {
                    GroupQuoteCommand quoteCommand = GetGroupQuoteCommand(args, instruction, prefix);
                    if (quoteCommand is not null) await quoteCommand.InvokeAsync(BaseSession, baseReporter);
                    return;
                }

                if (isAt == false && isInstruct == false)//复读,分步操作,保存消息记录
                {
                    if (ProcessCache.HandleStep(relay)) return; //分步处理
                    if (RepeatCache.CheckCanRepeat(groupId, BotConfig.BotQQ, memberId, GetSimpleSendContent(args))) await SendRepeat(session, args);//复读机
                    List<string> imgUrls = args.Message.OfType<CqImageMsg>().Select(o => o.Url?.ToString()).ToList();
                    Task task1 = RecordHelper.AddImageRecords(imgUrls, PlatformType.GoCQHttp, msgId, groupId, memberId);
                    List<string> plainMessages = args.Message.OfType<CqTextMsg>().Select(o => o.Text).ToList();
                    Task task2 = RecordHelper.AddMessageRecord(plainMessages, PlatformType.GoCQHttp, msgId, groupId, memberId);
                    return;
                }

                if (string.IsNullOrWhiteSpace(instruction))//空指令
                {
                    return;
                }

                var command = GetGroupCommand(args, instruction, prefix);
                if (command is not null)
                {
                    await command.InvokeAsync(BaseSession, baseReporter);
                    return;
                }

                if (BotConfig.GeneralConfig.SendRelevantCommands)
                {
                    await CQHelper.ReplyRelevantCommandsAsync(instruction, groupId, memberId);
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "群指令异常");
                await BaseSession.ReplyGroupErrorAsync(ex, args.GroupId);
                await Task.Delay(1000);
                await baseReporter.SendError(ex, "群指令异常");
            }
        }


        private async Task SendRepeat(ICqActionSession session, CqGroupMessagePostContext args)
        {
            try
            {
                await session.SendGroupMessageAsync(args.GroupId, args.Message);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "复读失败");
            }
        }

    }
}
