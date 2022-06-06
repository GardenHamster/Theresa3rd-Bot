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

        public Dictionary<long, List<BanWordPO>> getBanSetuMap()
        {
            Dictionary<long, List<BanWordPO>> banSetuMap = new Dictionary<long, List<BanWordPO>>();
            List<BanWordPO> banList = banWordDao.getListByType(BanType.Setu);
            if (banList.Count == 0) return banSetuMap;
            foreach (var item in banList)
            {
                long groupId = item.GroupId;
                if (banSetuMap.ContainsKey(groupId) == false) banSetuMap[groupId] = new List<BanWordPO>();
                banSetuMap[groupId].Add(item);
            }
            return banSetuMap;
        }

        public BanWordPO insertBanWord(string keyword, BanType type, long groupId, bool isRegular)
        {
            BanWordPO banWord = new BanWordPO();
            banWord.KeyWord = keyword;
            banWord.BanType = type;
            banWord.GroupId = groupId;
            banWord.IsRegular = isRegular;
            banWord.CreateDate = DateTime.Now;
            return banWordDao.Insert(banWord);
        }

        public BanWordPO getBanWord(BanType type, long groupId, string keyWord)
        {
            return banWordDao.getBanWord(type, groupId, keyWord);
        }

        public void delBanWord(BanType type, long groupId, string keyWord)
        {
            banWordDao.delBanWord(type, groupId, keyWord);
        }


    }
}
