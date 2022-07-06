using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Model.Config;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Timer
{
    public static class TimerManager
    {
        public static void init()
        {
            initCustomJob();//初始化配置列表中的定时器
            initSubscribeTimers();//初始化订阅任务
            initClearJobAsync();//初始化清理任务
            initCookieJobAsync();//初始化cookie检查任务
        }

        private static void initCustomJob()
        {
            try
            {
                ReminderConfig reminderConfig = BotConfig.ReminderConfig;
                if (reminderConfig == null) return;
                if (reminderConfig.Enable == false) return;
                foreach (var item in reminderConfig.Timers) createCustomJob(item);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"定时功能异常");
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
                LogHelper.Info($"清理定时器启动完毕...");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"清理定时器启动失败");
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
            if (subscribeConfig == null) return;
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


        private static async void createCustomJob(ReminderTimer reminderTimer)
        {
            try
            {
                ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create().WithCronSchedule(reminderTimer.Cron).Build();
                IJobDetail jobDetail = JobBuilder.Create<CustomJob>().WithIdentity(reminderTimer.GetHashCode().ToString(), "CustomJob").Build();//创建作业
                IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                jobDetail.JobDataMap.Put("ReminderTimer", reminderTimer);
                await scheduler.ScheduleJob(jobDetail, trigger);
                await scheduler.Start();
                LogHelper.Info($"定时器[{reminderTimer.Name}]启动完毕...");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"定时器[{reminderTimer.Name}]启动失败");
            }
        }



    }
}
