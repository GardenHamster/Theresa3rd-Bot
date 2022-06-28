using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Theresa3rd_Bot.Business;
using Theresa3rd_Bot.Model.PO;
using Theresa3rd_Bot.Type;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Handler
{
    public class WebsiteHandler
    {
        private WebsiteBusiness websiteBusiness;

        public WebsiteHandler()
        {
            websiteBusiness = new WebsiteBusiness();
        }

        public async Task UpdateCookieAsync(IMiraiHttpSession session, IFriendMessageEventArgs args, WebsiteType websiteType, string command, string message, int cookieExpire)
        {
            string cookie = message.splitKeyWord(command);
            if (string.IsNullOrWhiteSpace(cookie))
            {
                await session.SendFriendMessageAsync(args.Sender.Id, new PlainMessage($"未检测到cookie,请使用${command} + cookie形式发送"));
                return;
            }

            cookie = cookie.Trim();
            Dictionary<string, string> cookieDic = cookie.splitCookie();
            string PHPSESSID = cookieDic.ContainsKey("PHPSESSID") ? cookieDic["PHPSESSID"] : null;
            if (string.IsNullOrWhiteSpace(PHPSESSID))
            {
                await session.SendFriendMessageAsync(args.Sender.Id, new PlainMessage($"cookie中没有检测到PHPSESSID，请重新获取cookie"));
                return;
            }

            string[] sessionArr = PHPSESSID.Split('_', StringSplitOptions.RemoveEmptyEntries);
            if (sessionArr.Length < 2 || string.IsNullOrWhiteSpace(sessionArr[0]))
            {
                await session.SendFriendMessageAsync(args.Sender.Id, new PlainMessage($"cookie中的PHPSESSID格式不正确，请重新获取cookie"));
                return;
            }

            long userId = 0;
            if (long.TryParse(sessionArr[0].Trim(), out userId) == false)
            {
                await session.SendFriendMessageAsync(args.Sender.Id, new PlainMessage($"cookie中的PHPSESSID格式不正确，请重新获取cookie"));
                return;
            }

            if (cookieDic.ContainsKey("__cf_bm")) cookieDic.Remove("__cf_bm");
            if (cookieDic.ContainsKey("cto_bundle")) cookieDic.Remove("cto_bundle");
            if (cookieDic.ContainsKey("categorized_tags")) cookieDic.Remove("cookieDic");
            if (cookieDic.ContainsKey("tag_view_ranking")) cookieDic.Remove("tag_view_ranking");
            cookie = cookieDic.joinCookie();

            WebsitePO website = websiteBusiness.updateWebsite(Enum.GetName(typeof(WebsiteType), websiteType), cookie, userId, cookieExpire);
            ConfigHelper.loadWebsite();

            string expireDate = website.CookieExpireDate.ToString("yyyy-MM-dd HH:mm:ss");
            await session.SendFriendMessageAsync(args.Sender.Id, new PlainMessage($"cookie更新完毕,过期时间为{expireDate}"));
        }


    }
}
