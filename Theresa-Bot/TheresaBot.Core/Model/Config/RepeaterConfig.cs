namespace TheresaBot.Core.Model.Config
{
    public record RepeaterConfig : BasePluginConfig
    {
        public int RepeatTime { get; set; } = 3;

        public int RepeatMode { get; set; }

        public override RepeaterConfig FormatConfig()
        {
            if (RepeatTime < 1) RepeatTime = 1;
            return this;
        }

    }
}
