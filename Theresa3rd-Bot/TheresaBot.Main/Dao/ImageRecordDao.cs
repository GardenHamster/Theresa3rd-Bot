using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Dao
{
    public class ImageRecordDao : DbContext<ImageRecordPO>
    {
        public List<ImageRecordPO> GetRecords(PlatformType platformType, long msgId, long groupId)
        {
            return Db.Queryable<ImageRecordPO>().Where(o => o.MessageId == msgId && o.PlatformType == platformType && o.GroupId == groupId).ToList();
        }

    }
}
