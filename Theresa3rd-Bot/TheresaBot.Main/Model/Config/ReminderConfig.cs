namespace TheresaBot.Main.Model.Config
{
    public record ReminderConfig : BasePluginConfig
    {
        public List<ReminderTimer> Timers { get; set; } = new();

        public override ReminderConfig FormatConfig()
        {
            if (Timers is null) Timers = new();
            foreach (var item in Timers) item?.FormatConfig();
            return this;
        }
    }

    public record ReminderTimer : BaseConfig
    {
        public bool Enable { get; set; }

        public string Name { get; set; }

        public string Cron { get; set; }

        public List<long> Groups { get; set; } = new();

        public bool AtAll { get; set; }

        public List<long> AtMembers { get; set; } = new();

        public List<RemindTemplate> Templates { get; set; } = new();

        public override BaseConfig FormatConfig()
        {
            if (Groups is null) Groups = new();
            if (AtMembers is null) AtMembers = new();
            if (Templates is null) Templates = new();
            foreach (var item in Templates) item?.FormatConfig();
            return this;
        }
    }

    public record RemindTemplate : BaseConfig
    {
        public string Template { get; set; }

        public override BaseConfig FormatConfig()
        {
            return this;
        }
    }

}
