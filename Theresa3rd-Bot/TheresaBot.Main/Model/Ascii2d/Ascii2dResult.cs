namespace TheresaBot.Main.Model.Ascii2d
{
    public class Ascii2dResult
    {
        public int MatchCount { get; set; }

        public DateTime StartDateTime { get; set; }

        public List<Ascii2dItem> Items { get; set; }

        public Ascii2dResult(List<Ascii2dItem> items, DateTime startDateTime, int matchCount)
        {
            Items = items;
            StartDateTime = startDateTime;
            MatchCount = matchCount;
        }

    }
}
