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

                var plainList = args.Chain.OfType<PlainMessage>().Where(o => string.IsNullOrWhiteSpace(o.Message) == false).Select(m => m.Message.Trim()).ToList();
                var instruction = plainList.FirstOrDefault()?.Trim() ?? string.Empty;
                var message = plainList.Count > 0 ? string.Join(null, plainList)?.Trim() : string.Empty;
                var prefix = instruction.MatchPrefix();
                var isInstruct = prefix.Length > 0;
                if (prefix.Length > 0) instruction = instruction.Remove(0, prefix.Length).Trim();
                if (prefix.Length > 0) message = message.Remove(0, prefix.Length).Trim();

                var relay = new MiraiFriendRelay(args, message, isInstruct);
                if (GameCahce.HandleGameMessage(relay)) return; //处理游戏消息
                if (isInstruct == false && ProcessCache.HandleStep(relay)) return; //分步处理
                if (string.IsNullOrWhiteSpace(instruction)) return; //空指令

                var botCommand = GetFriendCommand(args, instruction, prefix);
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
