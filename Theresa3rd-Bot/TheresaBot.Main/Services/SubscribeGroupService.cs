using TheresaBot.Main.Common;
using TheresaBot.Main.Dao;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Model.Subscribe;
using TheresaBot.Main.Type;
using TheresaBot.Main.Type.StepOptions;

namespace TheresaBot.Main.Services
{
    internal class SubscribeGroupService
    {
        private SubscribeGroupDao subscribeGroupDao;

        public SubscribeGroupService()
        {
            subscribeGroupDao = new SubscribeGroupDao();
        }

        /// <summary>
        /// 添加关联数据
        /// </summary>
        /// <param name="subscribeId"></param>
        /// <param name="groupId"></param>
        /// <param name="pushType"></param>
        public void AddGroupSubscribe(int subscribeId, PushType pushType, long groupId)
        {
            var subscribeGroupId = pushType == PushType.SubscribableGroup ? 0 : groupId;
            var subscribeGroupList = subscribeGroupDao.GetList(subscribeId, groupId);
            if (subscribeGroupList.Count > 0) return;//已存在数据时，不再添加
            var subscribeGroup = new SubscribeGroupPO();
            subscribeGroup.GroupId = subscribeGroupId;
            subscribeGroup.SubscribeId = subscribeId;
            subscribeGroupDao.Insert(subscribeGroup);
        }

        /// <summary>
        /// 判断某个群是否已存在某个订阅
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="subscribeId"></param>
        /// <returns></returns>
        public bool IsSubscribed(int subscribeId, long groupId)
        {
            var subscribes = subscribeGroupDao.GetList(subscribeId);
            if (subscribes.Any(o => o.GroupId == 0) && BotConfig.SubscribeGroups.Contains(groupId)) return true;
            if (subscribes.Any(o => o.GroupId == groupId)) return true;
            return false;
        }

        /// <summary>
        /// 获取某个群订阅某个类型的列表
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="subscribeType"></param>
        /// <returns></returns>
        public List<SubscribeInfo> GetSubscribeInfos(long groupId, SubscribeType subscribeType)
        {
            var returnList = new List<SubscribeInfo>();
            if (groupId > 0)
            {
                returnList.AddRange(subscribeGroupDao.GetSubscribeInfos(groupId, subscribeType));
            }
            if (BotConfig.SubscribeGroups.Contains(groupId))
            {
                returnList.AddRange(subscribeGroupDao.GetSubscribeInfos(0, subscribeType));
            }
            return returnList;
        }

        /// <summary>
        /// 获取订阅列表
        /// </summary>
        /// <param name="subscribeCode"></param>
        /// <param name="subscribeType"></param>
        /// <returns></returns>
        public List<SubscribeInfo> GetSubscribes(string subscribeCode, SubscribeType subscribeType)
        {
            return subscribeGroupDao.GetSubscribeInfos(subscribeCode, subscribeType);
        }

        /// <summary>
        /// 获取用于前端展示的订阅列表,其中GroupId可能为0
        /// </summary>
        /// <param name="subscribeType"></param>
        /// <returns></returns>
        public List<SubscribeInfo> GetSubscribeInfos(SubscribeType subscribeType)
        {
            return subscribeGroupDao.GetSubscribeInfos(subscribeType);
        }

        /// <summary>
        /// 统计某个订阅的数量
        /// </summary>
        /// <param name="subscribeType"></param>
        /// <returns></returns>
        public int CountSubscribes(SubscribeType subscribeType)
        {
            return subscribeGroupDao.CountByType(subscribeType);
        }

        public int DeleteBySubscribeId(int subscribeId)
        {
            return subscribeGroupDao.DelBySubscribeId(subscribeId);
        }

        public void DeleteBySubscribeId(List<int> subscribeIds)
        {
            subscribeGroupDao.DelBySubscribeId(subscribeIds);
        }

        public int DeleteById(List<int> ids)
        {
            return subscribeGroupDao.DeleteByIds(ids);
        }

    }
}
