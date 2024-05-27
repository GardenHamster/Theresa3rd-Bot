using TheresaBot.Core.Model.Mys;
using TheresaBot.Core.Model.PO;

namespace TheresaBot.Core.Model.Subscribe
{
    public class MysSubscribe
    {
        public SubscribeRecordPO SubscribeRecord { get; set; }

        public MysPostListDto MysUserPostDto { get; set; }

        public DateTime CreateTime { get; set; }

    }
}
