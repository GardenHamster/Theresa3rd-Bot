using Theresa3rd_Bot.Type;

namespace Theresa3rd_Bot.Model.Config
{
    public class PixivUserSubscribeConfig: BaseSubscribeConfig
    {
        public PixivScanMode ScanMode { get; set; }

        public string SyncCommand { get; set; }

        public int ShelfLife { get; set; }

    }
}
