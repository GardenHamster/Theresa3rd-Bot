using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.Config
{
    public class SetuPixivConfig : BasePluginConfig
    {
        public List<string> Commands { get; private set; }
        public PixivRandomType RandomMode { get; private set; }
        public List<string> RandomTags { get; private set; }
        public string Template { get; private set; }
        public int MaxScreen { get; private set; }
        public double MinBookmark { get; private set; }
        public double MinBookRate { get; private set; }

        public SetuPixivConfig()
        {
            this.RandomMode = PixivRandomType.RandomTag;
            this.MaxScreen = 60;
            this.MinBookmark = 1500;
            this.MinBookRate = 0.05;
        }

        public override BasePluginConfig FormatConfig()
        {
            return this;
        }

    }
}
