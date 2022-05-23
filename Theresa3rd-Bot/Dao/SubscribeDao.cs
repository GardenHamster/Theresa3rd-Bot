using System.Collections.Generic;
using System.Text;
using Theresa3rd_Bot.Model.PO;
using Theresa3rd_Bot.Model.Subscribe;
using Theresa3rd_Bot.Type;

namespace Theresa3rd_Bot.Dao
{
    public class SubscribeDao<T> : DbContext<SubscribePO<T>>
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

        public SubscribePO<T> getSubscribe<T>(string subscribeCode, SubscribeType subscribeType)
        {
            return Db.Queryable<SubscribePO<T>>().Where(o => o.SubscribeCode == subscribeCode && o.SubscribeType == subscribeType).First();
        }

        public SubscribePO<T> getSubscribe<T>(string subscribeCode, SubscribeType subscribeType, T subscribeSubType)
        {
            return Db.Queryable<SubscribePO<T>>().Where(o => o.SubscribeCode == subscribeCode && o.SubscribeType == subscribeType && o.SubscribeSubType.Equals(subscribeSubType)).First();
        }


    }
}
