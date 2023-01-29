using System.Collections.Generic;
using Theresa3rd_Bot.Type;

namespace Theresa3rd_Bot.Model.Config
{
    public class SetuPixivConfig : BasePluginConfig
    {
        public List<string> Commands { get; set; }

        public PixivRandomMode RandomMode { get; set; }

        public List<string> RandomTags { get; set; }

        public string Template { get; set; }

        public int MaxScreen { get; set; }

        public double MinBookmark { get; set; }

        public double MinBookRate { get; set; }

        public SetuPixivConfig()
        {
            this.RandomMode = PixivRandomMode.RandomTag;
            this.MaxScreen = 60;
            this.MinBookmark = 1500;
            this.MinBookRate = 0.05;
        }

    }
}
