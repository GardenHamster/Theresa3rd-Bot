using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
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
            PixivUserTimer.init();//初始化pixiv画师订阅任务
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
