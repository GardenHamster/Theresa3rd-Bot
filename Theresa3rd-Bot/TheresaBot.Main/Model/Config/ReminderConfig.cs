namespace TheresaBot.Main.Model.Config
{
    public record ReminderConfig : BasePluginConfig
    {
        public List<ReminderTimer> Timers { get; private set; }

        public override ReminderConfig FormatConfig()
        {
            return this;
        }
    }

    public record ReminderTimer
    {
        public bool Enable { get; private set; }

        public string Name { get; private set; }

        public string Cron { get; private set; }

        public List<long> Groups { get; private set; }

        public bool AtAll { get; private set; }

        public List<long> AtMembers { get; private set; }

        public List<RemindTemplate> Templates { get; private set; }
    }

    public record RemindTemplate
    {
        public string Template { get; private set; }
    }

}
