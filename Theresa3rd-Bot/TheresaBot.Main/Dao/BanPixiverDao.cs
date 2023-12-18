using TheresaBot.Main.Model.PO;

namespace TheresaBot.Main.Dao
{
    public class BanPixiverDao : DbContext<BanPixiverPO>
    {
        public List<BanPixiverPO> GetBanPixivers()
        {
            return Db.Queryable<BanPixiverPO>().OrderByDescending(o => o.Id).ToList();
        }

        public BanPixiverPO GetBanPixiver(long pixiverId)
        {
            return Db.Queryable<BanPixiverPO>().Where(o => o.PixiverId == pixiverId).First();
        }

        public int DeleteBanPixiver(long pixiverId)
        {
            return Db.Deleteable<BanPixiverPO>().Where(o => o.PixiverId == pixiverId).ExecuteCommand();
        }

    }
}
