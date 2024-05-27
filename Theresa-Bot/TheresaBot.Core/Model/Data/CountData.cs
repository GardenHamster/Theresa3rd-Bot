namespace TheresaBot.Core.Model.Data
{
    public record CountData
    {
        public int HandleTimes { get; set; }

        public int PixivPushTimes { get; set; }

        public int PixivScanTimes { get; set; }

        public int PixivScanError { get; set; }

    }
}
