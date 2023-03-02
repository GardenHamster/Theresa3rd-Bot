namespace TheresaBot.Main.Model.Config
{
    public class BaseSubscribeConfig : BasePluginConfig
    {
        public List<string> AddCommands { get; private set; }

        public List<string> RmCommands { get; private set; }

        public string Template { get; private set; }

        public int ScanInterval { get; protected set; }
    }
}
