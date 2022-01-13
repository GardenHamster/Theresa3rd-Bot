using System.IO;
using Theresa3rd_Bot.Model.Pixiv;
using Theresa3rd_Bot.Model.PO;

namespace Theresa3rd_Bot.Model.Subscribe
{
    public class PixivSubscribe
    {
        public SubscribeRecordPO SubscribeRecord { get; set; }

        public FileInfo WorkFileInfo { get; set; }

        public PixivWorkInfoDto PixivWorkInfoDto { get; set; }

        public string WorkInfo { get; set; }
    }
}
