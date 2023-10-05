namespace TheresaBot.Main.Model.Config
{
    public record SaucenaoConfig : BasePluginConfig
    {
        public List<string> Commands { get; set; }
        public string NotFoundMsg { get; set; }
        public string ProcessingMsg { get; set; }
        public string Template { get; set; }
        public int MemberCD { get; set; }
        public int MaxDaily { get; set; }
        public int MaxReceive { get; set; }
        public decimal MinSimilarity { get; set; } = 60;
        public int SaucenaoReadCount { get; set; } = 3;
        public decimal PixivPriority { get; set; } = 80;
        public decimal SinglePriority { get; set; } = 85;
        public decimal ImagePriority { get; set; } = 80;
        public bool PullOrigin { get; set; } = true;
        public bool SendPrivate { get; set; } = true;
        public int RevokeInterval { get; set; }
        public bool RevokeSearched { get; set; }
        public bool ContinueAscii2d { get; set; } = true;
        public bool Ascii2dWithIp { get; set; }
        public int Ascii2dReadCount { get; set; } = 3;

        public override SaucenaoConfig FormatConfig()
        {
            return this;
        }


    }
}
