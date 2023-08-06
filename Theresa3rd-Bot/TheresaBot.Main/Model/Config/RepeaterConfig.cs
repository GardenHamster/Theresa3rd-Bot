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

        public override RepeaterConfig FormatConfig()
        {
            if (RepeatTime <= 1) RepeatTime = 1;
            return this;
        }

    }
}
