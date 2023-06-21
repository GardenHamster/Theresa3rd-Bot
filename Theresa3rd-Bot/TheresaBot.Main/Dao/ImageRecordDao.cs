using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheresaBot.Main.Model.PO;

namespace TheresaBot.Main.Dao
{
    public class ImageRecordDao : DbContext<ImageRecordPO>
    {
        public List<ImageRecordPO> getRecord(int msgId)
        {
            return Db.Queryable<ImageRecordPO>().Where(o => o.MessageId == msgId).ToList();
        }

    }
}
