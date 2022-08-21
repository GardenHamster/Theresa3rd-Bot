using System;
using System.Collections.Generic;
using Theresa3rd_Bot.Dao;
using Theresa3rd_Bot.Model.PO;
using Theresa3rd_Bot.Type;

namespace Theresa3rd_Bot.Business
{
    public class BanWordBusiness
    {
        private BanWordDao banWordDao;

        public BanWordBusiness()
        {
            this.banWordDao = new BanWordDao();
        }

        public List<BanWordPO> getBanSetuTagList()
        {
            return banWordDao.getListByType(BanType.SetuTag);
        }

        public List<BanWordPO> getBanMemberList()
        {
            return banWordDao.getListByType(BanType.Member);
        }

        public BanWordPO insertBanWord(string keyword, BanType type, bool isRegular)
        {
            BanWordPO banWord = new BanWordPO();
            banWord.KeyWord = keyword;
            banWord.BanType = type;
            banWord.IsRegular = isRegular;
            banWord.CreateDate = DateTime.Now;
            return banWordDao.Insert(banWord);
        }

        public BanWordPO getBanWord(BanType type, string keyWord)
        {
            return banWordDao.getBanWord(type, keyWord);
        }

        public void delBanWord(BanType type, string keyWord)
        {
            banWordDao.delBanWord(type, keyWord);
        }


    }
}
