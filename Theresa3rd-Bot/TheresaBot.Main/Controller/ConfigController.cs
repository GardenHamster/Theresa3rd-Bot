using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Model.Result;

namespace TheresaBot.Main.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigController : BaseController
    {
        [HttpGet]
        [Authorize]
        [Route("get/general")]
        public ApiResult GetGeneral()
        {
            return ApiResult.Success(BotConfig.GeneralConfig);
        }

        [HttpPost]
        [Authorize]
        [Route("set/general")]
        public ApiResult SetGeneral([FromBody] GeneralConfig config)
        {
            BotConfig.GeneralConfig = config.FormatConfig();
            ConfigHelper.GeneralOperater.SaveConfig(config);
            return ApiResult.Success(config);
        }

        [HttpGet]
        [Authorize]
        [Route("get/pixiv")]
        public ApiResult GetPixiv()
        {
            return ApiResult.Success(BotConfig.PixivConfig);
        }

        [HttpPost]
        [Authorize]
        [Route("set/pixiv")]
        public ApiResult SetPixiv([FromBody] PixivConfig config)
        {
            BotConfig.PixivConfig = config.FormatConfig();
            ConfigHelper.PixivOperater.SaveConfig(config);
            return ApiResult.Success(config);
        }

    }
}
