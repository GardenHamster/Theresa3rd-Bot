namespace TheresaBot.Main.Model.Config
{
    public record GameConfig : BasePluginConfig
    {
        public List<string> JoinCommands { get; set; } = new();

        public List<string> StopCommands { get; set; } = new();

        public UndercoverConfig Undercover { get; set; }

        public override BaseConfig FormatConfig()
        {
            if (JoinCommands is null) JoinCommands = new();
            if (StopCommands is null) StopCommands = new();
            Undercover?.FormatConfig();
            return this;
        }
    }

    public record UndercoverConfig : BaseGameConfig
    {
        public List<string> CreateCommands { get; set; } = new();

        public override BaseConfig FormatConfig()
        {
            if (CreateCommands is null) CreateCommands = new();
            return this;
        }
    }


}
