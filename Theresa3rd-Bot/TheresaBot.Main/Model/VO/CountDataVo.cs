namespace TheresaBot.Main.Model.VO
{
    public record CountDataVo
    {
        public long RunningSeconds { get; set; }

        public int HandleTimes { get; set; }

        public int PixivPushTimes { get; set; }

        public int PixivScanTimes { get; set; }

        public int PixivScanError { get; set; }

    }
}
