using Mirai.CSharp.HttpApi.Handlers;
using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Parsers;
using Mirai.CSharp.HttpApi.Parsers.Attributes;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Models;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.MiraiHttpApi.Command;
using TheresaBot.MiraiHttpApi.Common;
using TheresaBot.MiraiHttpApi.Helper;

namespace TheresaBot.MiraiHttpApi.Event
{
    [RegisterMiraiHttpParser(typeof(DefaultMappableMiraiHttpMessageParser<IFriendMessageEventArgs, FriendMessageEventArgs>))]
    public class FriendMessageEvent : BaseEvent, IMiraiHttpMessageHandler<IFriendMessageEventArgs>
    {

        public async Task HandleMessageAsync(IMiraiHttpSession session, IFriendMessageEventArgs args)
        {
            try
            {
                int msgId = args.GetMessageId();
                long memberId = args.Sender.Id;
                if (memberId == MiraiConfig.BotQQ) return;
                if (BusinessHelper.IsBanMember(memberId)) return; //黑名单成员
                List<string> chainList = args.Chain.Select(m => m.ToString()).ToList();
                List<string> plainList = args.Chain.Where(v => v is PlainMessage && v.ToString().Trim().Length > 0).Select(m => m.ToString().Trim()).ToList();
                if (chainList is null || chainList.Count == 0) return;
                if (plainList is null || plainList.Count == 0) return;

                string instruction = plainList.FirstOrDefault()?.Trim() ?? "";
                string message = chainList.Count > 0 ? string.Join(null, chainList.Skip(1).ToArray())?.Trim() : string.Empty;
                if (string.IsNullOrWhiteSpace(message)) return;//空消息
                if (string.IsNullOrWhiteSpace(instruction)) return;//空指令

                string prefix = prefix = instruction.MatchPrefix();
                bool isInstruct = prefix.Length > 0 || BotConfig.GeneralConfig.Prefixs.Count == 0;//可以不设置任何指令前缀
                if (isInstruct) instruction = instruction.Remove(0, prefix.Length).Trim();

                MiraiFriendCommand botCommand = GetFriendCommand(session, args, instruction);
                if (botCommand is not null)
                {
                    args.BlockRemainingHandlers = await botCommand.InvokeAsync(miraiSession, miraiReporter);
                    return;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "私聊指令异常");
                await ReplyFriendErrorAsync(ex, args);
                await Task.Delay(1000);
                await miraiReporter.SendError(ex, "私聊指令异常");
            }
        }

        private async Task ReplyFriendErrorAsync(Exception exception, IFriendMessageEventArgs args)
        {
            try
            {
                List<BaseContent> contents = exception.GetErrorContents();
                IChatMessage[] msgList = await contents.ToMiraiMessageAsync(UploadTarget.Friend);
                await MiraiHelper.Session.SendFriendMessageAsync(args.Sender.Id, msgList.ToArray());
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "ReplyFriendErrorAsync异常");
            }
        }


    }
}
