using TheresaBot.Main.Model.PO;

namespace TheresaBot.Main.Dao
{
    public class BanTagDao : DbContext<BanTagPO>
    {
        public List<BanTagPO> getBanTags()
        {
            return Db.Queryable<BanTagPO>().ToList();
        }

        public BanTagPO getBanTag(string keyWord)
        {
            return Db.Queryable<BanTagPO>().Where(o => o.KeyWord == keyWord).First();
        }

        public int delBanTag(string keyWord)
        {
            return Db.Deleteable<BanTagPO>().Where(o => o.KeyWord == keyWord).ExecuteCommand();
        }

    }
}
