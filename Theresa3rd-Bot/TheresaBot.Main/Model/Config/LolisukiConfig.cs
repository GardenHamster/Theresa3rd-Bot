namespace TheresaBot.Main.Model.Config
{
    public record LolisukiConfig : BasePluginConfig
    {
        public List<string> Commands { get; private set; }

        public string Level { get; private set; }

        public string Template { get; private set; }

        public override BasePluginConfig FormatConfig()
        {
            return this;
        }

    }
}
