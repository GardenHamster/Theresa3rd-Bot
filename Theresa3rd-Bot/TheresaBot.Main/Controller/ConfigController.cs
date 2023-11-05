using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Model.Result;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;
using TheresaBot.Main.Timers;

namespace TheresaBot.Main.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigController : BaseController
    {
        private BaseSession Session;
        private BaseReporter Reporter;

        public ConfigController(BaseSession session, BaseReporter reporter)
        {
            this.Session = session;
            this.Reporter = reporter;
        }

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
            Task task = SchedulerManager.InitDownClearJobAsync(Session, Reporter);
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

        [HttpGet]
        [Authorize]
        [Route("get/reminder")]
        public ApiResult GetReminder()
        {
            return ApiResult.Success(BotConfig.ReminderConfig);
        }

        [HttpPost]
        [Authorize]
        [Route("set/reminder")]
        public ApiResult SetReminder([FromBody] ReminderConfig config)
        {
            BotConfig.ReminderConfig = config.FormatConfig();
            ConfigHelper.ReminderOperater.SaveConfig(config);
            Task task = SchedulerManager.InitReminderJobAsync(Session, Reporter);
            return ApiResult.Success(config);
        }

        [HttpGet]
        [Authorize]
        [Route("get/setu")]
        public ApiResult GetSetu()
        {
            return ApiResult.Success(BotConfig.SetuConfig);
        }

        [HttpPost]
        [Authorize]
        [Route("set/setu")]
        public ApiResult SetSetu([FromBody] SetuConfig config)
        {
            BotConfig.SetuConfig = config.FormatConfig();
            ConfigHelper.SetuOperater.SaveConfig(config);
            return ApiResult.Success(config);
        }

        [HttpGet]
        [Authorize]
        [Route("get/saucenao")]
        public ApiResult GetSaucenao()
        {
            return ApiResult.Success(BotConfig.SaucenaoConfig);
        }

        [HttpPost]
        [Authorize]
        [Route("set/saucenao")]
        public ApiResult SetSaucenao([FromBody] SaucenaoConfig config)
        {
            BotConfig.SaucenaoConfig = config.FormatConfig();
            ConfigHelper.SaucenaoOperater.SaveConfig(config);
            return ApiResult.Success(config);
        }

        [HttpGet]
        [Authorize]
        [Route("get/subscribe")]
        public ApiResult GetSubscribe()
        {
            return ApiResult.Success(BotConfig.SubscribeConfig);
        }

        [HttpPost]
        [Authorize]
        [Route("set/subscribe")]
        public ApiResult SetSubscribe([FromBody] SubscribeConfig config)
        {
            BotConfig.SubscribeConfig = config.FormatConfig();
            ConfigHelper.SubscribeOperater.SaveConfig(config);
            TimerManager.InitTimers(Session, Reporter);
            return ApiResult.Success(config);
        }

        [HttpGet]
        [Authorize]
        [Route("get/timing/setu")]
        public ApiResult GetTimingSetu()
        {
            return ApiResult.Success(BotConfig.TimingSetuConfig);
        }

        [HttpPost]
        [Authorize]
        [Route("set/timing/setu")]
        public ApiResult SetTimingSetu([FromBody] TimingSetuConfig config)
        {
            BotConfig.TimingSetuConfig = config.FormatConfig();
            ConfigHelper.TimingSetuOperater.SaveConfig(config);
            Task task = SchedulerManager.InitTimingSetuJobAsync(Session, Reporter);
            return ApiResult.Success(config);
        }

        [HttpGet]
        [Authorize]
        [Route("get/pixiv/ranking")]
        public ApiResult GetPixivRanking()
        {
            return ApiResult.Success(BotConfig.PixivRankingConfig);
        }

        [HttpPost]
        [Authorize]
        [Route("set/pixiv/ranking")]
        public ApiResult SetPixivRanking([FromBody] PixivRankingConfig config)
        {
            BotConfig.PixivRankingConfig = config.FormatConfig();
            ConfigHelper.PixivRankingOperater.SaveConfig(config);
            Task task = SchedulerManager.InitPixivRankingJobAsync(Session, Reporter);
            return ApiResult.Success(config);
        }

        [HttpGet]
        [Authorize]
        [Route("get/wordcloud")]
        public ApiResult GetWordCloud()
        {
            return ApiResult.Success(BotConfig.WordCloudConfig);
        }

        [HttpPost]
        [Authorize]
        [Route("set/wordcloud")]
        public ApiResult SetWordCloud([FromBody] WordCloudConfig config)
        {
            BotConfig.WordCloudConfig = config.FormatConfig();
            ConfigHelper.WordCloudOperater.SaveConfig(config);
            Task task = SchedulerManager.InitWordCloudJobAsync(Session, Reporter);
            return ApiResult.Success(config);
        }

    }
}
