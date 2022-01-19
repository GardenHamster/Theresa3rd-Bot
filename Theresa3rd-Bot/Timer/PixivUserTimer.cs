using Theresa3rd_Bot.Common;

namespace Theresa3rd_Bot.Timer
{
    public static class PixivUserTimer
    {
        private static System.Timers.Timer timer;

        public static void init()
        {
            timer = new System.Timers.Timer();
            timer.Interval = BotConfig.SubscribeConfig.PixivUser.ScanInterval;
            timer.AutoReset = true;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(HandlerMethod);
            timer.Enabled = true;
        }

        private static void HandlerMethod(object source, System.Timers.ElapsedEventArgs e)
        {
            TypeModel subscribeType = SubscribeSourceType.PixivUser;
            if (Setting.Subscribe.SubscribeTaskMap.ContainsKey(subscribeType.TypeId) == false) return;
            List<SubscribeTask> subscribeTaskList = Setting.Subscribe.SubscribeTaskMap[subscribeType.TypeId];
            if (subscribeTaskList == null || subscribeTaskList.Count == 0) return;
            foreach (SubscribeTask subscribeTask in subscribeTaskList)
            {
                try
                {
                    List<PixivSubscribe> pixivSubscribeList = pixivBusiness.getPixivUserNewestWork(subscribeTask.SubscribeCode, subscribeTask.SubscribeId, Setting.Subscribe.PixivEachRead);
                    if (pixivSubscribeList == null || pixivSubscribeList.Count == 0) continue;
                    sendGroupPixivUserSubscribe(subscribeTask, pixivSubscribeList);
                }
                catch (Exception ex)
                {
                    LogHelper.LogError(ex);
                    string errorMessage = string.Format("pixiv用户{0}订阅失败", subscribeTask.SubscribeCode);
                    CQLog.Error(errorMessage, ex.Message, ex.StackTrace);
                    BusinessHelper.sendErrorMessage(CQApi, ex, errorMessage);
                }
                finally
                {
                    Thread.Sleep(1000);
                }
            }
        }





    }
}
