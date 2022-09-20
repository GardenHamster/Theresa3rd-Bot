using Theresa3rd_Bot.Type;

namespace Theresa3rd_Bot.Model.Config
{
    public class PixivUserSubscribeConfig: BaseSubscribeConfig
    {
        public PixivScanMode ScanMode { get; set; }

        public string SyncCommand { get; set; }

        public int ShelfLife { get; set; }

        public PixivUserSubscribeConfig()
        {
            this.ScanMode = PixivScanMode.Default;
            this.ShelfLife = 12 * 60 * 60;
            this.ScanInterval = 60 * 60;
        }

    }
}
