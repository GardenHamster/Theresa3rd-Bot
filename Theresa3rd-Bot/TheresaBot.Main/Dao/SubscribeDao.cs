using System.Text;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Model.Subscribe;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Dao
{
    public class SubscribeDao : DbContext<SubscribePO>
    {
        public List<SubscribeInfo> GetSubscribeInfo()
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append(" select s.id as subscribeId,s.subscribeCode,s.subscribeType,s.subscribeSubType,");
            sqlBuilder.Append(" s.subscribeName,sg.groupId from subscribe s");
            sqlBuilder.Append(" inner join subscribe_group sg on sg.subscribeId=s.id");
            sqlBuilder.Append(" order by s.subscribeType asc,sg.groupId asc");
            return Db.Ado.SqlQuery<SubscribeInfo>(sqlBuilder.ToString());
        }

        public SubscribePO GetSubscribe(int subscribeId)
        {
            return Db.Queryable<SubscribePO>().Where(o => o.Id == subscribeId).First();
        }

        public SubscribePO GetSubscribe(string subscribeCode, SubscribeType subscribeType)
        {
            return Db.Queryable<SubscribePO>().Where(o => o.SubscribeCode == subscribeCode && o.SubscribeType == subscribeType).First();
        }

        public List<SubscribePO> GetSubscribes(SubscribeType subscribeType)
        {
            return Db.Queryable<SubscribePO>().Where(o => o.SubscribeType == subscribeType).OrderBy(o => o.SubscribeSubType).ToList();
        }

    }
}
