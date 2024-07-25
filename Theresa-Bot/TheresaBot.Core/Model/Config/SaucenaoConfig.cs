namespace TheresaBot.Core.Model.Config
{
    public record SaucenaoConfig : BasePluginConfig
    {
        public List<string> Commands { get; set; } = new();
        public string NotFoundMsg { get; set; }
        public string ProcessingMsg { get; set; }
        public string Template { get; set; }
        public int MemberCD { get; set; }
        public int MaxDaily { get; set; }
        public int MaxReceive { get; set; } = 5;
        public decimal MinSimilarity { get; set; } = 60;
        public int SaucenaoReadCount { get; set; } = 3;
        public decimal PixivPriority { get; set; } = 80;
        public decimal SinglePriority { get; set; } = 85;
        public decimal ImagePriority { get; set; } = 80;
        public bool PullOrigin { get; set; } = true;
        public bool SendPrivate { get; set; } = true;
        public bool SendPrivateOrigin { get; set; }
        public int RevokeInterval { get; set; }
        public bool RevokeSearched { get; set; }
        public bool ContinueAscii2d { get; set; } = true;
        public bool Ascii2dWithIp { get; set; }
        public int Ascii2dReadCount { get; set; } = 3;

        public override SaucenaoConfig FormatConfig()
        {
            if (MemberCD < 0) MemberCD = 0;
            if (MaxDaily < 0) MaxDaily = 0;
            if (MaxReceive < 1) MaxReceive = 1;
            if (MinSimilarity < 0) MinSimilarity = 0;
            if (MinSimilarity >= 99) MinSimilarity = 99;
            if (SaucenaoReadCount < 1) SaucenaoReadCount = 1;
            if (PixivPriority < 0) PixivPriority = 0;
            if (SinglePriority < 0) SinglePriority = 0;
            if (ImagePriority < 0) ImagePriority = 0;
            if (RevokeInterval < 0) RevokeInterval = 0;
            if (Ascii2dReadCount < 1) Ascii2dReadCount = 1;
            if (Commands is null) Commands = new();
            return this;
        }


    }
}
