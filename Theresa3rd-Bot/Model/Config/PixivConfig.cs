using System.Collections.Generic;

namespace Theresa3rd_Bot.Model.Config
{
    public class PixivConfig
    {
        public bool Enable { get; set; }

        public string Command { get; set; }

        public string DisableMsg { get; set; }

        public string NotFoundMsg { get; set; }

        public string ErrorMsg { get; set; }

        public bool CustomEnable { get; set; }

        public string CustomDisableMsg { get; set; }

        public int RandomMode { get; set; }

        public List<string> RandomTags { get; set; }

        public string ProcessingMag { get; set; }

        public string Template { get; set; }

        public int GroupCD { get; set; }

        public int MemberCD { get; set; }

        public int MaxDaily { get; set; }

        public double MinBookmark { get; set; }

        public double MinBookRate { get; set; }

        public int CookieExpireDay { get; set; }

        public string CookieExpireMag { get; set; }

    }
}
