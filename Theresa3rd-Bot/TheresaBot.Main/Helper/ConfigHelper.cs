using System.Text;
using TheresaBot.Main.Common;
using TheresaBot.Main.Model.Config;
using YamlDotNet.Serialization;

namespace TheresaBot.Main.Helper
{
    public class ConfigHelper
    {
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
            BotConfig.GeneralConfig = botConfig.General.FormatConfig();
            BotConfig.PixivConfig = botConfig.Pixiv.FormatConfig().FormatConfig();
            BotConfig.PermissionsConfig = botConfig.Permissions.FormatConfig();
            BotConfig.ManageConfig = botConfig.Manage.FormatConfig();
            BotConfig.MenuConfig = botConfig.Menu.FormatConfig();
            BotConfig.RepeaterConfig = botConfig.Repeater.FormatConfig();
            BotConfig.WelcomeConfig = botConfig.Welcome.FormatConfig();
            BotConfig.ReminderConfig = botConfig.Reminder.FormatConfig();
            BotConfig.SetuConfig = botConfig.Setu.FormatConfig();
            BotConfig.SaucenaoConfig = botConfig.Saucenao.FormatConfig();
            BotConfig.SubscribeConfig = botConfig.Subscribe.FormatConfig();
            BotConfig.TimingSetuConfig = botConfig.TimingSetu.FormatConfig();
            BotConfig.PixivRankingConfig = botConfig.PixivRanking.FormatConfig();
            BotConfig.WordCloudConfig = botConfig.WordCloud.FormatConfig();
        }



    }
}
