using Theresa3rd_Bot.Model.Config;
using Theresa3rd_Bot.Model.PO;

namespace Theresa3rd_Bot.Common
{
    public class BotConfig
    {
        public static DBConfig DBConfig = new DBConfig();

        public static MiraiConfig MiraiConfig = new MiraiConfig();

        public static GeneralConfig GeneralConfig = new GeneralConfig();

        public static RepeaterConfig RepeaterConfig = new RepeaterConfig();

        public static WelcomeConfig WelcomeConfig = new WelcomeConfig();

        public static ReminderConfig ReminderConfig = new ReminderConfig();

        public static SetuConfig SetuConfig = new SetuConfig();

        public static SubscribeConfig SubscribeConfig = new SubscribeConfig();


        public static WebsitePO PixivWebsite = new WebsitePO();

        public static WebsitePO BiliWebsite = new WebsitePO();
    }
}
