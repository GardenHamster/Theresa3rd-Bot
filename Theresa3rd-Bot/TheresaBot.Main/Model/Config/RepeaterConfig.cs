namespace TheresaBot.Main.Model.Config
{
    public class RepeaterConfig : BasePluginConfig
    {
        public int RepeatTime { get; set; }

        public int RepeatMode { get; set; }
        public RepeaterConfig()
        {
            this.RepeatTime = 3;
        }

    }
}
