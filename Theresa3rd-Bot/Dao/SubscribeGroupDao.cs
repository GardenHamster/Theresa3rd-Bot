using System.Text;
using Theresa3rd_Bot.Model.PO;

namespace Theresa3rd_Bot.Dao
{
    public class SubscribeGroupDao : DbContext<SubscribeGroupPO>
    {
        /// <summary>
        /// 获取某个订阅的订阅群的数量
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="subscribeId"></param>
        /// <returns></returns>
        public int getCountBySubscribe(long groupId, int subscribeId)
        {
            return Db.Queryable<SubscribeGroupPO>().Where(o => o.GroupId == groupId && o.SubscribeId == subscribeId).Count();
        }

        /// <summary>
        /// 删除订阅
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="subscribeId"></param>
        public int delSubscribeGroup(int subscribeId)
        {
            return Db.Deleteable<SubscribeGroupPO>().Where(o => o.SubscribeId == subscribeId).ExecuteCommand();
        }

        /// <summary>
        /// 删除订阅
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="subscribeId"></param>
        public int delSubscribeGroup(long groupId, int subscribeId)
        {
            return Db.Deleteable<SubscribeGroupPO>().Where(o => o.GroupId == groupId && o.SubscribeId == subscribeId).ExecuteCommand();
        }



    }
}
