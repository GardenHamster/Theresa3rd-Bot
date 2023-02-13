namespace TheresaBot.Main.Model.Config
{
    public class LocalSetuConfig : BasePluginConfig
    {
        public List<string> Commands { get; set; }

        public string LocalPath { get; set; }

        public string Template { get; set; }

    }
}
