using TheresaBot.Main.Model.Mys;
using TheresaBot.Main.Model.PO;

namespace TheresaBot.Main.Model.Subscribe
{
    public class MysSubscribe
    {
        public SubscribeRecordPO SubscribeRecord { get; set; }

        public MysPostListDto MysUserPostDto { get; set; }

        public DateTime CreateTime { get; set; }

    }
}
