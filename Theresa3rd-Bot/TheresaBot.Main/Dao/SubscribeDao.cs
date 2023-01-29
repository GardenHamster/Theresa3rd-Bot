using System.Collections.Generic;
using System.Text;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Model.Subscribe;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Dao
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

        public List<SubscribePO> getSubscribes(SubscribeType subscribeType)
        {
            return Db.Queryable<SubscribePO>().Where(o => o.SubscribeType == subscribeType).OrderBy(o => o.SubscribeSubType).ToList();
        }

        public List<SubscribePO> getSubscribes(string subscribeCode, SubscribeType subscribeType)
        {
            return Db.Queryable<SubscribePO>().Where(o => o.SubscribeCode == subscribeCode && o.SubscribeType == subscribeType).OrderBy(o => o.SubscribeSubType).ToList();
        }

        public SubscribeGroupPO getSubscribeGroup(long groupId, int subscribeId)
        {
            return Db.Queryable<SubscribeGroupPO>().Where(o => o.GroupId == groupId && o.SubscribeId == subscribeId).First();
        }

    }
}
