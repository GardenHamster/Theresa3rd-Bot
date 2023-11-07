using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Timers
{
    public static class TimerManager
    {
        /// <summary>
        /// 初始化订阅任务
        /// </summary>
        public static void InitTimers(BaseSession session, BaseReporter reporter)
        {
            DestroyScanTimers();
            HeartbeatTimer.Init();
            var subscribeConfig = BotConfig.SubscribeConfig;
            if (subscribeConfig is null) return;
            if (subscribeConfig.PixivUser != null && subscribeConfig.PixivUser.Enable)
            {
                PixivUserScanTimer.Init(session, reporter);
                LogHelper.Info($"pixiv用户订阅任务启动完毕...");
            }
            if (subscribeConfig.PixivTag != null && subscribeConfig.PixivTag.Enable)
            {
                PixivTagScanTimer.Init(session, reporter);
                LogHelper.Info($"pixiv标签订阅任务启动完毕...");
            }
            if (subscribeConfig.Miyoushe != null && subscribeConfig.Miyoushe.Enable)
            {
                MysUserScanTimer.Init(session, reporter);
                LogHelper.Info($"米游社订阅任务启动完毕...");
            }
        }

        public static void DestroyScanTimers()
        {
            if (PixivUserScanTimer.Destroy())
            {
                LogHelper.Info($"pixiv用户订阅任务已停止运行...");
            }
            if (PixivTagScanTimer.Destroy())
            {
                LogHelper.Info($"pixiv标签订阅任务已停止运行...");
            }
            if (MysUserScanTimer.Destroy())
            {
                LogHelper.Info($"米游社订阅任务已停止运行...");
            }
        }


    }
}
