using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Bot;
using TheresaBot.Main.Model.Config;

namespace TheresaBot.Main.Common
{
    public static class BotConfig
    {
        public const string BotVersion = "0.11.3";
        public const string BotHomepage = "https://www.theresa3rd.cn";
        public static BotInfos BotInfos = new BotInfos(0, "Bot");
        public static List<long> AcceptGroups = new();
        public static List<string> ServerAddress = new();
        public static List<GroupInfos> GroupInfos = new();
        public static BackstageConfig BackstageConfig = new BackstageConfig();
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
        public static WordCloudConfig WordCloudConfig = new WordCloudConfig();
        public static GameConfig GameConfig = new GameConfig();

        /// <summary>
        /// BotQQ
        /// </summary>
        public static long BotQQ => BotInfos.QQ;
        /// <summary>
        /// BotName
        /// </summary>
        public static string BotName => BotInfos.NickName;
        /// <summary>
        /// 获取第一个指令前缀
        /// </summary>
        public static string DefaultPrefix => GeneralConfig.Prefixs.FirstOrDefault() ?? string.Empty;
        /// <summary>
        /// 获取日志群
        /// </summary>
        public static List<long> ErrorPushGroups => GeneralConfig.ErrorGroups.ToSendableGroups();
        /// <summary>
        /// 获取超级管理员列表
        /// </summary>
        public static List<long> SuperManagers => PermissionsConfig.SuperManagers;
        /// <summary>
        /// 获取订阅群
        /// </summary>
        public static List<long> SubscribeGroups => PermissionsConfig.SubscribeGroups.Contains(0) ? AcceptGroups : PermissionsConfig.SubscribeGroups;

    }
}
