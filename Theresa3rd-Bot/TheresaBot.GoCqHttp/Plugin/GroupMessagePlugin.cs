using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;
using EleCho.GoCqHttpSdk.Post;
using TheresaBot.GoCqHttp.Command;
using TheresaBot.GoCqHttp.Helper;
using TheresaBot.GoCqHttp.Relay;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Type;

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
                long memberId = args.Sender.UserId;
                long groupId = args.GroupId;
                if (args.Session is not ICqActionSession session) return;
                if (groupId.IsAuthorized() == false) return;
                if (memberId == BotConfig.BotQQ) return;
                if (memberId.IsBanMember()) return; //黑名单成员

                var message = args.Message.Text;
                var plainList = args.Message.OfType<CqTextMsg>().Select(m => m.Text.Trim()).Where(o => !string.IsNullOrEmpty(o)).ToList();
                var instruction = plainList.FirstOrDefault()?.Trim() ?? "";
                var prefix = instruction.MatchPrefix();
                var isAt = args.Message.Any(v => v is CqAtMsg atMsg && atMsg.Target == BotConfig.BotQQ);
                var isInstruct = prefix.Length > 0 || BotConfig.GeneralConfig.Prefixs.Count == 0;//可以不设置任何指令前缀
                var relay = new CQGroupRelay(args, msgId, message, groupId, memberId);

                if (isInstruct) instruction = instruction.Remove(0, prefix.Length).Trim();
                if (GameCahce.HandleGameMessage(relay)) return; //处理游戏消息
                if (ProcessCache.HandleStep(relay)) return; //分步处理

                if (args.Message.Any(v => v is CqReplyMsg))//引用指令
                {
                    GroupQuoteCommand quoteCommand = GetGroupQuoteCommand(args, instruction, prefix);
                    if (quoteCommand is not null) await quoteCommand.InvokeAsync(baseSession, baseReporter);
                    return;
                }

                if (isAt == false && isInstruct == false)//复读,分步操作,保存消息记录
                {
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

                CQGroupCommand command = GetGroupCommand(args, instruction, prefix);
                if (command is not null)
                {
                    await command.InvokeAsync(baseSession, baseReporter);
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
                await baseSession.ReplyGroupErrorAsync(ex, args.GroupId);
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
