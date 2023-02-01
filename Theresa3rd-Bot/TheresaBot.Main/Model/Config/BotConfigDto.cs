namespace TheresaBot.Main.Model.Config
{
    public class BotConfigDto
    {
        public GeneralConfig General { get; set; }
        public PixivConfig Pixiv { get; set; }
        public PermissionsConfig Permissions { get; set; }
        public ManageConfig Manage { get; set; }
        public MenuConfig Menu { get; set; }
        public RepeaterConfig Repeater { get; set; }
        public WelcomeConfig Welcome { get; set; }
        public ReminderConfig Reminder { get; set; }
        public SetuConfig Setu { get; set; }
        public SaucenaoConfig Saucenao { get; set; }
        public SubscribeConfig Subscribe { get; set; }
        public TimingSetuConfig TimingSetu { get; set; }
        public object PixivRanking { get; set; }
    }

}
