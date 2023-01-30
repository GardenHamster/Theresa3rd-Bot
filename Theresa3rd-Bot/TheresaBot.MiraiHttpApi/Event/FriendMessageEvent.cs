using Mirai.CSharp.HttpApi.Handlers;
using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Parsers;
using Mirai.CSharp.HttpApi.Parsers.Attributes;
using Mirai.CSharp.HttpApi.Session;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.MiraiHttpApi.Command;
using TheresaBot.MiraiHttpApi.Helper;
using TheresaBot.MiraiHttpApi.Reporter;
using TheresaBot.MiraiHttpApi.Session;

namespace TheresaBot.MiraiHttpApi.Event
{
    [RegisterMiraiHttpParser(typeof(DefaultMappableMiraiHttpMessageParser<IFriendMessageEventArgs, FriendMessageEventArgs>))]
    public class FriendMessageEvent : BaseEvent, IMiraiHttpMessageHandler<IFriendMessageEventArgs>
    {
        public async Task HandleMessageAsync(IMiraiHttpSession session, IFriendMessageEventArgs args)
        {
            try
            {
                long memberId = args.Sender.Id;
                if (memberId == BotConfig.MiraiConfig.BotQQ) return;
                string prefix = BotConfig.GeneralConfig.Prefix;
                List<string> chainList = args.Chain.Select(m => m.ToString()).ToList();
                List<string> plainList = args.Chain.Where(v => v is PlainMessage && v.ToString().Trim().Length > 0).Select(m => m.ToString().Trim()).ToList();
                if (chainList is null || chainList.Count == 0) return;
                if (plainList is null || plainList.Count == 0) return;

                int msgId = args.GetMessageId();
                string message = chainList.Count > 0 ? string.Join(null, chainList.Skip(1).ToArray()) : "";
                string instructions = plainList.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(message) || string.IsNullOrWhiteSpace(instructions)) return;
                instructions = instructions.Trim();
                message = message.Trim();

                bool isInstruct = string.IsNullOrWhiteSpace(instructions) == false && string.IsNullOrWhiteSpace(prefix) == false && instructions.StartsWith(prefix);
                if (isInstruct) instructions = instructions.Remove(0, prefix.Length).Trim();

                if (string.IsNullOrWhiteSpace(instructions)) return;//不存在任何指令

                MiraiFriendCommand botCommand = GetFriendCommand(session, args, message, memberId);
                if (botCommand is not null)
                {
                    MiraiSession miraiSession = new MiraiSession();
                    MiraiReporter miraiReporter = new MiraiReporter();
                    args.BlockRemainingHandlers = await botCommand.InvokeAsync(miraiSession, miraiReporter);
                    return;
                }

                await session.SendFriendMessageAsync(args.Sender.Id, new PlainMessage("ヾ(≧∇≦*)ゝ"));
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex, "私聊指令异常");
                await ReplyErrorAsync(session, args);
                await Task.Delay(1000);
                new MiraiReporter().SendError(ex, "私聊指令异常");
            }
        }

        public async Task<int> ReplyErrorAsync(IMiraiHttpSession session, IFriendMessageEventArgs args)
        {
            string template = BotConfig.GeneralConfig.ErrorMsg;
            if (string.IsNullOrWhiteSpace(template)) template = "出了点小问题，再试一次吧~";
            List<IChatMessage> msgList = new List<IChatMessage>();
            msgList.AddRange(await template.SplitToChainAsync().ToMiraiMessageAsync());
            return await MiraiHelper.Session.SendFriendMessageAsync(args.Sender.Id, msgList.ToArray());
        }






    }
}
