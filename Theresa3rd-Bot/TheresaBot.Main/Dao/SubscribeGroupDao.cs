using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Model.Subscribe;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Dao
{
    public class SubscribeGroupDao : DbContext<SubscribeGroupPO>
    {
        /// <summary>
        /// 统计某个订阅的数量
        /// </summary>
        /// <param name="subscribeType"></param>
        /// <returns></returns>
        public int CountByType(SubscribeType subscribeType)
        {
            return Db.Queryable<SubscribeGroupPO>()
            .InnerJoin<SubscribePO>((sg, s) => sg.SubscribeId == s.Id)
            .Where((sg, s) => s.SubscribeType == subscribeType)
            .Select(sg => sg.SubscribeId).Distinct().Count();
        }

        /// <summary>
        /// 获取订阅列表
        /// </summary>
        /// <param name="subscribeId"></param>
        /// <returns></returns>
        public List<SubscribeGroupPO> GetList(int subscribeId)
        {
            return Db.Queryable<SubscribeGroupPO>().Where(sg => sg.SubscribeId == subscribeId).ToList();
        }

        /// <summary>
        /// 获取订阅列表
        /// </summary>
        /// <param name="subscribeId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public List<SubscribeGroupPO> GetList(int subscribeId, long groupId)
        {
            return Db.Queryable<SubscribeGroupPO>().Where(sg => sg.SubscribeId == subscribeId && sg.GroupId == groupId).ToList();
        }

        /// <summary>
        /// 查询某个订阅类型的列表
        /// </summary>
        /// <param name="subscribeType"></param>
        /// <returns></returns>
        public List<SubscribeInfo> GetSubscribeInfos(SubscribeType subscribeType)
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
        /// 查询某个群的某个订阅类型的列表
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="subscribeType"></param>
        /// <returns></returns>
        public List<SubscribeInfo> GetSubscribeInfos(long groupId, SubscribeType subscribeType)
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
        /// 查询某个订阅的列表
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="subscribeType"></param>
        /// <returns></returns>
        public List<SubscribeInfo> GetSubscribeInfos(string subscribeCode, SubscribeType subscribeType)
        {
            return Db.Queryable<SubscribeGroupPO>()
            .InnerJoin<SubscribePO>((sg, s) => sg.SubscribeId == s.Id)
            .Where((sg, s) => s.SubscribeCode == subscribeCode && s.SubscribeType == subscribeType)
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


        public List<SubscribeInfo> GetSubscribeInfos(long groupId, int subscribeId)
        {
            return Db.Queryable<SubscribeGroupPO>()
            .InnerJoin<SubscribePO>((sg, s) => sg.SubscribeId == s.Id)
            .Where((sg, s) => sg.SubscribeId == subscribeId && sg.GroupId == groupId)
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
        public int DelBySubscribeId(int subscribeId)
        {
            return Db.Deleteable<SubscribeGroupPO>().Where(o => o.SubscribeId == subscribeId).ExecuteCommand();
        }

        /// <summary>
        /// 删除订阅
        /// </summary>
        /// <param name="subscribeId"></param>
        public int DelBySubscribeId(List<int> subscribeIds)
        {
            return Db.Deleteable<SubscribeGroupPO>().Where(o => subscribeIds.Contains(o.SubscribeId)).ExecuteCommand();
        }

        /// <summary>
        /// 删除订阅
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="subscribeId"></param>
        public int DelBySubscribeId(int subscribeId, long groupId)
        {
            return Db.Deleteable<SubscribeGroupPO>().Where(o => o.GroupId == groupId && o.SubscribeId == subscribeId).ExecuteCommand();
        }

    }
}
