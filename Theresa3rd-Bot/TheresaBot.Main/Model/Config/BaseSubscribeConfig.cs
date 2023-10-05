namespace TheresaBot.Main.Model.Config
{
    public record BaseSubscribeConfig : BasePluginConfig
    {
        public List<string> AddCommands { get; protected set; }

        public List<string> RmCommands { get; protected set; }

        public string Template { get; protected set; }

        public int ScanInterval { get; protected set; }

        public override BasePluginConfig FormatConfig()
        {
            return this;
        }

    }
}
