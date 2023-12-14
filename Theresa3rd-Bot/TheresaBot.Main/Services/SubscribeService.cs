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

        public SubscribeService()
        {
            subscribeDao = new SubscribeDao();
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

        public SubscribePO AddSubscribe(MysUserInfo userInfo, string userId)
        {
            SubscribePO dbSubscribe = new SubscribePO();
            dbSubscribe.SubscribeCode = userId;
            dbSubscribe.SubscribeName = userInfo.nickname?.FilterEmoji()?.CutString(50);
            dbSubscribe.SubscribeType = SubscribeType.米游社用户;
            dbSubscribe.SubscribeSubType = 0;
            dbSubscribe.CreateDate = DateTime.Now;
            return subscribeDao.Insert(dbSubscribe);
        }

        public SubscribePO AddSurscribe(PixivUserProfileTop pixivUserInfoDto, string userId)
        {
            SubscribePO dbSubscribe = new SubscribePO();
            dbSubscribe.SubscribeCode = userId;
            dbSubscribe.SubscribeName = pixivUserInfoDto.extraData.meta.UserName.FilterEmoji()?.Trim()?.CutString(200);
            dbSubscribe.SubscribeType = SubscribeType.P站画师;
            dbSubscribe.SubscribeSubType = 0;
            dbSubscribe.CreateDate = DateTime.Now;
            return subscribeDao.Insert(dbSubscribe);
        }

        public SubscribePO AddSurscribe(PixivFollowUser pixivFollowUser, DateTime createDate)
        {
            SubscribePO dbSubscribe = new SubscribePO();
            dbSubscribe.SubscribeCode = pixivFollowUser.userId;
            dbSubscribe.SubscribeName = pixivFollowUser.userName.FilterEmoji().CutString(200);
            dbSubscribe.SubscribeType = SubscribeType.P站画师;
            dbSubscribe.SubscribeSubType = 0;
            dbSubscribe.CreateDate = createDate;
            return subscribeDao.Insert(dbSubscribe);
        }

        public SubscribePO AddSurscribe(string pixivTag)
        {
            SubscribePO dbSubscribe = new SubscribePO();
            dbSubscribe.SubscribeCode = pixivTag;
            dbSubscribe.SubscribeName = pixivTag;
            dbSubscribe.SubscribeType = SubscribeType.P站标签;
            dbSubscribe.SubscribeSubType = 0;
            dbSubscribe.CreateDate = DateTime.Now;
            return subscribeDao.Insert(dbSubscribe);
        }

        public SubscribePO GetSubscribe(int subscribeId)
        {
            return subscribeDao.getSubscribe(subscribeId);
        }

        public SubscribePO GetSubscribe(string subscribeCode, SubscribeType subscribeType)
        {
            return subscribeDao.getSubscribe(subscribeCode, subscribeType);
        }

    }
}
