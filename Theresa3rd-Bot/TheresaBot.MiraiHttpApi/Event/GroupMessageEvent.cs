using Mirai.CSharp.HttpApi.Handlers;
using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Parsers;
using Mirai.CSharp.HttpApi.Parsers.Attributes;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.MiraiHttpApi.Command;
using TheresaBot.MiraiHttpApi.Common;
using TheresaBot.MiraiHttpApi.Helper;
using TheresaBot.MiraiHttpApi.Relay;
using TheresaBot.MiraiHttpApi.Reporter;
using TheresaBot.MiraiHttpApi.Session;

namespace TheresaBot.MiraiHttpApi.Event
{
    [RegisterMiraiHttpParser(typeof(DefaultMappableMiraiHttpMessageParser<IGroupMessageEventArgs, GroupMessageEventArgs>))]
    public class GroupMessageEvent : BaseEvent, IMiraiHttpMessageHandler<IGroupMessageEventArgs>
    {
        private MiraiSession miraiSession;
        private MiraiReporter miraiReporter;

        public GroupMessageEvent()
        {
            this.miraiSession = new MiraiSession();
            this.miraiReporter = new MiraiReporter();
        }

        public async Task HandleMessageAsync(IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            try
            {
                int msgId = args.GetMessageId();
                long memberId = args.Sender.Id;
                long groupId = args.Sender.Group.Id;
                long botId = session.QQNumber ?? 0;
                if (!BusinessHelper.IsHandleMessage(groupId)) return;
                if (memberId == MiraiConfig.MiraiBotQQ) return;
                if (BusinessHelper.IsBanMember(memberId)) return; //黑名单成员

                List<string> chainList = args.Chain.Select(m => m.ToString()).ToList();
                List<string> plainList = args.Chain.Where(v => v is PlainMessage && v.ToString().Trim().Length > 0).Select(m => m.ToString().Trim()).ToList();
                if (chainList is null || chainList.Count == 0) return;

                string instruction = plainList.FirstOrDefault()?.Trim() ?? "";
                string message = chainList.Count > 0 ? string.Join(null, chainList.Skip(1).ToArray())?.Trim() : string.Empty;
                if (string.IsNullOrWhiteSpace(message)) return;

                string prefix = prefix = MatchPrefix(message);
                bool isAt = args.Chain.Where(v => v is AtMessage atMsg && atMsg.Target == session.QQNumber).Any();
                bool isInstruct = prefix.Length > 0 || BotConfig.GeneralConfig.Prefixs.Count == 0;//可以不设置任何指令前缀
                if (isInstruct) instruction = instruction.Remove(0, prefix.Length).Trim();

                if (args.Chain.Where(v => v is QuoteMessage).Any())//引用指令
                {
                    GroupCommand quoteCommand = GetGroupQuoteCommand(session, args, instruction, groupId, memberId);
                    if (quoteCommand is not null) args.BlockRemainingHandlers = await quoteCommand.InvokeAsync(miraiSession, miraiReporter);
                    return;
                }

                if (isAt == false && isInstruct == false)//复读,分步操作,保存消息记录
                {
                    MiraiGroupRelay relay = new MiraiGroupRelay(args, msgId, message, groupId, memberId);
                    if (StepCache.HandleStep(relay, groupId, memberId)) return; //分步处理
                    if (RepeatCache.CheckCanRepeat(groupId, botId, memberId, message)) await SendRepeat(session, args);//复读机
                    List<string> imgUrls = args.Chain.Where(o => o is ImageMessage).Select(o => ((ImageMessage)o).Url).ToList();
                    Task task1 = RecordHelper.AddImageRecords(imgUrls, msgId, groupId, memberId);
                    List<string> plainMessages = args.Chain.Where(o => o is PlainMessage).Select(o => o.ToString()).ToList();
                    Task task2 = RecordHelper.AddPlainRecords(plainMessages, msgId, groupId, memberId);
                    return;
                }

                if (string.IsNullOrWhiteSpace(instruction))//空指令
                {
                    return;
                }

                MiraiGroupCommand command = GetGroupCommand(session, args, instruction, groupId, memberId);
                if (command is not null)
                {
                    args.BlockRemainingHandlers = await command.InvokeAsync(miraiSession, miraiReporter);
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
                await ReplyGroupErrorAsync(ex, args);
                await Task.Delay(1000);
                new MiraiReporter().SendError(ex, "群指令异常");
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

        private async Task ReplyGroupErrorAsync(Exception exception, IGroupMessageEventArgs args)
        {
            try
            {
                List<BaseContent> contents = exception.GetErrorContents();
                IChatMessage[] msgList = await contents.ToMiraiMessageAsync(UploadTarget.Group);
                await MiraiHelper.Session.SendGroupMessageAsync(args.Sender.Group.Id, msgList);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "ReplyGroupErrorAsync异常");
            }
        }


    }
}
