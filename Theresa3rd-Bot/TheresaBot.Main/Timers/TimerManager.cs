using Quartz;
using Quartz.Impl;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Timers
{
    public static class TimerManager
    {
        /// <summary>
        /// 初始化定时提醒任务
        /// </summary>
        public static void initReminderJob(BaseSession session, BaseReporter reporter)
        {
            try
            {
                ReminderConfig reminderConfig = BotConfig.ReminderConfig;
                if (reminderConfig is null) return;
                if (reminderConfig.Enable == false) return;
                foreach (var item in reminderConfig.Timers) createReminderJob(item, session, reporter);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"定时提醒任务初始化失败");
            }
        }

        /// <summary>
        /// 初始化定时色图任务
        /// </summary>
        /// <param name="session"></param>
        public static void initTimingSetuJob(BaseSession session, BaseReporter reporter)
        {
            try
            {
                TimingSetuConfig timingSetuConfig = BotConfig.TimingSetuConfig;
                if (timingSetuConfig is null) return;
                if (timingSetuConfig.Enable == false) return;
                if (timingSetuConfig.Timers is null) return;
                if (timingSetuConfig.Timers.Count == 0) return;
                List<TimingSetuTimer> timers = timingSetuConfig.Timers.Take(10).ToList();
                foreach (var item in timers) createTimingSetuJob(item, session, reporter);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"定时涩图任务初始化失败");
            }
        }

        /// <summary>
        /// 初始化订阅任务
        /// </summary>
        public static void initSubscribeTimers(BaseSession session, BaseReporter reporter)
        {
            SubscribeConfig subscribeConfig = BotConfig.SubscribeConfig;
            if (subscribeConfig is null) return;
            if (subscribeConfig.PixivUser != null && subscribeConfig.PixivUser.Enable)
            {
                PixivUserTimer.init(session, reporter);
                LogHelper.Info($"pixiv用户订阅任务启动完毕...");
            }
            if (subscribeConfig.PixivTag != null && subscribeConfig.PixivTag.Enable)
            {
                PixivTagTimer.init(session, reporter);
                LogHelper.Info($"pixiv标签订阅任务启动完毕...");
            }
            if (subscribeConfig.Miyoushe != null && subscribeConfig.Miyoushe.Enable)
            {
                MysUserTimer.init(session, reporter);
                LogHelper.Info($"米游社订阅任务启动完毕...");
            }
        }

        /// <summary>
        /// 初始化清理任务
        /// </summary>
        public static async void initClearJobAsync(BaseSession session, BaseReporter reporter)
        {
            try
            {
                string clearCron = "0 0 4 * * ?";
                ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create().WithCronSchedule(clearCron).Build();
                IJobDetail jobDetail = JobBuilder.Create<ClearJob>().WithIdentity("ClearJob", "ClearJob").Build();//创建作业
                IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                jobDetail.JobDataMap.Put("BaseSession", session);
                jobDetail.JobDataMap.Put("BaseReporter", reporter);
                await scheduler.ScheduleJob(jobDetail, trigger);
                await scheduler.Start();
                LogHelper.Info($"定时清理任务初始化完毕...");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"清定时清理任务初始化失败");
            }
        }

        /// <summary>
        /// 初始化cookie检查任务
        /// </summary>
        /// <param name="session"></param>
        /// <param name="reporter"></param>
        public static async void initCookieJobAsync(BaseSession session, BaseReporter reporter)
        {
            try
            {
                string cookieCron = "0 0 9 * * ?";
                ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create().WithCronSchedule(cookieCron).Build();
                IJobDetail jobDetail = JobBuilder.Create<CookieJob>().WithIdentity("CookieJob", "CookieJob").Build();//创建作业
                IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                jobDetail.JobDataMap.Put("BaseSession", session);
                jobDetail.JobDataMap.Put("BaseReporter", reporter);
                await scheduler.ScheduleJob(jobDetail, trigger);
                await scheduler.Start();
                LogHelper.Info($"Cookie检查定时器启动完毕...");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"Cookie检查定时器启动失败");
            }
        }


        private static async void createReminderJob(ReminderTimer reminderTimer, BaseSession session, BaseReporter reporter)
        {
            try
            {
                ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create().WithCronSchedule(reminderTimer.Cron).Build();
                IJobDetail jobDetail = JobBuilder.Create<ReminderJob>().WithIdentity(reminderTimer.GetHashCode().ToString(), "ReminderJob").Build();
                IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                jobDetail.JobDataMap.Put("ReminderTimer", reminderTimer);
                jobDetail.JobDataMap.Put("BaseSession", session);
                jobDetail.JobDataMap.Put("BaseReporter", reporter);
                await scheduler.ScheduleJob(jobDetail, trigger);
                await scheduler.Start();
                LogHelper.Info($"定时提醒任务[{reminderTimer.Name}]启动完毕...");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"定时提醒任务[{reminderTimer.Name}]启动失败");
            }
        }

        private static async void createTimingSetuJob(TimingSetuTimer timingSetuTimer, BaseSession session, BaseReporter reporter)
        {
            try
            {
                if (timingSetuTimer.Enable == false) return;
                ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create().WithCronSchedule(timingSetuTimer.Cron).Build();
                IJobDetail jobDetail = JobBuilder.Create<TimingSetuJob>().WithIdentity(timingSetuTimer.GetHashCode().ToString(), "TimingSetuJob").Build();
                IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                jobDetail.JobDataMap.Put("TimingSetuTimer", timingSetuTimer);
                jobDetail.JobDataMap.Put("BaseReporter", reporter);
                jobDetail.JobDataMap.Put("BaseSession", session);
                await scheduler.ScheduleJob(jobDetail, trigger);
                await scheduler.Start();
                LogHelper.Info($"定时涩图任务[{timingSetuTimer.Name}]启动完毕...");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"定时涩图任务[{timingSetuTimer.Name}]启动失败");
            }
        }



    }
}
