using Microsoft.AspNetCore.Mvc;
using TheresaBot.Main.Common;
using TheresaBot.Main.Model.Result;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Services;
using TheresaBot.Main.Type;
using Microsoft.AspNetCore.Authorization;
using TheresaBot.Main.Model.DTO;


namespace TheresaBot.Main.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class CookieController : ControllerBase
    {
        private WebsiteService websiteService;
        public CookieController()
        {
            websiteService = new WebsiteService();
        }
        [HttpPost]
        [Authorize]
        [Route("update/pck")]
        public async Task<ApiResult> UpdatePixivCookie([FromBody] CookieDto pck)
        {
            ApiResult result = await UpdatePixivCookieAsync(pck.pcookie);
            return result;
        }
        private async Task<ApiResult> UpdatePixivCookieAsync(string pcko)
        {
            string cookie = pcko;
            if (string.IsNullOrWhiteSpace(cookie))
            {
                Console.WriteLine($"未检测到cookie");
                return ApiResult.Fail($"未检测到cookie");
            }

            cookie = cookie.Replace("\r\n", "").Replace("\r", "").Replace("\n", "").Replace("\"", "").Trim();
            Dictionary<string, string> cookieDic = cookie.SplitCookie();
            string PHPSESSID = cookieDic.ContainsKey("PHPSESSID") ? cookieDic["PHPSESSID"] : string.Empty;
            if (string.IsNullOrWhiteSpace(PHPSESSID))
            {
                Console.WriteLine($"cookie中没有检测到PHPSESSID，请重新获取cookie");
                return ApiResult.Fail($"cookie中没有检测到PHPSESSID，请重新获取cookie");
            }

            string[] sessionArr = PHPSESSID.Split('_', StringSplitOptions.RemoveEmptyEntries);
            if (sessionArr.Length < 2 || string.IsNullOrWhiteSpace(sessionArr[0]))
            {
               Console.WriteLine($"cookie中的PHPSESSID格式不正确，请重新获取cookie");
                return ApiResult.Fail($"cookie中的PHPSESSID格式不正确，请重新获取cookie");
            }

            long userId = 0;
            if (long.TryParse(sessionArr[0].Trim(), out userId) == false)
            {
                Console.WriteLine($"cookie中的PHPSESSID格式不正确，请重新获取cookie");
                return ApiResult.Fail($"cookie中的PHPSESSID格式不正确，请重新获取cookie");
            }

            if (cookieDic.ContainsKey("__cf_bm")) cookieDic.Remove("__cf_bm");
            if (cookieDic.ContainsKey("cto_bundle")) cookieDic.Remove("cto_bundle");
            if (cookieDic.ContainsKey("categorized_tags")) cookieDic.Remove("cookieDic");
            if (cookieDic.ContainsKey("tag_view_ranking")) cookieDic.Remove("tag_view_ranking");
            cookie = cookieDic.JoinCookie();

            string websiteCode = Enum.GetName(typeof(WebsiteType), WebsiteType.Pixiv) ?? string.Empty;
            WebsitePO website = websiteService.UpdateWebsite(websiteCode, cookie, userId, BotConfig.PixivConfig.CookieExpire);
            WebsiteDatas.LoadWebsite();
            string expireDate = website.CookieExpireDate.ToString("yyyy-MM-dd HH:mm:ss");
            Console.WriteLine($"cookie更新完毕,过期时间为{expireDate}");

            return ApiResult.Success();
        }
    }
}
