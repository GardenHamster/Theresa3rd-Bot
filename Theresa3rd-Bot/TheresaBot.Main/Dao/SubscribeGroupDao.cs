using System.Text;
using TheresaBot.Main.Model.PO;

namespace TheresaBot.Main.Dao
{
    public class SubscribeGroupDao : DbContext<SubscribeGroupPO>
    {
        /// <summary>
        /// 某个群是否已订阅某个subscribeId
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="subscribeId"></param>
        /// <returns></returns>
        public bool isExistsSubscribeGroup(long groupId, int subscribeId)
        {
            return Db.Queryable<SubscribeGroupPO>().Where(o => o.GroupId == groupId && o.SubscribeId == subscribeId).Any();
        }

        /// <summary>
        /// 是否已订阅某个subscribeId
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="subscribeId"></param>
        /// <returns></returns>
        public bool isExistsSubscribeGroup(int subscribeId)
        {
            return Db.Queryable<SubscribeGroupPO>().Where(o => o.SubscribeId == subscribeId).Any();
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
