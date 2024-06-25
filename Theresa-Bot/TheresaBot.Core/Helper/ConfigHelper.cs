using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using System.Text;
using TheresaBot.Core.Common;
using TheresaBot.Core.Model.Config;
using TheresaBot.Core.Model.Yml;

namespace TheresaBot.Core.Helper
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
        public static readonly YmlOperater<PixivCollectionConfig> PixivCollectionOperater = new(Path.Combine(AppContext.BaseDirectory, "Config", "PixivCollection.yml"));
        public static readonly YmlOperater<GameConfig> GameOperater = new(Path.Combine(AppContext.BaseDirectory, "Config", "Game.yml"));

        public static void LoadBotConfig()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            BotConfig.BackstageConfig = BackstageOperater.LoadConfig();
            BotConfig.GeneralConfig = GeneralOperater.LoadConfig();
            BotConfig.PixivConfig = PixivOperater.LoadConfig();
            BotConfig.PermissionsConfig = PermissionsOperater.LoadConfig();
            BotConfig.ManageConfig = ManageOperater.LoadConfig();
            BotConfig.MenuConfig = MenuOperater.LoadConfig();
            BotConfig.RepeaterConfig = RepeaterOperater.LoadConfig();
            BotConfig.WelcomeConfig = WelcomeOperater.LoadConfig();
            BotConfig.ReminderConfig = ReminderOperater.LoadConfig();
            BotConfig.SetuConfig = SetuOperater.LoadConfig();
            BotConfig.SaucenaoConfig = SaucenaoOperater.LoadConfig();
            BotConfig.SubscribeConfig = SubscribeOperater.LoadConfig();
            BotConfig.TimingSetuConfig = TimingSetuOperater.LoadConfig();
            BotConfig.PixivRankingConfig = PixivRankingOperater.LoadConfig();
            BotConfig.WordCloudConfig = WordCloudOperater.LoadConfig();
            BotConfig.PixivCollectionConfig = PixivCollectionOperater.LoadConfig();
            BotConfig.GameConfig = GameOperater.LoadConfig();

            if (BotConfig.BackstageConfig is null) BotConfig.BackstageConfig = new();
            if (BotConfig.GeneralConfig is null) BotConfig.GeneralConfig = new();
            if (BotConfig.PermissionsConfig is null) BotConfig.PermissionsConfig = new();
            if (BotConfig.ManageConfig is null) BotConfig.ManageConfig = new();

            BackstageOperater.SaveConfig(BotConfig.BackstageConfig.FormatConfig());
            GeneralOperater.SaveConfig(BotConfig.GeneralConfig.FormatConfig());
            PermissionsOperater.SaveConfig(BotConfig.PermissionsConfig.FormatConfig());
            ManageOperater.SaveConfig(BotConfig.ManageConfig.FormatConfig());
        }

        public static void SetAppConfig(IApplicationBuilder app)
        {
            BotConfig.ServerAddress = app.ServerFeatures.Get<IServerAddressesFeature>()?.Addresses?.ToList() ?? new();
        }



    }
}
