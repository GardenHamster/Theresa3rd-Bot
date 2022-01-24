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
using Theresa3rd_Bot.Model.PO;
using Theresa3rd_Bot.Type;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Event
{
    [RegisterMiraiHttpParser(typeof(DefaultMappableMiraiHttpMessageParser<IFriendMessageEventArgs, FriendMessageEventArgs>))]
    public class FriendMessageEvent : IMiraiHttpMessageHandler<IFriendMessageEventArgs>
    {
        public Task HandleMessageAsync(IMiraiHttpSession session, IFriendMessageEventArgs args)
        {
            Task task = Task.Factory.StartNew(() => DoHandle(session, args));
            return Task.CompletedTask;
        }

        public async Task DoHandle(IMiraiHttpSession session, IFriendMessageEventArgs args)
        {
            long memberId = args.Sender.Id;
            string prefix = BotConfig.GeneralConfig.Prefix;
            List<string> chainList = args.Chain.Select(m => m.ToString()).ToList();
            List<string> plainList = args.Chain.Where(v => v is PlainMessage && v.ToString().Trim().Length > 0).Select(m => m.ToString().Trim()).ToList();
            string message = chainList.Count > 0 ? string.Join(null, chainList.Skip(1).ToArray()) : "";
            string instructions = plainList.FirstOrDefault();
            bool isInstruct = instructions != null && prefix != null && prefix.Trim().Length > 0 && instructions.StartsWith(prefix);
            if (isInstruct) instructions = instructions.Remove(0, prefix.Length);

            if (instructions.StartsWith(Command.PixivCookie))
            {
                if (BotConfig.PermissionsConfig.SuperManagers.Contains(memberId) == false) return;
                await UpdateCookieAsync(session, args, WebsiteType.Pixiv, Command.PixivCookie, message, BotConfig.SetuConfig.Pixiv.CookieExpire);
                new RequestRecordBusiness().addRecord(args, CommandType.SetCookie, message);
                return;
            }

            if (instructions.StartsWith(Command.BiliCookie))
            {
                if (BotConfig.PermissionsConfig.SuperManagers.Contains(memberId) == false) return;
                await UpdateCookieAsync(session, args, WebsiteType.Bili, Command.BiliCookie, message, 60);
                new RequestRecordBusiness().addRecord(args, CommandType.SetCookie, message);
                return;
            }

        }

        private async Task UpdateCookieAsync(IMiraiHttpSession session, IFriendMessageEventArgs args, WebsiteType websiteType, string command, string message, int cookieExpire)
        {
            string cookie = message.splitKeyWord(command);
            if (string.IsNullOrWhiteSpace(cookie))
            {
                await session.SendFriendMessageAsync(args.Sender.Id, new PlainMessage($"未检测到cookie,请使用${command} + cookie形式发送"));
                return;
            }
            WebsitePO website = new WebsiteBusiness().updateWebsite(Enum.GetName(typeof(WebsiteType), websiteType), cookie, cookieExpire);
            ConfigHelper.loadWebsite();
            string expireDate = website.CookieExpireDate.ToString("yyyy-MM-dd HH:mm:ss");
            await session.SendFriendMessageAsync(args.Sender.Id, new PlainMessage($"cookie更新完毕,过期时间为{expireDate}"));
        }





    }
}
