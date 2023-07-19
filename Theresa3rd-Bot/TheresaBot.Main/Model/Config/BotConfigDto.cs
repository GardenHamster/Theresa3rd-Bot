namespace TheresaBot.Main.Model.Config
{
    public class BotConfigDto
    {
        public GeneralConfig General { get; private set; }
        public PixivConfig Pixiv { get; private set; }
        public PermissionsConfig Permissions { get; private set; }
        public ManageConfig Manage { get; private set; }
        public MenuConfig Menu { get; private set; }
        public RepeaterConfig Repeater { get; private set; }
        public WelcomeConfig Welcome { get; private set; }
        public ReminderConfig Reminder { get; private set; }
        public SetuConfig Setu { get; private set; }
        public SaucenaoConfig Saucenao { get; private set; }
        public SubscribeConfig Subscribe { get; private set; }
        public TimingSetuConfig TimingSetu { get; private set; }
        public PixivRankingConfig PixivRanking { get; private set; }
        public WordCloudConfig WordCloudConfig { get; private set; }
    }

}
