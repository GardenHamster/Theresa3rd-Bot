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
            //初始化字典
            Dictionary<SubscribeType, List<SubscribeTask>> subscribeTaskMap = new Dictionary<SubscribeType, List<SubscribeTask>>();
            foreach (var item in Enum.GetValues(typeof(SubscribeType)))
            {
                SubscribeType subscribeType = (SubscribeType)Convert.ToInt32(item);
                subscribeTaskMap[subscribeType] = new List<SubscribeTask>();
            }

            List<SubscribeInfo> subscribeInfoList = subscribeDao.getSubscribeInfo();
            foreach (var item in subscribeTaskMap)
            {
                SubscribeType subscribeType = item.Key;//订阅大类
                List<SubscribeTask> subscribeTaskList = item.Value;//订阅大类任务列表
                List<SubscribeInfo> subscribeList = subscribeInfoList.Where(o => o.SubscribeType == subscribeType).ToList();//订阅大类列表

                List<SubscribeInfo> subscribeAllList = subscribeList.Where(o => o.SubscribeSubType == 0).ToList();
                foreach (SubscribeInfo subscribeInfo in subscribeAllList) addSubscribeTask(subscribeTaskList, subscribeInfo);

                List<SubscribeInfo> subscribeCurrentList = subscribeList.Where(o => o.SubscribeSubType != 0).ToList();
                foreach (SubscribeInfo subscribeInfo in subscribeCurrentList) addSubscribeTask(subscribeTaskList, subscribeInfo);
            }
            return subscribeTaskMap;
        }

        public void addSubscribeTask(List<SubscribeTask> subscribeTaskList, SubscribeInfo subscribeInfo)
        {
            SubscribeTask subscribeTask = subscribeTaskList.Where(o => o.SubscribeCode == subscribeInfo.SubscribeCode && o.SubscribeSubType == 0).FirstOrDefault();
            if (subscribeTask == null)
            {
                subscribeTask = subscribeTaskList.Where(o => o.SubscribeCode == subscribeInfo.SubscribeCode && o.SubscribeSubType == subscribeInfo.SubscribeSubType).FirstOrDefault();
            }
            if (subscribeTask == null)
            {
                subscribeTask = new SubscribeTask(subscribeInfo);
                subscribeTaskList.Add(subscribeTask);
            }

            //groupId=0表示推送给所有有订阅权限的群
            if (subscribeInfo.GroupId == 0)
            {
                foreach (long groupId in BotConfig.PermissionsConfig.SubscribeGroups)
                {
                    if (subscribeTask.GroupIdList.Contains(groupId) == false) subscribeTask.GroupIdList.Add(groupId);
                }
                return;
            }
            if (subscribeTask.GroupIdList.Contains(subscribeInfo.GroupId) == false)//将要推动的群号添加到集合中
            {
                subscribeTask.GroupIdList.Add(subscribeInfo.GroupId);
            }
        }

        public SubscribePO insertSurscribe(MysUserFullInfo userInfo, MysSectionType mysSectionType, string userId)
        {
            SubscribePO dbSubscribe = new SubscribePO();
            dbSubscribe.SubscribeCode = userId;
            dbSubscribe.SubscribeName = StringHelper.filterEmoji(userInfo.nickname)?.filterEmoji().cutString(50);
            dbSubscribe.SubscribeDescription = userInfo.introduce?.filterEmoji().cutString(200);
            dbSubscribe.SubscribeType = SubscribeType.米游社用户;
            dbSubscribe.SubscribeSubType = (int)mysSectionType;
            dbSubscribe.Isliving = false;
            dbSubscribe.CreateDate = DateTime.Now;
            return subscribeDao.Insert(dbSubscribe);
        }

        public SubscribePO insertSurscribe(PixivUserInfoDto pixivUserInfoDto, string userId)
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

        public SubscribeGroupPO getSubscribeGroup(long groupId, long subscribeId)
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
