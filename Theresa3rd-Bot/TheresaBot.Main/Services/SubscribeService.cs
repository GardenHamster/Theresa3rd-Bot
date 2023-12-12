using TheresaBot.Main.Common;
using TheresaBot.Main.Dao;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Mys;
using TheresaBot.Main.Model.Pixiv;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Model.Subscribe;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Services
{
    internal class SubscribeService
    {
        private SubscribeDao subscribeDao;
        private SubscribeGroupDao subscribeGroupDao;

        public SubscribeService()
        {
            subscribeDao = new SubscribeDao();
            subscribeGroupDao = new SubscribeGroupDao();
        }

        /// <summary>
        /// 获取订阅任务
        /// </summary>
        /// <returns></returns>
        public Dictionary<SubscribeType, List<SubscribeTask>> LoadSubscribeTasks()
        {
            var subscribeTaskMap = new Dictionary<SubscribeType, List<SubscribeTask>>();
            var subscribeInfoList = subscribeDao.getSubscribeInfo();
            foreach (SubscribeInfo subscribeInfo in subscribeInfoList)
            {
                SubscribeType subscribeType = subscribeInfo.SubscribeType;
                if (subscribeType == SubscribeType.米游社用户 && subscribeInfo.SubscribeSubType != 0) continue;
                if (subscribeType == SubscribeType.P站画师 && subscribeInfo.SubscribeSubType != 0) continue;
                if (subscribeType == SubscribeType.P站标签 && subscribeInfo.SubscribeSubType != 0) continue;
                if (!subscribeTaskMap.ContainsKey(subscribeType)) subscribeTaskMap[subscribeType] = new List<SubscribeTask>();
                var subscribeTaskList = subscribeTaskMap[subscribeType];
                var subscribeTask = subscribeTaskList.Where(o => o.SubscribeCode == subscribeInfo.SubscribeCode).FirstOrDefault();
                if (subscribeTask is null)
                {
                    subscribeTask = new SubscribeTask(subscribeInfo);
                    subscribeTaskList.Add(subscribeTask);
                }
                subscribeTask.AddGroup(subscribeInfo.GroupId);
            }
            return subscribeTaskMap;
        }

        /// <summary>
        /// 判断是否已存在某个订阅
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="subscribeCode"></param>
        /// <returns></returns>
        public bool CheckExists(long groupId, string subscribeCode, SubscribeType subscribeType)
        {
            var subscribes = subscribeDao.getSubscribes(subscribeCode, subscribeType);
            if (subscribes.Count == 0) return false;
            var groupSubscribes = subscribeGroupDao.isExists()
            return subscribeList;
        }

        /// <summary>
        /// 统计某个订阅的数量
        /// </summary>
        /// <param name="subscribeType"></param>
        /// <returns></returns>
        public int countSubscribes(SubscribeType subscribeType)
        {
            return subscribeGroupDao.countByType(subscribeType);
        }

        /// <summary>
        /// 获取某个群订阅某个类型的列表
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="subscribeType"></param>
        /// <returns></returns>
        public List<SubscribeInfo> getSubscribes(long groupId, SubscribeType subscribeType)
        {
            var returnList = new List<SubscribeInfo>();
            if (groupId > 0)
            {
                returnList.AddRange(subscribeGroupDao.getSubscribes(groupId, subscribeType));
            }
            if (BotConfig.SubscribeGroups.Contains(groupId))
            {
                returnList.AddRange(subscribeGroupDao.getSubscribes(0, subscribeType));
            }
            return returnList;
        }

        /// <summary>
        /// 获取用于前端展示的订阅列表,其中GroupId可能为0
        /// </summary>
        /// <param name="subscribeType"></param>
        /// <returns></returns>
        public List<SubscribeInfo> getSubscribeDatas(SubscribeType subscribeType)
        {
            return subscribeGroupDao.getSubscribes(subscribeType);
        }

        public SubscribePO insertSurscribe(MysUserFullInfo userInfo, string userId)
        {
            SubscribePO dbSubscribe = new SubscribePO();
            dbSubscribe.SubscribeCode = userId;
            dbSubscribe.SubscribeName = userInfo.nickname?.FilterEmoji()?.CutString(50);
            dbSubscribe.SubscribeType = SubscribeType.米游社用户;
            dbSubscribe.SubscribeSubType = 0;
            dbSubscribe.CreateDate = DateTime.Now;
            return subscribeDao.Insert(dbSubscribe);
        }

        public SubscribePO insertSurscribe(PixivUserProfileTop pixivUserInfoDto, string userId)
        {
            SubscribePO dbSubscribe = new SubscribePO();
            dbSubscribe.SubscribeCode = userId;
            dbSubscribe.SubscribeName = pixivUserInfoDto.extraData.meta.UserName.FilterEmoji()?.Trim()?.CutString(200);
            dbSubscribe.SubscribeType = SubscribeType.P站画师;
            dbSubscribe.SubscribeSubType = 0;
            dbSubscribe.CreateDate = DateTime.Now;
            return subscribeDao.Insert(dbSubscribe);
        }

        public SubscribePO insertSurscribe(PixivFollowUser pixivFollowUser, DateTime createDate)
        {
            SubscribePO dbSubscribe = new SubscribePO();
            dbSubscribe.SubscribeCode = pixivFollowUser.userId;
            dbSubscribe.SubscribeName = pixivFollowUser.userName.FilterEmoji().CutString(200);
            dbSubscribe.SubscribeType = SubscribeType.P站画师;
            dbSubscribe.SubscribeSubType = 0;
            dbSubscribe.CreateDate = createDate;
            return subscribeDao.Insert(dbSubscribe);
        }

        public SubscribePO insertSurscribe(string pixivTag)
        {
            SubscribePO dbSubscribe = new SubscribePO();
            dbSubscribe.SubscribeCode = pixivTag;
            dbSubscribe.SubscribeName = pixivTag;
            dbSubscribe.SubscribeType = SubscribeType.P站标签;
            dbSubscribe.SubscribeSubType = 0;
            dbSubscribe.CreateDate = DateTime.Now;
            return subscribeDao.Insert(dbSubscribe);
        }

        public SubscribeGroupPO insertSubscribeGroup(long groupId, int subscribeId)
        {
            SubscribeGroupPO subscribeGroup = new SubscribeGroupPO();
            subscribeGroup.GroupId = groupId;
            subscribeGroup.SubscribeId = subscribeId;
            return subscribeGroupDao.Insert(subscribeGroup);
        }

        public int updateSubscribeGroup(SubscribeGroupPO subscribeGroup)
        {
            return subscribeGroupDao.Update(subscribeGroup);
        }

        public SubscribePO getSubscribe(int subscribeId)
        {
            return subscribeDao.getSubscribe(subscribeId);
        }

        public SubscribePO getSubscribe(string subscribeCode, SubscribeType subscribeType)
        {
            return subscribeDao.getSubscribe(subscribeCode, subscribeType);
        }

        public SubscribePO getSubscribe(string subscribeCode, SubscribeType subscribeType, int subscribeSubType)
        {
            return subscribeDao.getSubscribe(subscribeCode, subscribeType, subscribeSubType);
        }

        public SubscribeGroupPO getSubscribeGroup(long groupId, int subscribeId)
        {
            return subscribeDao.getSubscribeGroup(groupId, subscribeId);
        }

        public List<SubscribePO> getSubscribes(SubscribeType subscribeType)
        {
            return subscribeDao.getSubscribes(subscribeType);
        }

        public bool isSubscribed(long groupId, int subscribeId)
        {
            return subscribeGroupDao.isExists(groupId, subscribeId);
        }

        public int deleteSubscribe(int subscribeId)
        {
            return subscribeGroupDao.delBySubscribeId(subscribeId);
        }

        public void deleteSubscribe(List<int> subscribeIds)
        {
            foreach (int subscribeId in subscribeIds)
            {
                subscribeGroupDao.delBySubscribeId(subscribeId);
            }
        }

        public int deleteSubscribe(long groupId, int subscribeId)
        {
            return subscribeGroupDao.delBySubscribeId(groupId, subscribeId);
        }

        public int deleteSubscribeGroup(List<int> ids)
        {
            return subscribeGroupDao.DeleteByIds(ids);
        }

    }
}
