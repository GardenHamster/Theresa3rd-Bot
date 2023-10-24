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

        [HttpGet]
        [Authorize]
        [Route("get/permissions")]
        public ApiResult GetPermissions()
        {
            return ApiResult.Success(BotConfig.PermissionsConfig);
        }

        [HttpPost]
        [Authorize]
        [Route("set/permissions")]
        public ApiResult SetPermissions([FromBody] PermissionsConfig config)
        {
            BotConfig.PermissionsConfig = config.FormatConfig();
            ConfigHelper.PermissionsOperater.SaveConfig(config);
            return ApiResult.Success(config);
        }

        [HttpGet]
        [Authorize]
        [Route("get/manage")]
        public ApiResult GetManage()
        {
            return ApiResult.Success(BotConfig.ManageConfig);
        }

        [HttpPost]
        [Authorize]
        [Route("set/manage")]
        public ApiResult SetManage([FromBody] ManageConfig config)
        {
            BotConfig.ManageConfig = config.FormatConfig();
            ConfigHelper.ManageOperater.SaveConfig(config);
            return ApiResult.Success(config);
        }

        [HttpGet]
        [Authorize]
        [Route("get/menu")]
        public ApiResult GetMenu()
        {
            return ApiResult.Success(BotConfig.MenuConfig);
        }

        [HttpPost]
        [Authorize]
        [Route("set/menu")]
        public ApiResult SetMenu([FromBody] MenuConfig config)
        {
            BotConfig.MenuConfig = config.FormatConfig();
            ConfigHelper.MenuOperater.SaveConfig(config);
            return ApiResult.Success(config);
        }

        [HttpGet]
        [Authorize]
        [Route("get/repeater")]
        public ApiResult GetRepeater()
        {
            return ApiResult.Success(BotConfig.RepeaterConfig);
        }

        [HttpPost]
        [Authorize]
        [Route("set/repeater")]
        public ApiResult SetRepeater([FromBody] RepeaterConfig config)
        {
            BotConfig.RepeaterConfig = config.FormatConfig();
            ConfigHelper.RepeaterOperater.SaveConfig(config);
            return ApiResult.Success(config);
        }

        [HttpGet]
        [Authorize]
        [Route("get/welcome")]
        public ApiResult GetWelcome()
        {
            return ApiResult.Success(BotConfig.WelcomeConfig);
        }

        [HttpPost]
        [Authorize]
        [Route("set/welcome")]
        public ApiResult SetWelcome([FromBody] WelcomeConfig config)
        {
            BotConfig.WelcomeConfig = config.FormatConfig();
            ConfigHelper.WelcomeOperater.SaveConfig(config);
            return ApiResult.Success(config);
        }

    }
}
