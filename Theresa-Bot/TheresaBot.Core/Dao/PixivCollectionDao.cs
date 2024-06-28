using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheresaBot.Core.Model.PO;

namespace TheresaBot.Core.Dao
{
    public class PixivCollectionDao : DbContext<PixivCollectionPO>
    {
        public PixivCollectionPO GetByPixivId(int pixivId)
        {
            return Db.Queryable<PixivCollectionPO>().Where(o => o.PixivId == pixivId).First();
        }



    }
}
