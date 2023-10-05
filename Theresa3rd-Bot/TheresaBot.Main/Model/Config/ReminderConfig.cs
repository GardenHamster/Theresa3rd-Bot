namespace TheresaBot.Main.Model.Config
{
    public record ReminderConfig : BasePluginConfig
    {
        public List<ReminderTimer> Timers { get; set; }

        public override ReminderConfig FormatConfig()
        {
            return this;
        }
    }

    public record ReminderTimer
    {
        public bool Enable { get; set; }

        public string Name { get; set; }

        public string Cron { get; set; }

        public List<long> Groups { get; set; }

        public bool AtAll { get; set; }

        public List<long> AtMembers { get; set; }

        public List<RemindTemplate> Templates { get; set; }
    }

    public record RemindTemplate
    {
        public string Template { get; private set; }
    }

}
