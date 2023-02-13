namespace TheresaBot.Main.Model.Saucenao
{
    public class SaucenaoResult
    {
        public int MatchCount { get; set; }

        public DateTime StartDateTime { get; set; }

        public List<SaucenaoItem> Items { get; set; }

        public SaucenaoResult(List<SaucenaoItem> items, DateTime startDateTime, int matchCount)
        {
            this.Items = items;
            this.StartDateTime = startDateTime;
            this.MatchCount = matchCount;
        }

    }
}
