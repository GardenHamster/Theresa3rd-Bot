using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Services;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Handler
{
    internal class CookieHandler : BaseHandler
    {
        private WebsiteService websiteService;

        public CookieHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            websiteService = new WebsiteService();
        }

        /// <summary>
        /// 更新pixivcookie
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task UpdatePixivCookieAsync(FriendCommand command)
        {
            string cookie = command.KeyWord;
            if (string.IsNullOrWhiteSpace(cookie))
            {
                await command.ReplyFriendMessageAsync($"未检测到cookie");
                return;
            }

            cookie = cookie.Replace("\r\n", "").Replace("\r", "").Replace("\n", "").Replace("\"", "").Trim();
            Dictionary<string, string> cookieDic = cookie.SplitCookie();
            string PHPSESSID = cookieDic.ContainsKey("PHPSESSID") ? cookieDic["PHPSESSID"] : string.Empty;
            if (string.IsNullOrWhiteSpace(PHPSESSID))
            {
                await command.ReplyFriendMessageAsync($"cookie中没有检测到PHPSESSID，请重新获取cookie");
                return;
            }

            string[] sessionArr = PHPSESSID.Split('_', StringSplitOptions.RemoveEmptyEntries);
            if (sessionArr.Length < 2 || string.IsNullOrWhiteSpace(sessionArr[0]))
            {
                await command.ReplyFriendMessageAsync($"cookie中的PHPSESSID格式不正确，请重新获取cookie");
                return;
            }

            long userId = 0;
            if (long.TryParse(sessionArr[0].Trim(), out userId) == false)
            {
                await command.ReplyFriendMessageAsync($"cookie中的PHPSESSID格式不正确，请重新获取cookie");
                return;
            }

            if (cookieDic.ContainsKey("__cf_bm")) cookieDic.Remove("__cf_bm");
            if (cookieDic.ContainsKey("cto_bundle")) cookieDic.Remove("cto_bundle");
            if (cookieDic.ContainsKey("categorized_tags")) cookieDic.Remove("cookieDic");
            if (cookieDic.ContainsKey("tag_view_ranking")) cookieDic.Remove("tag_view_ranking");
            cookie = cookieDic.JoinCookie();

            string websiteCode = Enum.GetName(typeof(WebsiteType), WebsiteType.Pixiv) ?? string.Empty;
            WebsitePO website = websiteService.updateWebsite(websiteCode, cookie, userId, BotConfig.PixivConfig.CookieExpire);
            WebsiteDatas.LoadWebsite();
            string expireDate = website.CookieExpireDate.ToString("yyyy-MM-dd HH:mm:ss");
            await command.ReplyFriendMessageAsync($"cookie更新完毕,过期时间为{expireDate}");
        }

        /// <summary>
        /// 更新SaucenaoCookie
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task UpdateSaucenaoCookieAsync(FriendCommand command)
        {
            string cookie = command.KeyWord;
            if (string.IsNullOrWhiteSpace(cookie))
            {
                await command.ReplyFriendMessageAsync($"未检测到cookie");
                return;
            }
            //token=62b9ae236fdf9; user=58109; auth=9cd37025e035f2ed99d096f2b7cf5485b7dd50a7;
            cookie = cookie.Replace("\r\n", "").Replace("\r", "").Replace("\n", "").Replace("\"", "").Trim();
            Dictionary<string, string> cookieDic = cookie.SplitCookie();

            string tokenStr = cookieDic.ContainsKey("token") ? cookieDic["token"] : string.Empty;
            if (string.IsNullOrWhiteSpace(tokenStr))
            {
                await command.ReplyFriendMessageAsync($"cookie中没有检测到token，请重新获取cookie");
                return;
            }

            string authStr = cookieDic.ContainsKey("auth") ? cookieDic["auth"] : string.Empty;
            if (string.IsNullOrWhiteSpace(authStr))
            {
                await command.ReplyFriendMessageAsync($"cookie中没有检测到auth，请重新获取cookie");
                return;
            }

            string userStr = cookieDic.ContainsKey("user") ? cookieDic["user"] : string.Empty;
            if (string.IsNullOrWhiteSpace(userStr))
            {
                await command.ReplyFriendMessageAsync($"cookie中没有检测到user，请重新获取cookie");
                return;
            }

            long userId = 0;
            if (long.TryParse(userStr, out userId) == false)
            {
                await command.ReplyFriendMessageAsync($"cookie中的user格式不正确，请重新获取cookie");
                return;
            }

            string websiteCode = Enum.GetName(typeof(WebsiteType), WebsiteType.Saucenao) ?? string.Empty;
            websiteService.updateWebsite(websiteCode, cookie, userId, DateTime.Now.AddYears(1));
            WebsiteDatas.LoadWebsite();
            await command.ReplyFriendMessageAsync($"cookie更新完毕");
        }

        public async Task CheckAndWarn(WebsitePO website, int diffDay, string cookieName)
        {
            DateTime expireDate = website.CookieExpireDate;
            if (DateTime.Now.AddDays(diffDay) < expireDate) return;
            if (expireDate.AddDays(diffDay) < DateTime.Now) return;
            string warnMessage = $"{cookieName}将在{expireDate.ToString("yyyy-MM-dd HH:mm:ss")}过期，请尽快更新cookie";
            foreach (long groupId in BotConfig.GeneralConfig.ErrorGroups)
            {
                await Session.SendGroupMessageAsync(groupId, warnMessage);
            }
            foreach (long memberId in BotConfig.PermissionsConfig.SuperManagers)
            {
                await Session.SendFriendMessageAsync(memberId, warnMessage);
            }
        }

    }
}
