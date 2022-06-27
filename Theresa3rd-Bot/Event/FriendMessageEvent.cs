using Mirai.CSharp.HttpApi.Handlers;
using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Parsers;
using Mirai.CSharp.HttpApi.Parsers.Attributes;
using Mirai.CSharp.HttpApi.Session;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theresa3rd_Bot.Business;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Handler;
using Theresa3rd_Bot.Type;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Event
{
    [RegisterMiraiHttpParser(typeof(DefaultMappableMiraiHttpMessageParser<IFriendMessageEventArgs, FriendMessageEventArgs>))]
    public class FriendMessageEvent : IMiraiHttpMessageHandler<IFriendMessageEventArgs>
    {
        public async Task HandleMessageAsync(IMiraiHttpSession session, IFriendMessageEventArgs args)
        {
            try
            {
                long memberId = args.Sender.Id;
                string prefix = BotConfig.GeneralConfig.Prefix;
                List<string> chainList = args.Chain.Select(m => m.ToString()).ToList();
                List<string> plainList = args.Chain.Where(v => v is PlainMessage && v.ToString().Trim().Length > 0).Select(m => m.ToString().Trim()).ToList();
                if (chainList == null || chainList.Count == 0) return;
                if (plainList == null || plainList.Count == 0) return;

                string message = chainList.Count > 0 ? string.Join(null, chainList.Skip(1).ToArray()) : "";
                string instructions = plainList.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(message) || string.IsNullOrWhiteSpace(instructions)) return;
                instructions = instructions.Trim();
                message = message.Trim();

                bool isInstruct = instructions != null && prefix != null && prefix.Trim().Length > 0 && instructions.StartsWith(prefix);
                if (isInstruct) instructions = instructions.Remove(0, prefix.Length).Trim();

                if (instructions.StartsWith(Command.PixivCookie))
                {
                    if (await BusinessHelper.CheckSuperManagersAsync(session, args) == false) return;
                    await new WebsiteHandler().UpdatePixivCookieAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.SetCookie, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                if (instructions.StartsWith(Command.SaucenaoCookie))
                {
                    if (await BusinessHelper.CheckSuperManagersAsync(session, args) == false) return;
                    await new WebsiteHandler().UpdateSaucenaoCookieAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.SetCookie, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //if (instructions.StartsWith(Command.BiliCookie))
                //{
                //    if (await BusinessHelper.CheckSuperManagersAsync(session, args) == false) return;
                //    await new WebsiteHandler().UpdateBiliCookieAsync(session, args, message);
                //    new RequestRecordBusiness().addRecord(args, CommandType.SetCookie, message);
                //    args.BlockRemainingHandlers = true;
                //    return;
                //}

                await Task.Delay(1000);
                await session.SendFriendMessageAsync(args.Sender.Id, new PlainMessage("ヾ(≧∇≦*)ゝ"));
            }
            catch (System.Exception ex)
            {
                await session.SendTemplateAsync(args, BotConfig.GeneralConfig.ErrorMsg, " 出了点小问题，再试一次吧~");
                LogHelper.Error(ex, "FriendMessageEvent异常");
            }
        }

        





    }
}
