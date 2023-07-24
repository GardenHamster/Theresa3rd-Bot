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
            BotConfig.PixivConfig = botConfig.Pixiv.FormatConfig();
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
            BotConfig.WordCloudConfig = botConfig.WordCloud;
        }



    }
}
