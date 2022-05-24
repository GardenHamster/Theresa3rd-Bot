using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Dao;
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

        public SubscribeBusiness()
        {
            subscribeDao = new SubscribeDao();
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
                if (!subscribeTaskMap.ContainsKey(subscribeType)) subscribeTaskMap[subscribeType] = new List<SubscribeTask>();
                List<SubscribeTask> subscribeTaskList = subscribeTaskMap[subscribeType];
                SubscribeTask subscribeTask = subscribeTaskList.Where(o => o.SubscribeInfo.SubscribeCode == subscribeInfo.SubscribeCode).FirstOrDefault();
                if (subscribeTask == null)
                {
                    subscribeTask = new SubscribeTask(subscribeInfo);
                    subscribeTaskList.Add(subscribeTask);
                }
                if (subscribeTask.GroupIdList.Contains(subscribeInfo.GroupId) == false)
                {
                    subscribeTask.GroupIdList.Add(subscribeInfo.GroupId);
                }
            }
            return subscribeTaskMap;
        }



    }
}
