using TheresaBot.Core.Model.PO;

namespace TheresaBot.Core.Dao
{
    public class BanTagDao : DbContext<BanTagPO>
    {
        public List<BanTagPO> GetBanTags()
        {
            return Db.Queryable<BanTagPO>().OrderByDescending(o => o.Id).ToList();
        }

        public BanTagPO GetBanTag(string keyWord)
        {
            return Db.Queryable<BanTagPO>().Where(o => o.Keyword == keyWord).First();
        }

        public int DeleteBanTag(string keyWord)
        {
            return Db.Deleteable<BanTagPO>().Where(o => o.Keyword == keyWord).ExecuteCommand();
        }

    }
}
