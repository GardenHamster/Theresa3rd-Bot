using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;
using EleCho.GoCqHttpSdk.Post;
using TheresaBot.GoCqHttp.Command;
using TheresaBot.GoCqHttp.Common;
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
                if (!BusinessHelper.IsHandleMessage(groupId)) return;
                if (memberId == CQConfig.BotQQ) return;
                if (memberId.IsBanMember()) return; //黑名单成员

                List<string> plainList = args.Message.OfType<CqTextMsg>().Select(m => m.Text.Trim()).Where(o => !string.IsNullOrEmpty(o)).ToList();

                string instruction = plainList.FirstOrDefault()?.Trim() ?? "";
                string message = args.Message.Text;

                string prefix = prefix = instruction.MatchPrefix();
                bool isAt = args.Message.Any(v => v is CqAtMsg atMsg && atMsg.Target == CQConfig.BotQQ);
                bool isInstruct = prefix.Length > 0 || BotConfig.GeneralConfig.Prefixs.Count == 0;//可以不设置任何指令前缀
                if (isInstruct) instruction = instruction.Remove(0, prefix.Length).Trim();

                if (args.Message.Any(v => v is CqReplyMsg))//引用指令
                {
                    GroupQuoteCommand quoteCommand = GetGroupQuoteCommand(args, instruction, prefix);
                    if (quoteCommand is not null) await quoteCommand.InvokeAsync(baseSession, baseReporter);
                    return;
                }

                if (isAt == false && isInstruct == false)//复读,分步操作,保存消息记录
                {
                    CQGroupRelay relay = new CQGroupRelay(args, msgId, message, groupId, memberId);
                    if (ProcessCache.HandleStep(relay, groupId, memberId)) return; //分步处理
                    if (RepeatCache.CheckCanRepeat(groupId, CQConfig.BotQQ, memberId, GetSimpleSendContent(args))) await SendRepeat(session, args);//复读机
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
