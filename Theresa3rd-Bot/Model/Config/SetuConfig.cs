using System.Collections.Generic;

namespace Theresa3rd_Bot.Model.Config
{
    public class SetuConfig
    {
        public int GroupCD { get; set; }

        public int MemberCD { get; set; }

        public List<long> NoneCDGroups { get; set; }

        public List<long> NoneCDMembers { get; set; }

        public LoliconConfig Lolicon { get; set; }

        public PixivConfig Pixiv { get; set; }

    }


}
