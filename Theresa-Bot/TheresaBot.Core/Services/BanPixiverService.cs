using TheresaBot.Core.Dao;
using TheresaBot.Core.Model.PO;

namespace TheresaBot.Core.Services
{
    internal class BanPixiverService
    {
        private BanPixiverDao banPixiverDao;

        public BanPixiverService()
        {
            banPixiverDao = new BanPixiverDao();
        }

        public List<BanPixiverPO> GetBanPixivers()
        {
            return banPixiverDao.GetBanPixivers();
        }

        public BanPixiverPO GetBanPixiver(long pixiverId)
        {
            return banPixiverDao.GetBanPixiver(pixiverId);
        }

        public void InsertBanPixivers(long[] pixiverIds)
        {
            foreach (var pixiverId in pixiverIds)
            {
                InsertBanPixivers(pixiverId);
            }
        }

        public BanPixiverPO InsertBanPixivers(long pixiverId)
        {
            var banPixiver = banPixiverDao.GetBanPixiver(pixiverId);
            if (banPixiver is not null)
            {
                return banPixiver;
            }
            else
            {
                banPixiver = new BanPixiverPO();
                banPixiver.PixiverId = pixiverId;
                banPixiver.CreateDate = DateTime.Now;
                return banPixiverDao.Insert(banPixiver);
            }
        }

        public void DelBanPixiver(long[] pixiverIds)
        {
            foreach (var pixiverId in pixiverIds)
            {
                DelBanPixiver(pixiverId);
            }
        }

        public int DelBanPixiver(long pixiverId)
        {
            return banPixiverDao.DeleteBanPixiver(pixiverId);
        }

        public int DelById(int id)
        {
            return banPixiverDao.DeleteById(id);
        }

        public int DelByIds(int[] ids)
        {
            return banPixiverDao.DeleteByIds(ids);
        }
    }
}
