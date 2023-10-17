namespace TheresaBot.Main.Model.Config
{
    public record PermissionsConfig : BaseConfig
    {
        public List<long> AcceptGroups { get; set; } = new();
        public List<long> SuperManagers { get; set; } = new();
        public List<long> LimitlessMembers { get; set; } = new();
        public List<long> SetuGroups { get; set; } = new();
        public List<long> SetuShowImgGroups { get; set; } = new();
        public List<long> SetuShowAIGroups { get; set; } = new();
        public List<long> SetuShowR18Groups { get; set; } = new();
        public List<long> SetuShowR18ImgGroups { get; set; } = new();
        public List<long> SetuCustomGroups { get; set; } = new();
        public List<long> SetuNoneCDGroups { get; set; } = new();
        public List<long> SetuLimitlessGroups { get; set; } = new();
        public List<long> SaucenaoGroups { get; set; } = new();
        public List<long> SaucenaoR18Groups { get; set; } = new();
        public List<long> SubscribeGroups { get; set; } = new();
        public List<long> PixivRankingGroups { get; set; } = new();
        public List<long> WordCloudGroups { get; set; } = new();

        public override PermissionsConfig FormatConfig()
        {
            if (AcceptGroups is null) AcceptGroups = new();
            if (SuperManagers is null) SuperManagers = new();
            if (LimitlessMembers is null) LimitlessMembers = new();
            if (SetuGroups is null) SetuGroups = new();
            if (SetuShowImgGroups is null) SetuShowImgGroups = new();
            if (SetuShowAIGroups is null) SetuShowAIGroups = new();
            if (SetuShowR18Groups is null) SetuShowR18Groups = new();
            if (SetuShowR18ImgGroups is null) SetuShowR18ImgGroups = new();
            if (SetuCustomGroups is null) SetuCustomGroups = new();
            if (SetuNoneCDGroups is null) SetuNoneCDGroups = new();
            if (SetuLimitlessGroups is null) SetuLimitlessGroups = new();
            if (SaucenaoGroups is null) SaucenaoGroups = new();
            if (SaucenaoR18Groups is null) SaucenaoR18Groups = new();
            if (SubscribeGroups is null) SubscribeGroups = new();
            if (PixivRankingGroups is null) PixivRankingGroups = new();
            if (WordCloudGroups is null) WordCloudGroups = new();
            return this;
        }

    }
}
