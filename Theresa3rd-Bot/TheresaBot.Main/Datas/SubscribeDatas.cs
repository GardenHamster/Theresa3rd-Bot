using TheresaBot.Main.Business;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Subscribe;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Datas
{
    public class SubscribeDatas
    {
        public static Dictionary<SubscribeType, List<SubscribeTask>> SubscribeTaskMap = new Dictionary<SubscribeType, List<SubscribeTask>>();

        public static void LoadSubscribeTask()
        {
            try
            {
                SubscribeTaskMap = new SubscribeBusiness().getSubscribeTask();
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
            return SubscribeTaskMap[subscribeType].Where(m => m.GroupIdList.Contains(groupId)).ToList();
        }


    }
}
