namespace TheresaBot.Main.Model.Pixiv
{
    public class PixivRankingDetail
    {
        public PixivRankingContent RankingContent { get; set; }

        public PixivWorkInfo WorkInfo { get; set; }

        public PixivRankingDetail(PixivRankingContent rankingContent, PixivWorkInfo workInfo)
        {
            RankingContent = rankingContent;
            WorkInfo = workInfo;
        }

    }
}
