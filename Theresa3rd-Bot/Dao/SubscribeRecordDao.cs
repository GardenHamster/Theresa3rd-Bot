using Theresa3rd_Bot.Model.PO;

namespace Theresa3rd_Bot.Dao
{
    public class SubscribeRecordDao : DbContext<SubscribeRecordPO>
    {
        public SubscribeRecordPO checkExists(int subscribeId, string dynamicCode)
        {
            return Db.Queryable<SubscribeRecordPO>().Where(o => o.DynamicCode == dynamicCode && o.SubscribeId == subscribeId).OrderBy(o => o.CreateDate, SqlSugar.OrderByType.Desc).First();
        }

    }
}
