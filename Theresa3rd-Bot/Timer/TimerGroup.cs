using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Model.Config;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Timer
{
    public static class TimerGroup
    {
        private static List<System.Timers.Timer> CustomTimers = new List<System.Timers.Timer>();

        public static void init()
        {
            try
            {
                initCustomJob();//初始化配置列表中的定时器
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }

        private static void initCustomJob()
        {
            ReminderConfig reminderConfig = BotConfig.ReminderConfig;
            if (reminderConfig == null) return;
            if (reminderConfig.Enable == false) return;
            foreach (var item in reminderConfig.Timers) createCustomJob(item);
        }


        private static async void createCustomJob(ReminderTimer reminderTimer)
        {
            ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create().WithCronSchedule(reminderTimer.Cron).Build();
            IJobDetail jobDetail = JobBuilder.Create<CustomJob>().WithIdentity(reminderTimer.GetHashCode().ToString(), "CustomJob").Build();//创建作业
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            jobDetail.JobDataMap.Put("ReminderTimer", reminderTimer);
            await scheduler.ScheduleJob(jobDetail, trigger);
            await scheduler.Start();
        }



    }
}
