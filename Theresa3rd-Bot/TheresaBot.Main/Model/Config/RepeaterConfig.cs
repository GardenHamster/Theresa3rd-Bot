namespace TheresaBot.Main.Model.Config
{
    public record RepeaterConfig : BasePluginConfig
    {
        public int RepeatTime { get; set; }

        public int RepeatMode { get; set; }

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
