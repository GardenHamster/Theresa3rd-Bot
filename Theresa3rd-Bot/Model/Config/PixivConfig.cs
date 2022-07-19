using System.Collections.Generic;
using Theresa3rd_Bot.Type;

namespace Theresa3rd_Bot.Model.Config
{
    public class PixivConfig
    {
        public bool Enable { get; set; }

        public string Command { get; set; }

        public string DisableMsg { get; set; }

        public string NotFoundMsg { get; set; }

        public string ErrorMsg { get; set; }

        public PixivRandomMode RandomMode { get; set; }

        public List<string> RandomTags { get; set; }

        public string ProcessingMsg { get; set; }

        public string Template { get; set; }

        public int MaxScreen { get; set; }

        public double MinBookmark { get; set; }

        public double MinBookRate { get; set; }

    }
}
