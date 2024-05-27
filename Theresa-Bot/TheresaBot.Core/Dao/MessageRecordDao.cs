using TheresaBot.Core.Model.PO;

namespace TheresaBot.Core.Dao
{
    public class MessageRecordDao : DbContext<MessageRecordPO>
    {
        public List<MessageRecordPO> GetRecords(long groupId, DateTime startTime, DateTime endTime)
        {
            return Db.Queryable<MessageRecordPO>().Where(o => o.GroupId == groupId && o.CreateDate >= startTime && o.CreateDate <= endTime).ToList();
        }

    }
}
