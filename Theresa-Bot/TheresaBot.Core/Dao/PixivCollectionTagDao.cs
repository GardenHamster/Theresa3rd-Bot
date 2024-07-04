using TheresaBot.Core.Model.PO;

namespace TheresaBot.Core.Dao
{
    public class PixivCollectionTagDao : DbContext<PixivCollectionTagPO>
    {
        public int Delete(int collectionId, int tagId)
        {
            return Db.Deleteable<PixivCollectionTagPO>().Where(o => o.CollectionId == collectionId && o.TagId == tagId).ExecuteCommand();
        }

        public bool CheckExists(int collectionId, int tagId)
        {
            return Db.Queryable<PixivCollectionTagPO>().Any(o => o.CollectionId == collectionId && o.TagId == tagId);
        }

        public PixivCollectionTagPO GetItem(int collectionId, int tagId)
        {
            return Db.Queryable<PixivCollectionTagPO>().First(o => o.CollectionId == collectionId && o.TagId == tagId);
        }

    }
}
