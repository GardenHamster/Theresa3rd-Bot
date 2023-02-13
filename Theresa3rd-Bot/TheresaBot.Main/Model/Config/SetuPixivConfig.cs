using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.Config
{
    public class SetuPixivConfig : BasePluginConfig
    {
        public List<string> Commands { get; set; }
        public PixivRandomType RandomMode { get; set; }
        public List<string> RandomTags { get; set; }
        public string Template { get; set; }
        public int MaxScreen { get; set; }
        public double MinBookmark { get; set; }
        public double MinBookRate { get; set; }

        public SetuPixivConfig()
        {
            this.RandomMode = PixivRandomType.RandomTag;
            this.MaxScreen = 60;
            this.MinBookmark = 1500;
            this.MinBookRate = 0.05;
        }

    }
}
