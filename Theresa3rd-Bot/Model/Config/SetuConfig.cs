using System.Collections.Generic;

namespace Theresa3rd_Bot.Model.Config
{
    public class SetuConfig
    {
        public int GroupCD { get; set; }

        public int MemberCD { get; set; }

        public List<string> DisableTags { get; set; }

        public string DisableTagsMsg { get; set; }

        public string DisableTagCommand { get; set; }

        public string EnableTagCommand { get; set; }

        public int MaxDaily { get; set; }

        public int RevokeInterval { get; set; }

        public bool SendPrivate { get; set; }
        
        public string ImgSavePath { get; set; }

        public LoliconConfig Lolicon { get; set; }

        public PixivConfig Pixiv { get; set; }

    }


}
