namespace TheresaBot.Core.Model.Pixiv
{
    public record PixivScanReport
    {
        public int PushWork { get; set; }

        public int ScanWork { get; set; }

        public int ErrorWork { get; set; }
    }
}
