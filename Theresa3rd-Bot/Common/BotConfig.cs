using System.Collections.Generic;
using Theresa3rd_Bot.Model.Config;
using Theresa3rd_Bot.Model.PO;
using Theresa3rd_Bot.Model.Subscribe;
using Theresa3rd_Bot.Type;

namespace Theresa3rd_Bot.Common
{
    public static class BotConfig
    {
        public const string BotVersion = "v0.7.2";

        public static DBConfig DBConfig = new DBConfig();

        public static MiraiConfig MiraiConfig = new MiraiConfig();

        public static GeneralConfig GeneralConfig = new GeneralConfig();

        public static PermissionsConfig PermissionsConfig = new PermissionsConfig();

        public static ManageConfig ManageConfig = new ManageConfig();
        
        public static MenuConfig MenuConfig = new MenuConfig();

        public static RepeaterConfig RepeaterConfig = new RepeaterConfig();

        public static WelcomeConfig WelcomeConfig = new WelcomeConfig();

        public static ReminderConfig ReminderConfig = new ReminderConfig();

        public static SetuConfig SetuConfig = new SetuConfig();

        public static SaucenaoConfig SaucenaoConfig = new SaucenaoConfig();

        public static SubscribeConfig SubscribeConfig = new SubscribeConfig();

        public static WebsiteConfig WebsiteConfig = new WebsiteConfig();

        public static Dictionary<SubscribeType, List<SubscribeTask>> SubscribeTaskMap = new Dictionary<SubscribeType, List<SubscribeTask>>();

        public static List<BanWordPO> BanSetuTagList = new List<BanWordPO>();

        public static List<BanWordPO> BanMemberList = new List<BanWordPO>();

    }
}
