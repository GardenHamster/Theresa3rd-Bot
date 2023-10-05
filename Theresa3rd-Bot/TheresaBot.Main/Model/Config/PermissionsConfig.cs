namespace TheresaBot.Main.Model.Config
{
    public record PermissionsConfig : BaseConfig
    {
        public List<long> AcceptGroups { get; set; }
        public List<long> SuperManagers { get; set; }
        public List<long> LimitlessMembers { get; set; }
        public List<long> SetuGroups { get; set; }
        public List<long> SetuShowImgGroups { get; set; }
        public List<long> SetuShowAIGroups { get; set; }
        public List<long> SetuShowR18Groups { get; set; }
        public List<long> SetuShowR18ImgGroups { get; set; }
        public List<long> SetuCustomGroups { get; set; }
        public List<long> SetuNoneCDGroups { get; set; }
        public List<long> SetuLimitlessGroups { get; set; }
        public List<long> SaucenaoGroups { get; set; }
        public List<long> SaucenaoR18Groups { get; set; }
        public List<long> SubscribeGroups { get; set; }
        public List<long> PixivRankingGroups { get; set; }
        public List<long> WordCloudGroups { get; set; }

        public override PermissionsConfig FormatConfig()
        {
            AcceptGroups = AcceptGroups ?? new();
            SuperManagers = SuperManagers ?? new();
            LimitlessMembers = LimitlessMembers ?? new();
            SetuGroups = SetuGroups ?? new();
            SetuShowImgGroups = SetuShowImgGroups ?? new();
            SetuShowAIGroups= SetuShowAIGroups ?? new();
            SetuShowR18Groups= SetuShowR18Groups ?? new();
            SetuShowR18ImgGroups= SetuShowR18ImgGroups ?? new();
            SetuCustomGroups= SetuCustomGroups ?? new();
            SetuNoneCDGroups= SetuNoneCDGroups ?? new();
            SetuLimitlessGroups= SetuLimitlessGroups ?? new();
            SaucenaoGroups= SaucenaoGroups ?? new();
            SaucenaoR18Groups= SaucenaoR18Groups ?? new();
            SubscribeGroups= SubscribeGroups ?? new();
            PixivRankingGroups= PixivRankingGroups ?? new();
            WordCloudGroups= WordCloudGroups ?? new();
            return this;
        }

    }
}
