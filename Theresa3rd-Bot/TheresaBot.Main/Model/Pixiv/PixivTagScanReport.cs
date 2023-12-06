namespace TheresaBot.Main.Model.Pixiv
{
    public record PixivTagScanReport : PixivScanReport
    {
        public int ScanTag { get; set; }

        public int ErrorTag { get; set; }
    }
}
