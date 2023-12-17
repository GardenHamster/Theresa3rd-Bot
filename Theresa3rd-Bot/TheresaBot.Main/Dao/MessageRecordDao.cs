using TheresaBot.Main.Model.PO;

namespace TheresaBot.Main.Dao
{
    public class MessageRecordDao : DbContext<MessageRecordPO>
    {
        public List<MessageRecordPO> GetRecords(long groupId, DateTime startTime, DateTime endTime)
        {
            return Db.Queryable<MessageRecordPO>().Where(o => o.GroupId == groupId && o.CreateDate >= startTime && o.CreateDate <= endTime).ToList();
        }

    }
}
