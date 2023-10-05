namespace TheresaBot.Main.Model.Config
{
    public record MenuConfig : BasePluginConfig
    {
        public List<string> Commands { get; private set; }

        public string Template { get; private set; }

        public override MenuConfig FormatConfig()
        {
            return this;
        }

    }
}
