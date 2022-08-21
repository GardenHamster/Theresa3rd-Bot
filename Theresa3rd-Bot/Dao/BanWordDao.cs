using System.Collections.Generic;
using Theresa3rd_Bot.Model.PO;
using Theresa3rd_Bot.Type;

namespace Theresa3rd_Bot.Dao
{
    public class BanWordDao : DbContext<BanWordPO>
    {
        public List<BanWordPO> getListByType(BanType type)
        {
            return Db.Queryable<BanWordPO>().Where(o => o.BanType == type).OrderBy(o => o.CreateDate, SqlSugar.OrderByType.Asc).ToList();
        }

        public BanWordPO getBanWord(BanType type, string keyWord)
        {
            return Db.Queryable<BanWordPO>().Where(o => o.BanType == type && o.KeyWord == keyWord).First();
        }

        public void delBanWord(BanType type, string keyWord)
        {
            Db.Deleteable<BanWordPO>().Where(o => o.BanType == type && o.KeyWord == keyWord).ExecuteCommand();
        }

    }
}
