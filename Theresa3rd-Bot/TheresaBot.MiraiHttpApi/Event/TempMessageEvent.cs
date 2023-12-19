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
    [RegisterMiraiHttpParser(typeof(DefaultMappableMiraiHttpMessageParser<ITempMessageEventArgs, TempMessageEventArgs>))]
    public class TempMessageEvent : BaseEvent, IMiraiHttpMessageHandler<ITempMessageEventArgs>
    {
        public async Task HandleMessageAsync(IMiraiHttpSession session, ITempMessageEventArgs args)
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

                var relay = new MiraiTempRelay(args, message, isInstruct);
                if (GameCahce.HandleGameMessage(relay)) return; //处理游戏消息
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "私聊指令异常");
                await BaseReporter.SendError(ex, "私聊指令异常");
            }
        }




    }
}
