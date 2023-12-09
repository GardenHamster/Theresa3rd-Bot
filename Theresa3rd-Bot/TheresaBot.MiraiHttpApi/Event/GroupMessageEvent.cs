using Mirai.CSharp.HttpApi.Handlers;
using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Parsers;
using Mirai.CSharp.HttpApi.Parsers.Attributes;
using Mirai.CSharp.HttpApi.Session;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Type;
using TheresaBot.MiraiHttpApi.Command;
using TheresaBot.MiraiHttpApi.Helper;
using TheresaBot.MiraiHttpApi.Relay;

namespace TheresaBot.MiraiHttpApi.Event
{
    [RegisterMiraiHttpParser(typeof(DefaultMappableMiraiHttpMessageParser<IGroupMessageEventArgs, GroupMessageEventArgs>))]
    public class GroupMessageEvent : BaseEvent, IMiraiHttpMessageHandler<IGroupMessageEventArgs>
    {

        public async Task HandleMessageAsync(IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            try
            {
                long msgId = args.GetMessageId();
                long memberId = args.Sender.Id;
                long groupId = args.Sender.Group.Id;
                if (groupId.IsAuthorized() == false) return;
                if (memberId == BotConfig.BotQQ) return;
                if (memberId.IsBanMember()) return; //黑名单成员

                var plainList = args.Chain.OfType<PlainMessage>().Select(m => m.Message?.Trim() ?? string.Empty).ToList();
                var instruction = plainList.FirstOrDefault()?.Trim() ?? string.Empty;
                var message = plainList.Count > 0 ? string.Join(null, plainList)?.Trim() : string.Empty;
                var prefix = instruction.MatchPrefix();
                var isInstruct = prefix.Length > 0;
                var isAt = args.Chain.Any(v => v is AtMessage atMsg && atMsg.Target == session.QQNumber);
                var isQuote = args.Chain.Any(v => v is QuoteMessage qtMsg && qtMsg.TargetId == session.QQNumber);
                if (prefix.Length > 0) instruction = instruction.Remove(0, prefix.Length).Trim();
                if (prefix.Length > 0) message = message.Remove(0, prefix.Length).Trim();

                var relay = new MiraiGroupRelay(args, message, isAt, isQuote, isInstruct);
                if (GameCahce.HandleGameMessage(relay)) return; //处理游戏

                if (args.Chain.Any(v => v is QuoteMessage))//引用指令
                {
                    GroupQuoteCommand quoteCommand = GetGroupQuoteCommand(args, instruction, prefix);
                    if (quoteCommand is not null) args.BlockRemainingHandlers = await quoteCommand.InvokeAsync(BaseSession, BaseReporter);
                    return;
                }

                if (isAt == false && isInstruct == false)//复读,分步操作,保存消息记录
                {
                    if (ProcessCache.HandleStep(relay)) return; //分步处理
                    if (RepeatCache.CheckCanRepeat(groupId, BotConfig.BotQQ, memberId, GetSimpleSendContent(args))) await SendRepeat(session, args);//复读机
                    List<string> imgUrls = args.Chain.Where(o => o is ImageMessage).Select(o => ((ImageMessage)o).Url).ToList();
                    Task task1 = RecordHelper.AddImageRecords(imgUrls, PlatformType.Mirai, msgId, groupId, memberId);
                    List<string> plainMessages = args.Chain.Where(o => o is PlainMessage).Select(o => o.ToString()).ToList();
                    Task task2 = RecordHelper.AddMessageRecord(plainMessages, PlatformType.Mirai, msgId, groupId, memberId);
                    return;
                }

                if (string.IsNullOrWhiteSpace(instruction))//空指令
                {
                    return;
                }

                var command = GetGroupCommand(args, instruction, prefix);
                if (command is not null)
                {
                    args.BlockRemainingHandlers = await command.InvokeAsync(BaseSession, BaseReporter);
                    return;
                }

                if (BotConfig.GeneralConfig.SendRelevantCommands)
                {
                    await MiraiHelper.ReplyRelevantCommandsAsync(instruction, groupId, memberId);
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "群指令异常");
                await BaseSession.ReplyGroupErrorAsync(ex, args.Sender.Group.Id);
                await Task.Delay(1000);
                await BaseReporter.SendError(ex, "群指令异常");
            }
        }

        private async Task SendRepeat(IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            try
            {
                if (args.Chain.Length < 2) return;
                IChatMessage[] repeatChain = args.Chain.Skip(1).ToArray();
                await session.SendGroupMessageAsync(args.Sender.Group.Id, repeatChain);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "复读失败");
            }
        }




    }
}
