using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using TheresaBot.Main.Common;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Helper;

namespace TheresaBot.Main.Timer
{
    public static class TimerManager
    {
        public static void init()
        {
            initReminderJob();//初始化定时提醒任务
            initTimingSetuJob();//初始化定时色图任务
            initSubscribeTimers();//初始化订阅任务
            initClearJobAsync();//初始化清理任务
            initCookieJobAsync();//初始化cookie检查任务
        }

        private static void initReminderJob()
        {
            try
            {
                ReminderConfig reminderConfig = BotConfig.ReminderConfig;
                if (reminderConfig is null) return;
                if (reminderConfig.Enable == false) return;
                foreach (var item in reminderConfig.Timers) createReminderJob(item);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"定时提醒任务初始化失败");
            }
        }

        private static void initTimingSetuJob()
        {
            try
            {
                TimingSetuConfig timingSetuConfig = BotConfig.TimingSetuConfig;
                if (timingSetuConfig is null) return;
                if (timingSetuConfig.Enable == false) return;
                if (timingSetuConfig.Timers is null) return;
                if (timingSetuConfig.Timers.Count == 0) return;
                List<TimingSetuTimer> timers = timingSetuConfig.Timers.Take(10).ToList();
                foreach (var item in timers) createTimingSetuJob(item);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"定时涩图任务初始化失败");
            }
        }

        private static async void initClearJobAsync()
        {
            try
            {
                string clearCron = "0 0 4 * * ?";
                ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create().WithCronSchedule(clearCron).Build();
                IJobDetail jobDetail = JobBuilder.Create<ClearJob>().WithIdentity("ClearJob", "ClearJob").Build();//创建作业
                IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                await scheduler.ScheduleJob(jobDetail, trigger);
                await scheduler.Start();
                LogHelper.Info($"定时清理任务初始化完毕...");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"清定时清理任务初始化失败");
            }
        }

        private static async void initCookieJobAsync()
        {
            try
            {
                string cookieCron = "0 0 9 * * ?";
                ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create().WithCronSchedule(cookieCron).Build();
                IJobDetail jobDetail = JobBuilder.Create<CookieJob>().WithIdentity("CookieJob", "CookieJob").Build();//创建作业
                IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                await scheduler.ScheduleJob(jobDetail, trigger);
                await scheduler.Start();
                LogHelper.Info($"Cookie检查定时器启动完毕...");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"Cookie检查定时器启动失败");
            }
        }

        private static void initSubscribeTimers()
        {
            SubscribeConfig subscribeConfig = BotConfig.SubscribeConfig;
            if (subscribeConfig is null) return;
            if (subscribeConfig.PixivUser != null && subscribeConfig.PixivUser.Enable)
            {
                PixivUserTimer.init();
                LogHelper.Info($"pixiv用户订阅任务启动完毕...");
            }
            if (subscribeConfig.PixivTag != null && subscribeConfig.PixivTag.Enable)
            {
                PixivTagTimer.init();
                LogHelper.Info($"pixiv标签订阅任务启动完毕...");
            }
            if (subscribeConfig.Mihoyo != null && subscribeConfig.Mihoyo.Enable)
            {
                MysUserTimer.init();
                LogHelper.Info($"米游社订阅任务启动完毕...");
            }
        }


        private static async void createReminderJob(ReminderTimer reminderTimer)
        {
            try
            {
                ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create().WithCronSchedule(reminderTimer.Cron).Build();
                IJobDetail jobDetail = JobBuilder.Create<ReminderJob>().WithIdentity(reminderTimer.GetHashCode().ToString(), "ReminderJob").Build();
                IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                jobDetail.JobDataMap.Put("ReminderTimer", reminderTimer);
                await scheduler.ScheduleJob(jobDetail, trigger);
                await scheduler.Start();
                LogHelper.Info($"定时提醒任务[{reminderTimer.Name}]启动完毕...");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"定时提醒任务[{reminderTimer.Name}]启动失败");
            }
        }

        private static async void createTimingSetuJob(TimingSetuTimer timingSetuTimer)
        {
            try
            {
                ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create().WithCronSchedule(timingSetuTimer.Cron).Build();
                IJobDetail jobDetail = JobBuilder.Create<TimingSetuJob>().WithIdentity(timingSetuTimer.GetHashCode().ToString(), "TimingSetuJob").Build();
                IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                jobDetail.JobDataMap.Put("TimingSetuTimer", timingSetuTimer);
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
