using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Model.Subscribe;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Common
{
    public static class BotConfig
    {
        public const string BotVersion = "v0.8.0";
        public const string BotHomepage = "https://www.theresa3rd.cn";
        public static DBConfig DBConfig = new DBConfig();
        public static MiraiConfig MiraiConfig = new MiraiConfig();
        public static GeneralConfig GeneralConfig = new GeneralConfig();
        public static PixivConfig PixivConfig = new PixivConfig();
        public static PermissionsConfig PermissionsConfig = new PermissionsConfig();
        public static ManageConfig ManageConfig = new ManageConfig();
        public static MenuConfig MenuConfig = new MenuConfig();
        public static RepeaterConfig RepeaterConfig = new RepeaterConfig();
        public static WelcomeConfig WelcomeConfig = new WelcomeConfig();
        public static ReminderConfig ReminderConfig = new ReminderConfig();
        public static SetuConfig SetuConfig = new SetuConfig();
        public static SaucenaoConfig SaucenaoConfig = new SaucenaoConfig();
        public static SubscribeConfig SubscribeConfig = new SubscribeConfig();
        public static TimingSetuConfig TimingSetuConfig = new TimingSetuConfig();
        public static PixivRankingConfig PixivRankingConfig = new PixivRankingConfig();
        public static WebsiteConfig WebsiteConfig = new WebsiteConfig();
        public static Dictionary<SubscribeType, List<SubscribeTask>> SubscribeTaskMap = new Dictionary<SubscribeType, List<SubscribeTask>>();
        public static List<BanWordPO> BanSetuTagList = new List<BanWordPO>();
        public static List<BanWordPO> BanMemberList = new List<BanWordPO>();

    }
}
