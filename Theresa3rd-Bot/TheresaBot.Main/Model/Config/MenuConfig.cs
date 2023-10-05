namespace TheresaBot.Main.Model.Config
{
    public record MenuConfig : BasePluginConfig
    {
        public List<string> Commands { get; set; }

        public string Template { get; set; }

        public override MenuConfig FormatConfig()
        {
            Commands = Commands ?? new();
            return this;
        }

    }
}
