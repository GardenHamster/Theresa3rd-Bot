using System.Collections.Generic;
using Theresa3rd_Bot.Type;

namespace Theresa3rd_Bot.Model.Config
{
    public class PixivConfig
    {
        public bool Enable { get; set; }

        public string Command { get; set; }

        public PixivRandomMode RandomMode { get; set; }

        public List<string> RandomTags { get; set; }

        public string Template { get; set; }

        public int MaxScreen { get; set; }

        public double MinBookmark { get; set; }

        public double MinBookRate { get; set; }

        public PixivConfig()
        {
            this.RandomMode = PixivRandomMode.RandomTag;
            this.MaxScreen = 60;
            this.MinBookmark = 1500;
            this.MinBookRate = 0.05;
        }

    }
}
