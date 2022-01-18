using System.Collections.Generic;
using Theresa3rd_Bot.Model.Config;
using Theresa3rd_Bot.Model.PO;
using Theresa3rd_Bot.Model.Subscribe;
using Theresa3rd_Bot.Type;

namespace Theresa3rd_Bot.Common
{
    public static class BotConfig
    {
        public static DBConfig DBConfig = new DBConfig();

        public static MiraiConfig MiraiConfig = new MiraiConfig();

        public static GeneralConfig GeneralConfig = new GeneralConfig();

        public static RepeaterConfig RepeaterConfig = new RepeaterConfig();

        public static WelcomeConfig WelcomeConfig = new WelcomeConfig();

        public static ReminderConfig ReminderConfig = new ReminderConfig();

        public static SetuConfig SetuConfig = new SetuConfig();

        public static SaucenaoConfig SaucenaoConfig = new SaucenaoConfig();

        public static SubscribeConfig SubscribeConfig = new SubscribeConfig();

        public static WebsiteConfig WebsiteConfig = new WebsiteConfig();

        /// <summary>
        /// 订阅任务
        /// </summary>
        public static Dictionary<SubscribeType, List<SubscribeTask>> SubscribeTaskMap = new Dictionary<SubscribeType, List<SubscribeTask>>();

    }
}
