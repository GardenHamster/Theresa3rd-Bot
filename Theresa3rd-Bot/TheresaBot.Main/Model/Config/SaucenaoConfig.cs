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
        public decimal MinSimilarity { get; private set; } = 60;
        public int SaucenaoReadCount { get; set; } = 3;
        public decimal PixivPriority { get; private set; } = 80;
        public decimal SinglePriority { get; private set; } = 85;
        public decimal ImagePriority { get; private set; } = 80;
        public bool PullOrigin { get; private set; } = true;
        public bool SendPrivate { get; private set; } = true;
        public int RevokeInterval { get; private set; }
        public bool RevokeSearched { get; private set; }
        public bool ContinueAscii2d { get; private set; } = true;
        public bool Ascii2dWithIp { get; set; }
        public int Ascii2dReadCount { get; private set; } = 3;

        public override SaucenaoConfig FormatConfig()
        {
            return this;
        }


    }
}
