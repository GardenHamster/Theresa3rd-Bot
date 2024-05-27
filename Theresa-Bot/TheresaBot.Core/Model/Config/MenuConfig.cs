namespace TheresaBot.Core.Model.Config
{
    public record MenuConfig : BasePluginConfig
    {
        public List<string> Commands { get; set; } = new();

        public string Template { get; set; }

        public override MenuConfig FormatConfig()
        {
            if (Commands is null) Commands = new();
            return this;
        }

    }
}
