using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheresaBot.Core.Model.PO;

namespace TheresaBot.Core.Dao
{
    public class PixivTagDao : DbContext<PixivTagPO>
    {
        public List<PixivTagPO> getTags(string name, bool fullMatch)
        {
            if (fullMatch)
            {
                return Db.Queryable<PixivTagPO>().Where(o => o.Tag == name || o.Zh == name || o.ZhTw == name || o.En == name || o.Ko == name).Take(10).ToList();
            }
            else
            {
                return Db.Queryable<PixivTagPO>().Where(o => o.Tag.Contains(name) || o.Zh.Contains(name) || o.ZhTw.Contains(name) || o.En.Contains(name) || o.Ko.Contains(name)).Take(20).ToList();
            }
        }

        public PixivTagPO getTag(string tagName)
        {
            return Db.Queryable<PixivTagPO>().Where(o => o.Tag == tagName || o.Zh == tagName || o.ZhTw == tagName).First();
        }

        public List<PixivTagPO> getTags(int collectionId)
        {
            return Db.Queryable<PixivTagPO>().InnerJoin<PixivCollectionTagPO>((o, st) => o.Id == st.TagId).Where((o, st) => st.CollectionId == collectionId).ToList();
        }




    }
}
