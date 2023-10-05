namespace TheresaBot.Main.Model.Config
{
    public record LolisukiConfig : BasePluginConfig
    {
        public List<string> Commands { get; set; }

        public string Level { get; set; }

        public string Template { get; set; }

        public override BasePluginConfig FormatConfig()
        {
            Commands = Commands ?? new();
            return this;
        }

    }
}
