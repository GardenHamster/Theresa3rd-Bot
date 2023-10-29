using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.Config
{
    public record SetuPixivConfig : BasePluginConfig
    {
        public List<string> Commands { get; set; } = new();
        public PixivRandomType RandomMode { get; set; } = PixivRandomType.RandomTag;
        public List<string> RandomTags { get; set; } = new();
        public string Template { get; set; }
        public int MaxScan { get; set; } = 60;
        public double MinBookmark { get; set; } = 1000;
        public double MinBookRate { get; set; } = 0.05;

        public override BasePluginConfig FormatConfig()
        {
            if (Commands is null) Commands = new();
            if (RandomTags is null) RandomTags = new();
            return this;
        }

    }
}
