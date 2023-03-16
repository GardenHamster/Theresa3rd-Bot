using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.Config
{
    public class SaucenaoConfig : BasePluginConfig
    {
        public List<string> Commands { get; private set; }
        public string NotFoundMsg { get; private set; }
        public string ProcessingMsg { get; private set; }
        public string Template { get; private set; }
        public int MemberCD { get; private set; }
        public int MaxDaily { get; private set; }
        public int MaxReceive { get; private set; }
        public decimal MinSimilarity { get; private set; }
        public bool PullOrigin { get; private set; }
        public bool SendPrivate { get; private set; }
        public int RevokeInterval { get; private set; }
        public bool RevokeSearched { get; private set; }
        public YNAType ContinueAscii2d { get; private set; }
        public bool Ascii2dWithIp { get; set; }
        public int Ascii2dReadCount { get; private set; }
    }
}
