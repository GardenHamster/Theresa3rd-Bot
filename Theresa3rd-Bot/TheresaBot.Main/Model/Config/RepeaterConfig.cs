namespace TheresaBot.Main.Model.Config
{
    public class RepeaterConfig : BasePluginConfig
    {
        public int RepeatTime { get; private set; }

        public int RepeatMode { get; private set; }
        public RepeaterConfig()
        {
            this.RepeatTime = 3;
        }

    }
}
