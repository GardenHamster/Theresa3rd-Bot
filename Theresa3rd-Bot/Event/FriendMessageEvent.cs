using Mirai.CSharp.HttpApi.Handlers;
using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Parsers;
using Mirai.CSharp.HttpApi.Parsers.Attributes;
using Mirai.CSharp.HttpApi.Session;
using System;
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
    public class FriendMessageEvent : BaseEvent, IMiraiHttpMessageHandler<IFriendMessageEventArgs>
    {
        public async Task HandleMessageAsync(IMiraiHttpSession session, IFriendMessageEventArgs args)
        {
            try
            {
                long memberId = args.Sender.Id;
                if (await session.FromOneselfAsync(args)) return;
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

                bool isInstruct = string.IsNullOrWhiteSpace(instructions) == false && string.IsNullOrWhiteSpace(prefix) == false && instructions.StartsWith(prefix);
                if (isInstruct) instructions = instructions.Remove(0, prefix.Length).Trim();

                if (instructions.StartWithCommand(BotConfig.ManageConfig?.PixivCookieCommand))
                {
                    if (await CheckSuperManagersAsync(session, args) == false) return;
                    await new WebsiteHandler().UpdatePixivCookieAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.SetCookie, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                if (instructions.StartWithCommand(BotConfig.ManageConfig?.SaucenaoCookieCommand))
                {
                    if (await CheckSuperManagersAsync(session, args) == false) return;
                    await new WebsiteHandler().UpdateSaucenaoCookieAsync(session, args, message);
                    new RequestRecordBusiness().addRecord(args, CommandType.SetCookie, message);
                    args.BlockRemainingHandlers = true;
                    return;
                }

                //if (instructions.StartsWith(BotConfig.ManageConfig?.BiliCookie))
                //{
                //    if (await BusinessHelper.CheckSuperManagersAsync(session, args) == false) return;
                //    await new WebsiteHandler().UpdateBiliCookieAsync(session, args, message);
                //    new RequestRecordBusiness().addRecord(args, CommandType.SetCookie, message);
                //    args.BlockRemainingHandlers = true;
                //    return;
                //}

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
