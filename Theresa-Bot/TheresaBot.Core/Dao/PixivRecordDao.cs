using TheresaBot.Core.Model.PO;
using TheresaBot.Core.Type;

namespace TheresaBot.Core.Dao
{
    public class PixivRecordDao : DbContext<PixivRecordPO>
    {
        public List<PixivRecordPO> GetRecords(PlatformType platformType, long msgId, long groupId)
        {
            return Db.Queryable<PixivRecordPO>().Where(o => o.MessageId == msgId && o.PlatformType == platformType && o.GroupId == groupId).ToList();
        }


    }
}
