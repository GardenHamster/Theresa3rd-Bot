namespace TheresaBot.Main.Model.Config
{
    public record LocalSetuConfig : BasePluginConfig
    {
        public List<string> Commands { get; private set; }

        public string LocalPath { get; private set; }

        public string Template { get; private set; }

        public override BasePluginConfig FormatConfig()
        {
            return this;
        }

    }
}
