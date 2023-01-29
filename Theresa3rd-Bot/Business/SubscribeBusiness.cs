using System;
using System.Collections.Generic;
using System.Linq;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Dao;
using Theresa3rd_Bot.Model.Mys;
using Theresa3rd_Bot.Model.Pixiv;
using Theresa3rd_Bot.Model.PO;
using Theresa3rd_Bot.Model.Subscribe;
using Theresa3rd_Bot.Type;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Business
{
    public class SubscribeBusiness
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

        public SubscribePO insertSurscribe(MysUserFullInfo userInfo, string userId)
        {
            SubscribePO dbSubscribe = new SubscribePO();
            dbSubscribe.SubscribeCode = userId;
            dbSubscribe.SubscribeName = StringHelper.filterEmoji(userInfo.nickname)?.filterEmoji().cutString(50);
            dbSubscribe.SubscribeDescription = userInfo.introduce?.filterEmoji().cutString(200);
            dbSubscribe.SubscribeType = SubscribeType.米游社用户;
            dbSubscribe.SubscribeSubType = 0;
            dbSubscribe.Isliving = false;
            dbSubscribe.CreateDate = DateTime.Now;
            return subscribeDao.Insert(dbSubscribe);
        }

        public SubscribePO insertSurscribe(PixivResult<PixivUserInfo> pixivUserInfoDto, string userId)
        {
            string userName = StringHelper.filterEmoji(pixivUserInfoDto.body.extraData.meta.title.Replace("- pixiv", "").Trim().cutString(200));
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

        public int delSubscribeGroup(int subscribeId)
        {
            return subscribeGroupDao.delSubscribeGroup(subscribeId);
        }

        public int delSubscribeGroup(long groupId, int subscribeId)
        {
            return subscribeGroupDao.delSubscribeGroup(groupId, subscribeId);
        }


    }
}
