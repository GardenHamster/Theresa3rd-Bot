namespace TheresaBot.Main.Model.Config
{
    public record LocalSetuConfig : BasePluginConfig
    {
        public List<string> Commands { get; set; } = new();

        public string LocalPath { get; set; }

        public string Template { get; set; }

        public override BasePluginConfig FormatConfig()
        {
            if (Commands is null) Commands = new();
            return this;
        }

    }
}
