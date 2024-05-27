using TheresaBot.Core.Model.PO;
using TheresaBot.Core.Type;

namespace TheresaBot.Core.Dao
{
    public class SubscribeRecordDao : DbContext<SubscribeRecordPO>
    {

        public bool CheckExists(SubscribeType subscribeType, string dynamicCode)
        {
            return Db.Queryable<SubscribeRecordPO>().InnerJoin<SubscribePO>((r, s) => r.SubscribeId == s.Id).Any((r, s) => s.SubscribeType == subscribeType && r.DynamicCode == dynamicCode);
        }

    }
}
