using TheresaBot.Core.Model.Pixiv;
using TheresaBot.Core.Model.PO;

namespace TheresaBot.Core.Model.Subscribe
{
    public class PixivSubscribe
    {
        public SubscribeRecordPO SubscribeRecord { get; set; }

        public PixivWorkInfo PixivWorkInfo { get; set; }

        public SubscribeTask SubscribeTask { get; set; }

        public PixivSubscribe(SubscribeRecordPO record, PixivWorkInfo workInfo, SubscribeTask subscribeTask)
        {
            this.SubscribeRecord = record;
            this.PixivWorkInfo = workInfo;
            this.SubscribeTask = subscribeTask;
        }

    }
}
