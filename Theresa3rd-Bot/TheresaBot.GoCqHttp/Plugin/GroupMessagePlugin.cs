using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;
using EleCho.GoCqHttpSdk.Post;
using TheresaBot.GoCqHttp.Command;
using TheresaBot.GoCqHttp.Common;
using TheresaBot.GoCqHttp.Helper;
using TheresaBot.GoCqHttp.Relay;
using TheresaBot.GoCqHttp.Reporter;
using TheresaBot.GoCqHttp.Session;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Type;

namespace TheresaBot.GoCqHttp.Plugin
{
    public class GroupMessagePlugin : BasePlugin
    {

        public override async Task OnGroupMessageReceivedAsync(CqGroupMessagePostContext args)
        {
            try
            {
                long msgId = args.MessageId;
                long memberId = args.Sender.UserId;
                long groupId = args.GroupId;
                if (args.Session is not ICqActionSession session) return;
                if (!BusinessHelper.IsHandleMessage(groupId)) return;
                if (memberId == CQConfig.BotQQ) return;
                if (BusinessHelper.IsBanMember(memberId)) return; //黑名单成员

                List<string> plainList = args.Message.OfType<CqTextMsg>().Select(m => m.Text.Trim()).Where(o => !string.IsNullOrEmpty(o)).ToList();

                string instruction = plainList.FirstOrDefault()?.Trim() ?? "";
                string message = args.Message.Text;
                if (string.IsNullOrWhiteSpace(message)) return;

                string prefix = prefix = instruction.MatchPrefix();
                bool isAt = args.Message.Any(v => v is CqAtMsg atMsg && atMsg.Target == CQConfig.BotQQ);
                bool isInstruct = prefix.Length > 0 || BotConfig.GeneralConfig.Prefixs.Count == 0;//可以不设置任何指令前缀
                if (isInstruct) instruction = instruction.Remove(0, prefix.Length).Trim();

                if (args.Message.Any(v => v is CqReplyMsg))//引用指令
                {
                    GroupQuoteCommand quoteCommand = GetGroupQuoteCommand(session, args, instruction);
                    if (quoteCommand is not null) await quoteCommand.InvokeAsync(cqSession, cqReporter);
                    return;
                }

                if (isAt == false && isInstruct == false)//复读,分步操作,保存消息记录
                {
                    CQGroupRelay relay = new CQGroupRelay(args, msgId, message, groupId, memberId);
                    if (StepCache.HandleStep(relay, groupId, memberId)) return; //分步处理
                    if (RepeatCache.CheckCanRepeat(groupId, CQConfig.BotQQ, memberId, message)) await SendRepeat(session, args);//复读机
                    List<string> imgUrls = args.Message.OfType<CqImageMsg>().Select(o => o.Image).ToList();
                    Task task1 = RecordHelper.AddImageRecords(imgUrls, PlatformType.GoCQHttp, msgId, groupId, memberId);
                    List<string> plainMessages = args.Message.OfType<CqTextMsg>().Select(o => o.Text).ToList();
                    Task task2 = RecordHelper.AddPlainRecords(plainMessages, PlatformType.GoCQHttp, msgId, groupId, memberId);
                    return;
                }

                if (string.IsNullOrWhiteSpace(instruction))//空指令
                {
                    return;
                }

                CQGroupCommand command = GetGroupCommand(session, args, instruction);
                if (command is not null)
                {
                    await command.InvokeAsync(cqSession, cqReporter);
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
                await ReplyGroupErrorAsync(ex, args);
                await Task.Delay(1000);
                await cqReporter.SendError(ex, "群指令异常");
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

        private async Task ReplyGroupErrorAsync(Exception exception, CqGroupMessagePostContext args)
        {
            try
            {
                List<BaseContent> contents = exception.GetErrorContents();
                CqMsg[] msgList = contents.ToCQMessageAsync();
                await CQHelper.Session.SendGroupMessageAsync(args.GroupId, new CqMessage(msgList));
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "ReplyGroupErrorAsync异常");
            }
        }




    }
}
