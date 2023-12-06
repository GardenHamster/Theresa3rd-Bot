using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Subscribe;
using TheresaBot.Main.Services;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Datas
{
    internal static class SubscribeDatas
    {
        private static Dictionary<SubscribeType, List<SubscribeTask>> SubscribeTaskMap = new Dictionary<SubscribeType, List<SubscribeTask>>();

        public static void LoadSubscribeTask()
        {
            try
            {
                SubscribeTaskMap = new SubscribeService().getSubscribeTask();
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
