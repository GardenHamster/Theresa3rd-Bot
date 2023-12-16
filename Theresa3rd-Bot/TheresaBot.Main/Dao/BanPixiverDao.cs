using TheresaBot.Main.Model.PO;

namespace TheresaBot.Main.Dao
{
    public class BanPixiverDao : DbContext<BanPixiverPO>
    {
        public List<BanPixiverPO> getBanPixivers()
        {
            return Db.Queryable<BanPixiverPO>().ToList();
        }

        public BanPixiverPO getBanPixiver(long pixiverId)
        {
            return Db.Queryable<BanPixiverPO>().Where(o => o.PixiverId == pixiverId).First();
        }

        public int delBanPixiver(long pixiverId)
        {
            return Db.Deleteable<BanPixiverPO>().Where(o => o.PixiverId == pixiverId).ExecuteCommand();
        }

    }
}
