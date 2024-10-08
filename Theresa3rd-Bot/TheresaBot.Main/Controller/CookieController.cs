using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Model.DTO;
using TheresaBot.Main.Model.Result;
using TheresaBot.Main.Services;

namespace TheresaBot.Main.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class CookieController : BaseController
    {
        private WebsiteService websiteService;

        public CookieController()
        {
            websiteService = new WebsiteService();
        }

        [HttpGet]
        [Authorize]
        [Route("get/pixiv")]
        public ApiResult GetPixiv()
        {
            var data = new { cookie = WebsiteDatas.Pixiv?.Cookie ?? string.Empty };
            return ApiResult.Success(data);
        }

        [HttpPost]
        [Authorize]
        [Route("set/pixiv")]
        public ApiResult SetPixiv([FromBody] CookieDto cookie)
        {
            try
            {
                var cookieStr = cookie.Cookie;
                if (string.IsNullOrWhiteSpace(cookieStr)) return ApiResult.ParamError;
                var website = websiteService.UpdatePixivCookie(cookieStr);
                WebsiteDatas.LoadWebsite();
                return ApiResult.Success();
            }
            catch (HandleException ex)
            {
                return ApiResult.Fail(ex);
            }
            catch (Exception ex)
            {
                return ApiResult.Fail(ex);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("get/saucenao")]
        public ApiResult GetSaucenao()
        {
            var data = new { cookie = WebsiteDatas.Saucenao?.Cookie ?? string.Empty };
            return ApiResult.Success(data);
        }

        [HttpPost]
        [Authorize]
        [Route("set/saucenao")]
        public ApiResult SetSaucenao([FromBody] CookieDto cookie)
        {
            try
            {
                var cookieStr = cookie.Cookie;
                if (string.IsNullOrWhiteSpace(cookieStr)) return ApiResult.ParamError;
                var website = websiteService.UpdateSaucenaoCookie(cookieStr);
                WebsiteDatas.LoadWebsite();
                return ApiResult.Success();
            }
            catch (HandleException ex)
            {
                return ApiResult.Fail(ex);
            }
            catch (Exception ex)
            {
                return ApiResult.Fail(ex);
            }
        }

    }
}
