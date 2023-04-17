using System.ComponentModel.Design;
using TheresaBot.Main.Common;
using TheresaBot.Main.Dao;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Mys;
using TheresaBot.Main.Model.Pixiv;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Model.Subscribe;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Business
{
    internal class SubscribeBusiness
    {
        private SubscribeDao subscribeDao;
        private SubscribeGroupDao subscribeGroupDao;

        public SubscribeBusiness()
        {
            subscribeDao = new SubscribeDao();
            subscribeGroupDao = new SubscribeGroupDao();
        }

        /// <summary>
        /// 获取订阅任务
        /// </summary>
        /// <returns></returns>
        public Dictionary<SubscribeType, List<SubscribeTask>> getSubscribeTask()
        {
            Dictionary<SubscribeType, List<SubscribeTask>> subscribeTaskMap = new Dictionary<SubscribeType, List<SubscribeTask>>();
            List<SubscribeInfo> subscribeInfoList = subscribeDao.getSubscribeInfo();
            foreach (SubscribeInfo subscribeInfo in subscribeInfoList)
            {
                SubscribeType subscribeType = subscribeInfo.SubscribeType;

                if (subscribeType == SubscribeType.米游社用户 && subscribeInfo.SubscribeSubType != 0) continue;
                if (subscribeType == SubscribeType.P站画师 && subscribeInfo.SubscribeSubType != 0) continue;
                if (subscribeType == SubscribeType.P站标签 && subscribeInfo.SubscribeSubType != 0) continue;

                if (!subscribeTaskMap.ContainsKey(subscribeType)) subscribeTaskMap[subscribeType] = new List<SubscribeTask>();
                List<SubscribeTask> subscribeTaskList = subscribeTaskMap[subscribeType];
                SubscribeTask subscribeTask = subscribeTaskList.Where(o => o.SubscribeCode == subscribeInfo.SubscribeCode).FirstOrDefault();
                if (subscribeTask is null)
                {
                    subscribeTask = new SubscribeTask(subscribeInfo);
                    subscribeTaskList.Add(subscribeTask);
                }
                if (subscribeInfo.GroupId == 0)
                {
                    foreach (long groupId in BotConfig.PermissionsConfig.SubscribeGroups)
                    {
                        if (subscribeTask.GroupIdList.Contains(groupId) == false) subscribeTask.GroupIdList.Add(groupId);
                    }
                    continue;
                }
                if (subscribeTask.GroupIdList.Contains(subscribeInfo.GroupId) == false)
                {
                    subscribeTask.GroupIdList.Add(subscribeInfo.GroupId);
                    continue;
                }
            }
            return subscribeTaskMap;
        }

        public List<SubscribeInfo> getSubscribes(long groupId, SubscribeType subscribeType)
        {
            var subscribeList = subscribeGroupDao.getSubscribes(groupId, subscribeType);
            if (BotConfig.PermissionsConfig.SubscribeGroups.Contains(groupId))
            {
                return subscribeList;
            }
            else
            {
                return subscribeList.Where(o => o.GroupId > 0 && o.GroupId == groupId).ToList();
            }
        }

        public SubscribePO insertSurscribe(MysUserFullInfo userInfo, string userId)
        {
            SubscribePO dbSubscribe = new SubscribePO();
            dbSubscribe.SubscribeCode = userId;
            dbSubscribe.SubscribeName = userInfo.nickname?.filterEmoji()?.cutString(50);
            dbSubscribe.SubscribeDescription = userInfo.introduce?.filterEmoji()?.cutString(200);
            dbSubscribe.SubscribeType = SubscribeType.米游社用户;
            dbSubscribe.SubscribeSubType = 0;
            dbSubscribe.Isliving = false;
            dbSubscribe.CreateDate = DateTime.Now;
            return subscribeDao.Insert(dbSubscribe);
        }

        public SubscribePO insertSurscribe(PixivUserProfileTop pixivUserInfoDto, string userId)
        {
            string userName = pixivUserInfoDto.extraData.meta.UserName.filterEmoji()?.Trim()?.cutString(200);
            SubscribePO dbSubscribe = new SubscribePO();
            dbSubscribe = new SubscribePO();
            dbSubscribe.SubscribeCode = userId;
            dbSubscribe.SubscribeName = userName;
            dbSubscribe.SubscribeDescription = userName;
            dbSubscribe.SubscribeType = SubscribeType.P站画师;
            dbSubscribe.SubscribeSubType = 0;
            dbSubscribe.Isliving = false;
            dbSubscribe.CreateDate = DateTime.Now;
            return subscribeDao.Insert(dbSubscribe);
        }

        public SubscribePO insertSurscribe(PixivFollowUser pixivFollowUser, DateTime createDate)
        {
            string userName = pixivFollowUser.userName.filterEmoji().cutString(200);
            SubscribePO dbSubscribe = new SubscribePO();
            dbSubscribe.SubscribeCode = pixivFollowUser.userId;
            dbSubscribe.SubscribeName = userName;
            dbSubscribe.SubscribeDescription = userName;
            dbSubscribe.SubscribeType = SubscribeType.P站画师;
            dbSubscribe.SubscribeSubType = 0;
            dbSubscribe.Isliving = false;
            dbSubscribe.CreateDate = createDate;
            return subscribeDao.Insert(dbSubscribe);
        }

        public SubscribePO insertSurscribe(string pixivTag)
        {
            SubscribePO dbSubscribe = new SubscribePO();
            dbSubscribe = new SubscribePO();
            dbSubscribe.SubscribeCode = pixivTag;
            dbSubscribe.SubscribeName = pixivTag;
            dbSubscribe.SubscribeDescription = pixivTag;
            dbSubscribe.SubscribeType = SubscribeType.P站标签;
            dbSubscribe.SubscribeSubType = 0;
            dbSubscribe.Isliving = false;
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

        public List<SubscribePO> getSubscribes(string subscribeCode, SubscribeType subscribeType)
        {
            return subscribeDao.getSubscribes(subscribeCode, subscribeType);
        }

        public bool isExistsSubscribeGroup(long groupId, int subscribeId)
        {
            return subscribeGroupDao.isExistsSubscribeGroup(groupId, subscribeId);
        }

        public int cancleSubscribe(int subscribeId)
        {
            return subscribeGroupDao.delSubscribeGroup(subscribeId);
        }

        public int cancleSubscribe(long groupId, int subscribeId)
        {
            return subscribeGroupDao.delSubscribeGroup(groupId, subscribeId);
        }


    }
}
