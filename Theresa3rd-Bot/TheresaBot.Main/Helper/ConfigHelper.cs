using Microsoft.Extensions.Configuration;
using System.Text;
using TheresaBot.Main.Business;
using TheresaBot.Main.Common;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Type;
using YamlDotNet.Serialization;

namespace TheresaBot.Main.Helper
{
    public class ConfigHelper
    {
        private static IConfiguration Configuration;

        public static void setConfiguration(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 加载MiraiHttpApi配置
        /// </summary>
        public static void LoadMiraiConfig()
        {
            BotConfig.DBConfig.ConnectionString = Configuration["Database:ConnectionString"];
            BotConfig.MiraiConfig.Host = Configuration["Mirai:host"];
            BotConfig.MiraiConfig.Port = Convert.ToInt32(Configuration["Mirai:port"]);
            BotConfig.MiraiConfig.AuthKey = Configuration["Mirai:authKey"];
            BotConfig.MiraiConfig.BotQQ = Convert.ToInt64(Configuration["Mirai:botQQ"]);
        }

        /// <summary>
        /// 加载botsetting.yml配置
        /// </summary>
        public static void LoadBotConfig()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            string ymlPath = Path.Combine(AppContext.BaseDirectory, "botsettings.yml");
            using FileStream fileStream = new FileStream(ymlPath, FileMode.Open, FileAccess.Read);
            using TextReader reader = new StreamReader(fileStream, Encoding.GetEncoding("gb2312"));
            Deserializer deserializer = new Deserializer();
            BotConfigDto botConfig = deserializer.Deserialize<BotConfigDto>(reader);
            BotConfig.GeneralConfig = botConfig.General;
            BotConfig.PixivConfig = botConfig.Pixiv;
            BotConfig.PermissionsConfig = botConfig.Permissions;
            BotConfig.ManageConfig = botConfig.Manage;
            BotConfig.MenuConfig = botConfig.Menu;
            BotConfig.RepeaterConfig = botConfig.Repeater;
            BotConfig.WelcomeConfig = botConfig.Welcome;
            BotConfig.ReminderConfig = botConfig.Reminder;
            BotConfig.SetuConfig = botConfig.Setu;
            BotConfig.SaucenaoConfig = botConfig.Saucenao;
            BotConfig.SubscribeConfig = botConfig.Subscribe;
            BotConfig.TimingSetuConfig = botConfig.TimingSetu;
            BotConfig.PixivRankingConfig = botConfig.PixivRanking;
        }

        public static void LoadWebsite()
        {
            try
            {
                WebsiteBusiness websiteBusiness = new WebsiteBusiness();
                WebsitePO pixivWebsite = new WebsiteBusiness().getOrInsertWebsite(Enum.GetName(typeof(WebsiteType), WebsiteType.Pixiv));
                WebsitePO biliWebsite = new WebsiteBusiness().getOrInsertWebsite(Enum.GetName(typeof(WebsiteType), WebsiteType.Bili));
                WebsitePO saucenaoWebsite = new WebsiteBusiness().getOrInsertWebsite(Enum.GetName(typeof(WebsiteType), WebsiteType.Saucenao));
                BotConfig.WebsiteConfig.Pixiv = pixivWebsite;
                BotConfig.WebsiteConfig.Bili = biliWebsite;
                BotConfig.WebsiteConfig.Saucenao = saucenaoWebsite;
                LogHelper.Info("网站cookie加载完成...");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "加载cookie失败");
            }
        }

        public static void LoadSubscribeTask()
        {
            try
            {
                BotConfig.SubscribeTaskMap = new SubscribeBusiness().getSubscribeTask();
                LogHelper.Info("订阅任务加载完成...");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "订阅任务加载失败");
            }
        }


        public static void LoadBanTag()
        {
            try
            {
                BotConfig.BanSetuTagList = new BanWordBusiness().getBanSetuTagList();
                LogHelper.Info("加载禁止标签完毕");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "加载禁止标签失败");
            }
        }

        public static void LoadBanMember()
        {
            try
            {
                BotConfig.BanMemberList = new BanWordBusiness().getBanMemberList();
                LogHelper.Info("加载黑名单完毕");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "加载黑名单失败");
            }
        }



    }
}
