using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheresaBot.Main.Dao;
using TheresaBot.Main.Model.PO;

namespace TheresaBot.Main.Services
{
    internal class BanPixiverService
    {
        private BanPixiverDao banPixiverDao;

        public BanPixiverService()
        {
            banPixiverDao = new BanPixiverDao();
        }

        public List<BanPixiverPO> getBanPixivers()
        {
            return banPixiverDao.getBanPixivers();
        }

        public BanPixiverPO getBanPixiver(long pixiverId)
        {
            return banPixiverDao.getBanPixiver(pixiverId);
        }

        public BanPixiverPO insertBanPixivers(long pixiverId)
        {
            BanPixiverPO banPixiver = new BanPixiverPO();
            banPixiver.PixiverId = pixiverId;
            banPixiver.CreateDate = DateTime.Now;
            return banPixiverDao.Insert(banPixiver);
        }

        public int DelBanPixiver(long pixiverId)
        {
            return banPixiverDao.delBanPixiver(pixiverId);
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
