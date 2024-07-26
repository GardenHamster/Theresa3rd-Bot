using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheresaBot.Core.Common;
using TheresaBot.Core.Helper;
using TheresaBot.Core.Model.Config;
using TheresaBot.Core.Model.Result;
using TheresaBot.Core.Reporter;
using TheresaBot.Core.Session;
using TheresaBot.Core.Timers;

namespace TheresaBot.Core.Controller
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
        public async Task<ApiResult> SetGeneralAsync([FromBody] GeneralConfig config)
        {
            try
            {
                BotConfig.GeneralConfig = config.FormatConfig();
                ConfigHelper.GeneralOperater.SaveConfig(config);
                await SchedulerManager.InitDownClearJobAsync(Session, Reporter);
                return ApiResult.Success(config);
            }
            catch (Exception ex)
            {
                return ApiResult.Fail(ex);
            }
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
            try
            {
                BotConfig.PixivConfig = config.FormatConfig();
                ConfigHelper.PixivOperater.SaveConfig(config);
                return ApiResult.Success(config);
            }
            catch (Exception ex)
            {
                return ApiResult.Fail(ex);
            }
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
            try
            {
                BotConfig.PermissionsConfig = config.FormatConfig();
                ConfigHelper.PermissionsOperater.SaveConfig(config);
                return ApiResult.Success(config);
            }
            catch (Exception ex)
            {
                return ApiResult.Fail(ex);
            }
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
            try
            {
                BotConfig.ManageConfig = config.FormatConfig();
                ConfigHelper.ManageOperater.SaveConfig(config);
                return ApiResult.Success(config);
            }
            catch (Exception ex)
            {
                return ApiResult.Fail(ex);
            }
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
            try
            {
                BotConfig.MenuConfig = config.FormatConfig();
                ConfigHelper.MenuOperater.SaveConfig(config);
                return ApiResult.Success(config);
            }
            catch (Exception ex)
            {
                return ApiResult.Fail(ex);
            }
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
            try
            {
                BotConfig.RepeaterConfig = config.FormatConfig();
                ConfigHelper.RepeaterOperater.SaveConfig(config);
                return ApiResult.Success(config);
            }
            catch (Exception ex)
            {
                return ApiResult.Fail(ex);
            }
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
            try
            {
                BotConfig.WelcomeConfig = config.FormatConfig();
                ConfigHelper.WelcomeOperater.SaveConfig(config);
                return ApiResult.Success(config);
            }
            catch (Exception ex)
            {
                return ApiResult.Fail(ex);
            }
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
        public async Task<ApiResult> SetReminderAsync([FromBody] ReminderConfig config)
        {
            try
            {
                BotConfig.ReminderConfig = config.FormatConfig();
                ConfigHelper.ReminderOperater.SaveConfig(config);
                await SchedulerManager.InitReminderJobAsync(Session, Reporter);
                return ApiResult.Success(config);
            }
            catch (Exception ex)
            {
                return ApiResult.Fail(ex);
            }
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
            try
            {
                BotConfig.SetuConfig = config.FormatConfig();
                ConfigHelper.SetuOperater.SaveConfig(config);
                return ApiResult.Success(config);
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
            return ApiResult.Success(BotConfig.SaucenaoConfig);
        }

        [HttpPost]
        [Authorize]
        [Route("set/saucenao")]
        public ApiResult SetSaucenao([FromBody] SaucenaoConfig config)
        {
            try
            {
                BotConfig.SaucenaoConfig = config.FormatConfig();
                ConfigHelper.SaucenaoOperater.SaveConfig(config);
                return ApiResult.Success(config);
            }
            catch (Exception ex)
            {
                return ApiResult.Fail(ex);
            }
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
            try
            {
                BotConfig.SubscribeConfig = config.FormatConfig();
                ConfigHelper.SubscribeOperater.SaveConfig(config);
                TimerManager.InitTimers(Session, Reporter);
                return ApiResult.Success(config);
            }
            catch (Exception ex)
            {
                return ApiResult.Fail(ex);
            }
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
        public async Task<ApiResult> SetTimingSetuAsync([FromBody] TimingSetuConfig config)
        {
            try
            {
                BotConfig.TimingSetuConfig = config.FormatConfig();
                ConfigHelper.TimingSetuOperater.SaveConfig(config);
                await SchedulerManager.InitTimingSetuJobAsync(Session, Reporter);
                return ApiResult.Success(config);
            }
            catch (Exception ex)
            {
                return ApiResult.Fail(ex);
            }
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
        public async Task<ApiResult> SetPixivRankingAsync([FromBody] PixivRankingConfig config)
        {
            try
            {
                BotConfig.PixivRankingConfig = config.FormatConfig();
                ConfigHelper.PixivRankingOperater.SaveConfig(config);
                await SchedulerManager.InitPixivRankingJobAsync(Session, Reporter);
                return ApiResult.Success(config);
            }
            catch (Exception ex)
            {
                return ApiResult.Fail(ex);
            }
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
        public async Task<ApiResult> SetWordCloudAsync([FromBody] WordCloudConfig config)
        {
            try
            {
                BotConfig.WordCloudConfig = config.FormatConfig();
                ConfigHelper.WordCloudOperater.SaveConfig(config);
                await SchedulerManager.InitWordCloudJobAsync(Session, Reporter);
                return ApiResult.Success(config);
            }
            catch (Exception ex)
            {
                return ApiResult.Fail(ex);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("get/game")]
        public ApiResult GetGame()
        {
            return ApiResult.Success(BotConfig.GameConfig);
        }

        [HttpPost]
        [Authorize]
        [Route("set/game")]
        public ApiResult SetGame([FromBody] GameConfig config)
        {
            try
            {
                BotConfig.GameConfig = config.FormatConfig();
                ConfigHelper.GameOperater.SaveConfig(config);
                return ApiResult.Success(config);
            }
            catch (Exception ex)
            {
                return ApiResult.Fail(ex);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("get/collection")]
        public ApiResult GetPixivCollection()
        {
            return ApiResult.Success(BotConfig.PixivCollectionConfig);
        }

        [HttpPost]
        [Authorize]
        [Route("set/collection")]
        public ApiResult SetPixivCollection([FromBody] PixivCollectionConfig config)
        {
            try
            {
                BotConfig.PixivCollectionConfig = config.FormatConfig();
                ConfigHelper.PixivCollectionOperater.SaveConfig(config);
                return ApiResult.Success(config);
            }
            catch (Exception ex)
            {
                return ApiResult.Fail(ex);
            }
        }

    }
}
