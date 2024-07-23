using TheresaBot.Core.Model.PO;
using TheresaBot.Core.Type;

namespace TheresaBot.Core.Dao
{
    public class ImageRecordDao : DbContext<ImageRecordPO>
    {
        public List<ImageRecordPO> GetRecords(PlatformType platformType, long msgId, long groupId)
        {
            return Db.Queryable<ImageRecordPO>().Where(o => o.MessageId == msgId && o.PlatformType == platformType && o.GroupId == groupId).ToList();
        }

        public List<ImageRecordPO> GetRecords(long msgId, long groupId)
        {
            return Db.Queryable<ImageRecordPO>().Where(o => o.MessageId == msgId && o.GroupId == groupId).ToList();
        }


    }
}
