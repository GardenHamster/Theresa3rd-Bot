using Mirai.CSharp.HttpApi.Handlers;
using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Parsers;
using Mirai.CSharp.HttpApi.Parsers.Attributes;
using Mirai.CSharp.HttpApi.Session;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Common;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Helper;
using TheresaBot.MiraiHttpApi.Command;
using TheresaBot.MiraiHttpApi.Helper;
using TheresaBot.MiraiHttpApi.Relay;

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
                if (memberId == BotConfig.BotQQ) return;
                if (memberId.IsBanMember()) return; //黑名单成员
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
                var relay = new MiraiFriendRelay(args, msgId, message, memberId, isInstruct);

                if (isInstruct) instruction = instruction.Remove(0, prefix.Length).Trim();
                if (ProcessCache.HandleStep(relay)) return; //分步处理

                MiraiFriendCommand botCommand = GetFriendCommand(args, instruction, prefix);
                if (botCommand is not null)
                {
                    args.BlockRemainingHandlers = await botCommand.InvokeAsync(BaseSession, BaseReporter);
                    return;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "私聊指令异常");
                await BaseSession.ReplyFriendErrorAsync(ex, args.Sender.Id);
                await Task.Delay(1000);
                await BaseReporter.SendError(ex, "私聊指令异常");
            }
        }




    }
}
