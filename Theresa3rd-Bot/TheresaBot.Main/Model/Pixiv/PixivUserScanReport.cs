namespace TheresaBot.Main.Model.Pixiv
{
    public record PixivUserScanReport : PixivScanReport
    {
        public int ScanUser { get; set; }

        public int ErrorUser { get; set; }
    }
}
