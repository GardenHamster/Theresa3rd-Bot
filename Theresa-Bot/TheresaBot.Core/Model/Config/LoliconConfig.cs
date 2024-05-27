namespace TheresaBot.Core.Model.Config
{
    public record LoliconConfig : BasePluginConfig
    {
        public List<string> Commands { get; set; } = new();

        public string Template { get; set; }

        public override BasePluginConfig FormatConfig()
        {
            if (Commands is null) Commands = new();
            return this;
        }

    }
}
