using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Theresa3rd_Bot.Business;
using Theresa3rd_Bot.Common;
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

        /// <summary>
        /// 更新pixivcookie
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task UpdatePixivCookieAsync(IMiraiHttpSession session, IFriendMessageEventArgs args, string message)
        {
            string cookie = message.splitKeyWord(BotConfig.ManageConfig?.PixivCookie);
            if (string.IsNullOrWhiteSpace(cookie))
            {
                await session.SendFriendMessageAsync(args.Sender.Id, new PlainMessage($"未检测到cookie,请使用{BotConfig.GeneralConfig.Prefix}{BotConfig.ManageConfig.PixivCookie} + cookie形式发送"));
                return;
            }

            cookie = cookie.Replace("\r\n", "").Replace("\r", "").Replace("\n", "").Replace("\"", "").Trim();
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

            string websiteCode = Enum.GetName(typeof(WebsiteType), WebsiteType.Pixiv);
            WebsitePO website = websiteBusiness.updateWebsite(websiteCode, cookie, userId, BotConfig.GeneralConfig.PixivCookieExpire);
            ConfigHelper.loadWebsite();
            string expireDate = website.CookieExpireDate.ToString("yyyy-MM-dd HH:mm:ss");
            await session.SendFriendMessageAsync(args.Sender.Id, new PlainMessage($" cookie更新完毕,过期时间为{expireDate}"));
        }

        /// <summary>
        /// 更新SaucenaoCookie
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task UpdateSaucenaoCookieAsync(IMiraiHttpSession session, IFriendMessageEventArgs args, string message)
        {
            string cookie = message.splitKeyWord(BotConfig.ManageConfig?.SaucenaoCookie);
            if (string.IsNullOrWhiteSpace(cookie))
            {
                await session.SendFriendMessageAsync(args.Sender.Id, new PlainMessage($"未检测到cookie,请使用{BotConfig.GeneralConfig.Prefix}{BotConfig.ManageConfig.SaucenaoCookie} + cookie形式发送"));
                return;
            }

            //token=62b9ae236fdf9; user=58109; auth=9cd37025e035f2ed99d096f2b7cf5485b7dd50a7;
            cookie = cookie.Replace("\r\n", "").Replace("\r", "").Replace("\n", "").Replace("\"", "").Trim();
            Dictionary<string, string> cookieDic = cookie.splitCookie();

            string tokenStr = cookieDic.ContainsKey("token") ? cookieDic["token"] : null;
            if (string.IsNullOrWhiteSpace(tokenStr))
            {
                await session.SendFriendMessageAsync(args.Sender.Id, new PlainMessage($"cookie中没有检测到token，请重新获取cookie"));
                return;
            }

            string authStr = cookieDic.ContainsKey("auth") ? cookieDic["auth"] : null;
            if (string.IsNullOrWhiteSpace(authStr))
            {
                await session.SendFriendMessageAsync(args.Sender.Id, new PlainMessage($"cookie中没有检测到auth，请重新获取cookie"));
                return;
            }

            string userStr = cookieDic.ContainsKey("user") ? cookieDic["user"] : null;
            if (string.IsNullOrWhiteSpace(userStr))
            {
                await session.SendFriendMessageAsync(args.Sender.Id, new PlainMessage($"cookie中没有检测到user，请重新获取cookie"));
                return;
            }

            long userId = 0;
            if (long.TryParse(userStr, out userId) == false)
            {
                await session.SendFriendMessageAsync(args.Sender.Id, new PlainMessage($"cookie中的user格式不正确，请重新获取cookie"));
                return;
            }

            string websiteCode = Enum.GetName(typeof(WebsiteType), WebsiteType.Saucenao);
            websiteBusiness.updateWebsite(websiteCode, cookie, userId, DateTime.Now.AddYears(1));
            ConfigHelper.loadWebsite();
            await session.SendFriendMessageAsync(args.Sender.Id, new PlainMessage($" cookie更新完毕"));
        }

        /// <summary>
        /// 更新BiliCookie
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task UpdateBiliCookieAsync(IMiraiHttpSession session, IFriendMessageEventArgs args, string message)
        {
            await Task.CompletedTask;
        }




    }
}
