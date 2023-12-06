namespace TheresaBot.Main.Model.Config
{
    public record BaseSubscribeConfig : BasePluginConfig
    {
        public List<string> AddCommands { get; set; } = new();

        public List<string> RmCommands { get; set; } = new();

        public string Template { get; set; }

        public int ScanInterval { get; set; } = 60;

        public override BasePluginConfig FormatConfig()
        {
            if (AddCommands is null) AddCommands = new();
            if (RmCommands is null) RmCommands = new();
            if (ScanInterval < 60) ScanInterval = 60;
            return this;
        }

    }
}
