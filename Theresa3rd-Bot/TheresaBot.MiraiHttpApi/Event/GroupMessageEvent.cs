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
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Type;
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
        public async Task HandleMessageAsync(IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            try
            {
                long memberId = args.Sender.Id;
                long groupId = args.Sender.Group.Id;
                long botId = session.QQNumber ?? 0;
                if (!BusinessHelper.IsHandleMessage(groupId)) return;
                if (memberId == MiraiConfig.MiraiBotQQ) return;
                if (BusinessHelper.IsBanMember(memberId)) return; //黑名单成员

                string prefix = BotConfig.GeneralConfig.Prefix;
                List<string> chainList = args.Chain.Select(m => m.ToString()).ToList();
                List<string> plainList = args.Chain.Where(v => v is PlainMessage && v.ToString().Trim().Length > 0).Select(m => m.ToString().Trim()).ToList();
                if (chainList is null || chainList.Count == 0) return;

                int msgId = args.GetMessageId();
                string message = chainList.Count > 0 ? string.Join(null, chainList.Skip(1).ToArray()) : "";
                string instruction = plainList.FirstOrDefault()?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(message)) return;
                message = message.Trim();

                bool isAt = args.Chain.Where(v => v is AtMessage atMsg && atMsg.Target == session.QQNumber).Any();
                bool isInstruct = string.IsNullOrWhiteSpace(instruction) == false && string.IsNullOrWhiteSpace(prefix) == false && instruction.StartsWith(prefix);
                if (isInstruct) instruction = instruction.Remove(0, prefix.Length).Trim();

                if (isAt == false && isInstruct == false)//没有@也不是一条指令
                {
                    MiraiGroupRelay relay = new MiraiGroupRelay(args, msgId, message, groupId, memberId);
                    if (StepCache.HandleStep(relay, groupId, memberId)) return; //分步处理
                    if (RepeatCache.CheckCanRepeat(groupId, botId, memberId, message)) await SendRepeat(session, args);//复读机
                    return;
                }

                if (string.IsNullOrWhiteSpace(instruction)) return;//不存在任何指令

                MiraiGroupCommand botCommand = GetGroupCommand(session, args, instruction, groupId, memberId);
                if (botCommand is not null)
                {
                    MiraiSession miraiSession = new MiraiSession();
                    MiraiReporter miraiReporter = new MiraiReporter();
                    args.BlockRemainingHandlers = await botCommand.InvokeAsync(miraiSession, miraiReporter);
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
