namespace TheresaBot.Main.Model.Config
{
    public record BaseSubscribeConfig : BasePluginConfig
    {
        public List<string> AddCommands { get; set; }

        public List<string> RmCommands { get; set; }

        public string Template { get; set; }

        public int ScanInterval { get; set; }

        public BaseSubscribeConfig()
        {
            ScanInterval = 60;
        }

        public override BasePluginConfig FormatConfig()
        {
            AddCommands = AddCommands ?? new();
            RmCommands = RmCommands ?? new();
            ScanInterval = ScanInterval < 60 ? 60 : ScanInterval;
            return this;
        }

    }
}
