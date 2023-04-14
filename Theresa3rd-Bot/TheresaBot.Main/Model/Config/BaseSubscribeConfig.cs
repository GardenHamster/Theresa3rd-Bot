namespace TheresaBot.Main.Model.Config
{
    public class BaseSubscribeConfig : BasePluginConfig
    {
        public List<string> AddCommands { get; protected set; }

        public List<string> RmCommands { get; protected set; }

        public List<string> ListCommands { get; protected set; }

        public string Template { get; protected set; }

        public int ScanInterval { get; protected set; }
    }
}
