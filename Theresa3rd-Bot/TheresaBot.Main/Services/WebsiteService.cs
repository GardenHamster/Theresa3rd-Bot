using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Dao;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Services
{
    internal class WebsiteService
    {
        private WebsiteDao websiteDao;

        public WebsiteService()
        {
            websiteDao = new WebsiteDao();
        }

        /// <summary>
        /// 更新Pixiv Cookie
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        /// <exception cref="HandleException"></exception>
        public WebsitePO UpdatePixivCookie(string cookie)
        {
            cookie = cookie.Replace("\r\n", string.Empty);
            cookie = cookie.Replace("\r", string.Empty);
            cookie = cookie.Replace("\n", string.Empty);
            cookie = cookie.Replace("\"", string.Empty);
            cookie = cookie.Trim();

            var cookieDic = cookie.SplitCookie();
            var sessionId = cookieDic.ContainsKey("PHPSESSID") ? cookieDic["PHPSESSID"] : string.Empty;
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                throw new HandleException($"cookie中缺少PHPSESSID，请重新获取cookie");
            }

            var splitArr = sessionId.Split('_', StringSplitOptions.RemoveEmptyEntries);
            if (splitArr.Length < 2 || string.IsNullOrWhiteSpace(splitArr[0]))
            {
                throw new HandleException($"cookie中的PHPSESSID格式不正确，请重新获取cookie");
            }

            long userId = 0;
            if (long.TryParse(splitArr[0].Trim(), out userId) == false)
            {
                throw new HandleException($"cookie中的PHPSESSID格式不正确，请重新获取cookie");
            }

            if (cookieDic.ContainsKey("__cf_bm")) cookieDic.Remove("__cf_bm");
            if (cookieDic.ContainsKey("cto_bundle")) cookieDic.Remove("cto_bundle");
            if (cookieDic.ContainsKey("categorized_tags")) cookieDic.Remove("cookieDic");
            if (cookieDic.ContainsKey("tag_view_ranking")) cookieDic.Remove("tag_view_ranking");

            var websiteCookie = cookieDic.JoinCookie();
            var websiteCode = WebsiteType.Pixiv.ToString();
            var websiteExpire = BotConfig.PixivConfig.CookieExpire;
            return UpdateWebsite(websiteCode, websiteCookie, userId, websiteExpire);
        }

        /// <summary>
        /// 更新Saucenao Cookie
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        /// <exception cref="HandleException"></exception>
        public WebsitePO UpdateSaucenaoCookie(string cookie)
        {
            cookie = cookie.Replace("\r\n", string.Empty);
            cookie = cookie.Replace("\r", string.Empty);
            cookie = cookie.Replace("\n", string.Empty);
            cookie = cookie.Replace("\"", string.Empty);
            cookie = cookie.Trim();

            //token=62b9ae236fdf9; user=58109; auth=9cd37025e035f2ed99d096f2b7cf5485b7dd50a7;
            var cookieDic = cookie.SplitCookie();
            var tokenStr = cookieDic.ContainsKey("token") ? cookieDic["token"] : string.Empty;
            if (string.IsNullOrWhiteSpace(tokenStr))
            {
                throw new HandleException($"cookie中缺少token，请重新获取cookie");
            }

            var authStr = cookieDic.ContainsKey("auth") ? cookieDic["auth"] : string.Empty;
            if (string.IsNullOrWhiteSpace(authStr))
            {
                throw new HandleException($"cookie中缺少auth，请重新获取cookie");
            }

            var userStr = cookieDic.ContainsKey("user") ? cookieDic["user"] : string.Empty;
            if (string.IsNullOrWhiteSpace(userStr))
            {
                throw new HandleException($"cookie中缺少user，请重新获取cookie");
            }

            long userId = 0;
            if (long.TryParse(userStr, out userId) == false)
            {
                throw new HandleException($"cookie中user格式不正确，请重新获取cookie");
            }

            var websiteCode = WebsiteType.Saucenao.ToString();
            return UpdateWebsite(websiteCode, cookie, userId, DateTime.Now.AddYears(1));
        }


        public WebsitePO UpdateWebsite(string code, string cookie, long userid, int expireSeconds)
        {
            WebsitePO website = GetOrInsert(code);
            website.Cookie = cookie;
            website.UserId = userid;
            website.UpdateDate = DateTime.Now;
            website.CookieExpireDate = DateTime.Now.AddSeconds(expireSeconds);
            websiteDao.Update(website);
            return website;
        }

        public WebsitePO UpdateWebsite(string code, string cookie, long userid, DateTime expireDate)
        {
            WebsitePO website = GetOrInsert(code);
            website.Cookie = cookie;
            website.UserId = userid;
            website.UpdateDate = DateTime.Now;
            website.CookieExpireDate = expireDate;
            websiteDao.Update(website);
            return website;
        }

        public WebsitePO GetOrInsert(string code)
        {
            WebsitePO website = websiteDao.GetByCode(code);
            if (website != null) return website;
            website = new WebsitePO();
            website.Code = code;
            website.Cookie = "";
            website.UserId = 0;
            website.UpdateDate = DateTime.Now;
            website.CookieExpireDate = DateTime.Now;
            return websiteDao.Insert(website);
        }




    }
}
