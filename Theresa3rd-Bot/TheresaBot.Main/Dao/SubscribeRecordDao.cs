using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Dao
{
    public class SubscribeRecordDao : DbContext<SubscribeRecordPO>
    {

        public bool checkExists(SubscribeType subscribeType, string dynamicCode)
        {
            return Db.Queryable<SubscribeRecordPO>().InnerJoin<SubscribePO>((r, s) => r.SubscribeId == s.Id).Where((r, s) => s.SubscribeType == subscribeType && r.DynamicCode == dynamicCode).Any();
        }

    }
}
