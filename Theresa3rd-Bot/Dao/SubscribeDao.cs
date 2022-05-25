using System.Collections.Generic;
using System.Text;
using Theresa3rd_Bot.Model.PO;
using Theresa3rd_Bot.Model.Subscribe;
using Theresa3rd_Bot.Type;

namespace Theresa3rd_Bot.Dao
{
    public class SubscribeDao : DbContext<SubscribePO>
    {
        public List<SubscribeInfo> getSubscribeInfo()
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append(" select s.id as subscribeId,s.subscribeCode,s.subscribeType,s.subscribeSubType,s.subscribeName,");
            sqlBuilder.Append(" s.subscribeDescription,sg.groupId from subscribe s");
            sqlBuilder.Append(" inner join subscribe_group sg on sg.subscribeId=s.id");
            sqlBuilder.Append(" order by s.subscribeType asc,sg.groupId asc");
            return Db.Ado.SqlQuery<SubscribeInfo>(sqlBuilder.ToString());
        }

        public SubscribePO getSubscribe(string subscribeCode, SubscribeType subscribeType)
        {
            return Db.Queryable<SubscribePO>().Where(o => o.SubscribeCode == subscribeCode && o.SubscribeType == subscribeType).First();
        }

        public SubscribePO getSubscribe(string subscribeCode, SubscribeType subscribeType, int subscribeSubType)
        {
            return Db.Queryable<SubscribePO>().Where(o => o.SubscribeCode == subscribeCode && o.SubscribeType == subscribeType && o.SubscribeSubType == subscribeSubType).First();
        }

        public List<SubscribePO> getSubscribes(string subscribeCode, SubscribeType subscribeType)
        {
            return Db.Queryable<SubscribePO>().Where(o => o.SubscribeCode == subscribeCode && o.SubscribeType == subscribeType).OrderBy(o => o.SubscribeSubType).ToList();
        }

    }
}
