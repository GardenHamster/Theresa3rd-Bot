using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Dao
{
    public class MessageRecordDao : DbContext<MessageRecordPO>
    {
        public List<MessageRecordPO> getRecords(long groupId, DateTime startTime, DateTime endTime)
        {
            return Db.Queryable<MessageRecordPO>().Where(o => o.GroupId == groupId && o.CreateDate >= startTime && o.CreateDate <= endTime).ToList();
        }

    }
}
