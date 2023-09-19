using System.Text;
using TheresaBot.Main.Common;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Model.Yml;

namespace TheresaBot.Main.Helper
{
    public static class ConfigHelper
    {
        public static readonly YmlOperater<BackstageConfig> BackstageOperater = new(Path.Combine(AppContext.BaseDirectory, "Config", "Backstage.yml"));
        public static readonly YmlOperater<GeneralConfig> GeneralOperater = new(Path.Combine(AppContext.BaseDirectory, "Config", "General.yml"));
        public static readonly YmlOperater<PixivConfig> PixivOperater = new(Path.Combine(AppContext.BaseDirectory, "Config", "Pixiv.yml"));
        public static readonly YmlOperater<PermissionsConfig> PermissionsOperater = new(Path.Combine(AppContext.BaseDirectory, "Config", "Permissions.yml"));
        public static readonly YmlOperater<ManageConfig> ManageOperater = new(Path.Combine(AppContext.BaseDirectory, "Config", "Manage.yml"));
        public static readonly YmlOperater<MenuConfig> MenuOperater = new(Path.Combine(AppContext.BaseDirectory, "Config", "Menu.yml"));
        public static readonly YmlOperater<RepeaterConfig> RepeaterOperater = new(Path.Combine(AppContext.BaseDirectory, "Config", "Repeater.yml"));
        public static readonly YmlOperater<WelcomeConfig> WelcomeOperater = new(Path.Combine(AppContext.BaseDirectory, "Config", "Welcome.yml"));
        public static readonly YmlOperater<ReminderConfig> ReminderOperater = new(Path.Combine(AppContext.BaseDirectory, "Config", "Reminder.yml"));
        public static readonly YmlOperater<SetuConfig> SetuOperater = new(Path.Combine(AppContext.BaseDirectory, "Config", "Setu.yml"));
        public static readonly YmlOperater<SaucenaoConfig> SaucenaoOperater = new(Path.Combine(AppContext.BaseDirectory, "Config", "Saucenao.yml"));
        public static readonly YmlOperater<SubscribeConfig> SubscribeOperater = new(Path.Combine(AppContext.BaseDirectory, "Config", "Subscribe.yml"));
        public static readonly YmlOperater<TimingSetuConfig> TimingSetuOperater = new(Path.Combine(AppContext.BaseDirectory, "Config", "TimingSetu.yml"));
        public static readonly YmlOperater<PixivRankingConfig> PixivRankingOperater = new(Path.Combine(AppContext.BaseDirectory, "Config", "PixivRanking.yml"));
        public static readonly YmlOperater<WordCloudConfig> WordCloudOperater = new(Path.Combine(AppContext.BaseDirectory, "Config", "WordCloud.yml"));

        public static void LoadBotConfig()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            BotConfig.BackstageConfig = BackstageOperater.LoadConfig().FormatConfig();
            BotConfig.GeneralConfig = GeneralOperater.LoadConfig().FormatConfig();
            BotConfig.PixivConfig = PixivOperater.LoadConfig().FormatConfig();
            BotConfig.PermissionsConfig = PermissionsOperater.LoadConfig().FormatConfig();
            BotConfig.ManageConfig = ManageOperater.LoadConfig().FormatConfig();
            BotConfig.MenuConfig = MenuOperater.LoadConfig().FormatConfig();
            BotConfig.RepeaterConfig = RepeaterOperater.LoadConfig().FormatConfig();
            BotConfig.WelcomeConfig = WelcomeOperater.LoadConfig().FormatConfig();
            BotConfig.ReminderConfig = ReminderOperater.LoadConfig().FormatConfig();
            BotConfig.SetuConfig = SetuOperater.LoadConfig().FormatConfig();
            BotConfig.SaucenaoConfig = SaucenaoOperater.LoadConfig().FormatConfig();
            BotConfig.SubscribeConfig = SubscribeOperater.LoadConfig().FormatConfig();
            BotConfig.TimingSetuConfig = TimingSetuOperater.LoadConfig().FormatConfig();
            BotConfig.PixivRankingConfig = PixivRankingOperater.LoadConfig().FormatConfig();
            BotConfig.WordCloudConfig = WordCloudOperater.LoadConfig().FormatConfig();
        }

        public static void InitBotConfig()
        {
            InitBackstageConfig();
        }

        private static void InitBackstageConfig()
        {
            if (BotConfig.BackstageConfig is null)
            {
                BotConfig.BackstageConfig = new BackstageConfig();
            }
            var config = BotConfig.BackstageConfig;
            if (string.IsNullOrWhiteSpace(config.SecretKey))
            {
                config.SecretKey = StringHelper.RandomUUID32();
            }
            if (string.IsNullOrWhiteSpace(config.Password))
            {
                config.Password = StringHelper.RandomUUID8();
            }
            BackstageOperater.SaveConfig();
        }













    }
}
