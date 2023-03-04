namespace TheresaBot.Main.Model.Config
{
    public class ReminderConfig : BasePluginConfig
    {
        public List<ReminderTimer> Timers { get; private set; }
    }

    public class ReminderTimer
    {
        public bool Enable { get; private set; }

        public string Name { get; private set; }

        public string Cron { get; private set; }

        public List<long> Groups { get; private set; }

        public bool AtAll { get; private set; }

        public List<long> AtMembers { get; private set; }

        public string Template { get; private set; }
    }


}
