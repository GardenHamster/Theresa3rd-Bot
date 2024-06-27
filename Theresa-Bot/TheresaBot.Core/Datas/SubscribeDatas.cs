﻿using TheresaBot.Core.Helper;
using TheresaBot.Core.Model.Subscribe;
using TheresaBot.Core.Services;
using TheresaBot.Core.Type;

namespace TheresaBot.Core.Datas
{
    internal static class SubscribeDatas
    {
        private static Dictionary<SubscribeType, List<SubscribeTask>> SubscribeTaskMap = new Dictionary<SubscribeType, List<SubscribeTask>>();

        public static void LoadSubscribeTask()
        {
            try
            {
                SubscribeTaskMap = new SubscribeService().LoadSubscribeTasks();
                LogHelper.Info("订阅任务加载完成...");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "订阅任务加载失败");
            }
        }

        public static List<SubscribeTask> GetSubscribeTasks(SubscribeType subscribeType)
        {
            if (SubscribeTaskMap.ContainsKey(subscribeType) == false) return new();
            return SubscribeTaskMap[subscribeType] ?? new List<SubscribeTask>();
        }

        public static List<SubscribeTask> GetSubscribeTasks(SubscribeType subscribeType, long groupId)
        {
            if (SubscribeTaskMap.ContainsKey(subscribeType) == false) return new List<SubscribeTask>();
            return SubscribeTaskMap[subscribeType].Where(m => m.SubscribeGroups.Contains(groupId)).ToList();
        }


    }
}