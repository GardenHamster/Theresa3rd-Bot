namespace TheresaBot.Main.Model.Config
{
    public class PermissionsConfig
    {
        public List<long> AcceptGroups { get; private set; }

        public List<long> SuperManagers { get; private set; }

        public List<long> LimitlessMembers { get; private set; }

        public List<long> SetuGroups { get; private set; }

        public List<long> SetuShowImgGroups { get; private set; }

        public List<long> SetuShowAIGroups { get; private set; }

        public List<long> SetuShowR18Groups { get; private set; }

        public List<long> SetuShowR18ImgGroups { get; private set; }

        public List<long> SetuCustomGroups { get; private set; }

        public List<long> SetuNoneCDGroups { get; private set; }

        public List<long> SetuLimitlessGroups { get; private set; }

        public List<long> SaucenaoGroups { get; private set; }

        public List<long> SaucenaoR18Groups { get; private set; }

        public List<long> SubscribeGroups { get; private set; }

        public List<long> PixivRankingGroups { get; private set; }

    }
}
