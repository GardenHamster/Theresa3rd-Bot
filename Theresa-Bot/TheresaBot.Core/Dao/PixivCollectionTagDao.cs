using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheresaBot.Core.Model.PO;

namespace TheresaBot.Core.Dao
{
    public class PixivCollectionTagDao : DbContext<PixivCollectionTagPO>
    {
        public int Delete(int collectionId, int tagId)
        {
            return Db.Deleteable<PixivCollectionTagPO>().Where(o => o.CollectionId == collectionId && o.TagId == tagId).ExecuteCommand();
        }
    }

}
