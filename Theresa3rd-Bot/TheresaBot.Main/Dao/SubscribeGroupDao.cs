using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Model.Subscribe;
using TheresaBot.Main.Type;

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
        public bool isExists(long groupId, int subscribeId)
        {
            return Db.Queryable<SubscribeGroupPO>().Any(o => o.GroupId == groupId && o.SubscribeId == subscribeId);
        }

        /// <summary>
        /// 统计某个订阅的数量
        /// </summary>
        /// <param name="subscribeType"></param>
        /// <returns></returns>
        public int countByType(SubscribeType subscribeType)
        {
            return Db.Queryable<SubscribeGroupPO>()
            .InnerJoin<SubscribePO>((sg, s) => sg.SubscribeId == s.Id)
            .Where((sg, s) => s.SubscribeType == subscribeType)
            .Select(sg => sg.SubscribeId).Distinct().Count();
        }

        /// <summary>
        /// 根据订阅Id获取订阅列表
        /// </summary>
        /// <param name="subscribeId"></param>
        /// <returns></returns>
        public List<SubscribeGroupPO> GetSubscribes(int subscribeId)
        {
            return Db.Queryable<SubscribeGroupPO>().Where(sg => sg.SubscribeId == subscribeId).ToList();
        }

        /// <summary>
        /// 查询某个群的某个订阅类型的列表
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="subscribeType"></param>
        /// <returns></returns>
        public List<SubscribeInfo> getSubscribes(long groupId, SubscribeType subscribeType)
        {
            return Db.Queryable<SubscribeGroupPO>()
            .InnerJoin<SubscribePO>((sg, s) => sg.SubscribeId == s.Id)
            .Where((sg, s) => s.SubscribeType == subscribeType && sg.GroupId == groupId)
            .Select((sg, s) => new SubscribeInfo
            {
                Id = sg.Id,
                SubscribeId = sg.SubscribeId,
                SubscribeCode = s.SubscribeCode,
                SubscribeType = s.SubscribeType,
                SubscribeSubType = s.SubscribeSubType,
                SubscribeName = s.SubscribeName,
                SubscribeDate = s.CreateDate,
                GroupId = sg.GroupId
            }).ToList();
        }

        /// <summary>
        /// 查询某个订阅类型的列表
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="subscribeType"></param>
        /// <returns></returns>
        public List<SubscribeInfo> getSubscribes(SubscribeType subscribeType)
        {
            return Db.Queryable<SubscribeGroupPO>()
            .InnerJoin<SubscribePO>((sg, s) => sg.SubscribeId == s.Id)
            .Where((sg, s) => s.SubscribeType == subscribeType)
            .Select((sg, s) => new SubscribeInfo
            {
                Id = sg.Id,
                SubscribeId = sg.SubscribeId,
                SubscribeCode = s.SubscribeCode,
                SubscribeType = s.SubscribeType,
                SubscribeSubType = s.SubscribeSubType,
                SubscribeName = s.SubscribeName,
                SubscribeDate = s.CreateDate,
                GroupId = sg.GroupId
            }).ToList();
        }

        /// <summary>
        /// 删除订阅
        /// </summary>
        /// <param name="subscribeId"></param>
        public int delBySubscribeId(int subscribeId)
        {
            return Db.Deleteable<SubscribeGroupPO>().Where(o => o.SubscribeId == subscribeId).ExecuteCommand();
        }

        /// <summary>
        /// 删除订阅
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="subscribeId"></param>
        public int delBySubscribeId(long groupId, int subscribeId)
        {
            return Db.Deleteable<SubscribeGroupPO>().Where(o => o.GroupId == groupId && o.SubscribeId == subscribeId).ExecuteCommand();
        }

    }
}
