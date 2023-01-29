using System.Collections.Generic;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.Config
{
    public class SaucenaoConfig : BasePluginConfig
    {
        public List<string> Commands { get; set; }
        public string DisableMsg { get; set; }
        public string NotFoundMsg { get; set; }
        public string ErrorMsg { get; set; }
        public string ProcessingMsg { get; set; }
        public string Template { get; set; }
        public int MemberCD { get; set; }
        public int MaxDaily { get; set; }
        public int MaxReceive { get; set; }
        public decimal MinSimilarity { get; set; }
        public bool PullOrigin { get; set; }
        public bool SendPrivate { get; set; }
        public int RevokeInterval { get; set; }
        public bool RevokeSearched { get; set; }
        public YNAType ContinueAscii2d { get; set; }
        public int Ascii2dReadCount { get; set; }
    }
}
