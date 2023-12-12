using Quartz;
using Quartz.Impl;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Timers
{
    public static class SchedulerManager
    {
        /// <summary>
        /// 清理任务线程锁
        /// </summary>
        internal static readonly object ClearLock = new object();
        private static readonly List<IScheduler> TempClearSchedulers = new();
        private static readonly List<IScheduler> DownClearSchedulers = new();
        private static readonly List<IScheduler> ReminderSchedulers = new();
        private static readonly List<IScheduler> TimingSetuSchedulers = new();
        private static readonly List<IScheduler> TimingRankingSchedulers = new();
        private static readonly List<IScheduler> WordCloudSchedulers = new();
        private static readonly List<IScheduler> CookieJobSchedulers = new();

        /// <summary>
        /// 启动定时任务
        /// </summary>
        /// <param name="session"></param>
        /// <param name="reporter"></param>
        public static async void InitSchedulers(BaseSession session, BaseReporter reporter)
        {
            await InitTempClearJobAsync(session, reporter);
            await InitDownClearJobAsync(session, reporter);
            await InitCookieJobAsync(session, reporter);
            await InitReminderJobAsync(session, reporter);
            await InitTimingSetuJobAsync(session, reporter);
            await InitPixivRankingJobAsync(session, reporter);
            await InitWordCloudJobAsync(session, reporter);
        }

        /// <summary>
        /// 启动清理任务
        /// </summary>
        public static async Task InitTempClearJobAsync(BaseSession session, BaseReporter reporter)
        {
            try
            {
                await TempClearSchedulers.DestroyAndClearAsync();
                string tempClearCron = "0 0 4 * * ?";
                ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create().WithCronSchedule(tempClearCron).Build();
                IJobDetail jobDetail = JobBuilder.Create<TempClearJob>().WithIdentity("TempClearJob", "TempClearJob").Build();
                IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                jobDetail.JobDataMap.Put("BaseSession", session);
                jobDetail.JobDataMap.Put("BaseReporter", reporter);
                await scheduler.ScheduleJob(jobDetail, trigger);
                await scheduler.Start();
                TempClearSchedulers.Add(scheduler);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"定时清理任务启动失败");
                throw;
            }
        }

        /// <summary>
        /// 启动清理任务
        /// </summary>
        public static async Task InitDownClearJobAsync(BaseSession session, BaseReporter reporter)
        {
            try
            {
                await DownClearSchedulers.DestroyAndClearAsync();
                string downloadClearCron = BotConfig.GeneralConfig.ClearCron;
                if (string.IsNullOrWhiteSpace(downloadClearCron)) return;
                ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create().WithCronSchedule(downloadClearCron).Build();
                IJobDetail jobDetail = JobBuilder.Create<DownloadClearJob>().WithIdentity("DownloadClearJob", "DownloadClearJob").Build();
                IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                jobDetail.JobDataMap.Put("BaseSession", session);
                jobDetail.JobDataMap.Put("BaseReporter", reporter);
                await scheduler.ScheduleJob(jobDetail, trigger);
                await scheduler.Start();
                LogHelper.Info($"定时清理任务启动完毕...");
                DownClearSchedulers.Add(scheduler);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"定时清理任务启动失败");
                throw;
            }
        }

        /// <summary>
        /// 启动Cookie检查任务
        /// </summary>
        /// <param name="session"></param>
        /// <param name="reporter"></param>
        public static async Task InitCookieJobAsync(BaseSession session, BaseReporter reporter)
        {
            try
            {
                await CookieJobSchedulers.DestroyAndClearAsync();
                string cookieCron = "0 0 9 * * ?";
                ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create().WithCronSchedule(cookieCron).Build();
                IJobDetail jobDetail = JobBuilder.Create<CookieJob>().WithIdentity("CookieJob", "CookieJob").Build();
                IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                jobDetail.JobDataMap.Put("BaseSession", session);
                jobDetail.JobDataMap.Put("BaseReporter", reporter);
                await scheduler.ScheduleJob(jobDetail, trigger);
                await scheduler.Start();
                LogHelper.Info($"Cookie检查定时器启动完毕...");
                CookieJobSchedulers.Add(scheduler);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"Cookie检查定时器启动失败");
                throw;
            }
        }

        /// <summary>
        /// 启动定时提醒任务
        /// </summary>
        public static async Task InitReminderJobAsync(BaseSession session, BaseReporter reporter)
        {
            try
            {
                await ReminderSchedulers.DestroyAndClearAsync();
                var reminderConfig = BotConfig.ReminderConfig;
                if (reminderConfig is null) return;
                if (reminderConfig.Enable == false) return;
                foreach (var item in reminderConfig.Timers)
                {
                    IScheduler scheduler = await CreateReminderJobAsync(item, session, reporter);
                    ReminderSchedulers.Add(scheduler);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"定时提醒任务启动失败");
                throw;
            }
        }

        private static async Task<IScheduler> CreateReminderJobAsync(ReminderTimer timer, BaseSession session, BaseReporter reporter)
        {
            try
            {
                if (timer is null || timer.Enable == false) return null;
                if (string.IsNullOrWhiteSpace(timer.Cron)) return null;
                ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create().WithCronSchedule(timer.Cron).Build();
                IJobDetail jobDetail = JobBuilder.Create<ReminderJob>().WithIdentity(timer.GetHashCode().ToString(), "ReminderJob").Build();
                IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                jobDetail.JobDataMap.Put("ReminderTimer", timer);
                jobDetail.JobDataMap.Put("BaseSession", session);
                jobDetail.JobDataMap.Put("BaseReporter", reporter);
                await scheduler.ScheduleJob(jobDetail, trigger);
                await scheduler.Start();
                LogHelper.Info($"定时提醒任务[{timer.Name}]启动完毕...");
                return scheduler;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"定时提醒任务[{timer.Name}]启动失败");
                throw;
            }
        }

        /// <summary>
        /// 启动定时色图任务
        /// </summary>
        /// <param name="session"></param>
        public static async Task InitTimingSetuJobAsync(BaseSession session, BaseReporter reporter)
        {
            try
            {
                await TimingSetuSchedulers.DestroyAndClearAsync();
                var timingSetuConfig = BotConfig.TimingSetuConfig;
                if (timingSetuConfig is null) return;
                if (timingSetuConfig.Enable == false) return;
                if (timingSetuConfig.Timers is null) return;
                if (timingSetuConfig.Timers.Count == 0) return;
                List<TimingSetuTimer> timers = timingSetuConfig.Timers.Take(10).ToList();
                foreach (var item in timers)
                {
                    IScheduler scheduler = await CreateTimingSetuJobAsync(item, session, reporter);
                    TimingSetuSchedulers.Add(scheduler);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"涩图定时推送任务启动失败");
                throw;
            }
        }

        private static async Task<IScheduler> CreateTimingSetuJobAsync(TimingSetuTimer timer, BaseSession session, BaseReporter reporter)
        {
            try
            {
                if (timer is null || timer.Enable == false) return null;
                if (string.IsNullOrWhiteSpace(timer.Cron)) return null;
                ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create().WithCronSchedule(timer.Cron).Build();
                IJobDetail jobDetail = JobBuilder.Create<TimingSetuJob>().WithIdentity(timer.GetHashCode().ToString(), "TimingSetuJob").Build();
                IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                jobDetail.JobDataMap.Put("TimingSetuTimer", timer);
                jobDetail.JobDataMap.Put("BaseReporter", reporter);
                jobDetail.JobDataMap.Put("BaseSession", session);
                await scheduler.ScheduleJob(jobDetail, trigger);
                await scheduler.Start();
                LogHelper.Info($"涩图定时推送任务[{timer.Name}]启动完毕...");
                return scheduler;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"涩图定时推送任务[{timer.Name}]启动失败");
                throw;
            }
        }

        public static async Task InitPixivRankingJobAsync(BaseSession session, BaseReporter reporter)
        {
            try
            {
                await TimingRankingSchedulers.DestroyAndClearAsync();
                var rankingConfig = BotConfig.PixivRankingConfig;
                if (rankingConfig is null) return;
                if (rankingConfig.Enable == false) return;
                if (rankingConfig.Subscribes is null) return;
                if (rankingConfig.Subscribes.Count == 0) return;
                List<PixivRankingTimer> timers = rankingConfig.Subscribes;
                foreach (var item in timers)
                {
                    IScheduler scheduler = await CreateTimingRankingJobAsync(item, session, reporter);
                    TimingRankingSchedulers.Add(scheduler);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"Pixiv榜单定时推送任务启动失败");
                throw;
            }
        }

        private static async Task<IScheduler> CreateTimingRankingJobAsync(PixivRankingTimer timer, BaseSession session, BaseReporter reporter)
        {
            try
            {
                if (timer is null || timer.Enable == false) return null;
                if (timer.Contents is null || timer.Contents.Count == 0) return null;
                if (string.IsNullOrWhiteSpace(timer.Cron)) return null;
                ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create().WithCronSchedule(timer.Cron).Build();
                IJobDetail jobDetail = JobBuilder.Create<TimingRankingJob>().WithIdentity(timer.GetHashCode().ToString(), "TimingRankingJob").Build();
                IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                jobDetail.JobDataMap.Put("PixivRankingTimer", timer);
                jobDetail.JobDataMap.Put("BaseReporter", reporter);
                jobDetail.JobDataMap.Put("BaseSession", session);
                await scheduler.ScheduleJob(jobDetail, trigger);
                await scheduler.Start();
                LogHelper.Info($"Pixiv榜单定时推送任务[{string.Join(',', timer.Contents)}]启动完毕...");
                return scheduler;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"Pixiv榜单定时推送任务[{string.Join(',', timer.Contents)}]启动失败");
                throw;
            }
        }

        /// <summary>
        /// 启动词云推送任务
        /// </summary>
        public static async Task InitWordCloudJobAsync(BaseSession session, BaseReporter reporter)
        {
            try
            {
                await WordCloudSchedulers.DestroyAndClearAsync();
                var wordCloudConfig = BotConfig.WordCloudConfig;
                var subscribes = wordCloudConfig?.Subscribes;
                if (subscribes is null || subscribes.Count == 0) return;
                foreach (var subscribe in subscribes)
                {
                    if (subscribe.Enable == false) continue;
                    IScheduler scheduler = await CreateWordCloudJobAsync(subscribe, session, reporter);
                    WordCloudSchedulers.Add(scheduler);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"词云定时推送任务启动失败");
                throw;
            }
        }

        private static async Task<IScheduler> CreateWordCloudJobAsync(WordCloudTimer timer, BaseSession session, BaseReporter reporter)
        {
            try
            {
                if (timer is null || timer.Enable == false) return null;
                if (string.IsNullOrWhiteSpace(timer.Cron)) return null;
                ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create().WithCronSchedule(timer.Cron).Build();
                IJobDetail jobDetail = JobBuilder.Create<WordCloudJob>().WithIdentity(timer.GetHashCode().ToString(), "WordCloudJob").Build();
                IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                jobDetail.JobDataMap.Put("WordCloudTimer", timer);
                jobDetail.JobDataMap.Put("BaseReporter", reporter);
                jobDetail.JobDataMap.Put("BaseSession", session);
                await scheduler.ScheduleJob(jobDetail, trigger);
                await scheduler.Start();
                LogHelper.Info($"词云定时推送任务[{timer.Name}]启动完毕...");
                return scheduler;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"词云定时推送任务[{timer.Name}]启动失败");
                throw;
            }
        }

        private static async Task DestroyAndClearAsync(this List<IScheduler> schedulers)
        {
            await schedulers.DestroyAsync();
            schedulers.Clear();
        }

        private static async Task DestroyAsync(this List<IScheduler> schedulers)
        {
            if (schedulers is null) return;
            foreach (IScheduler scheduler in schedulers)
            {
                await scheduler.DestroyAsync();
            }
        }

        private static async Task DestroyAsync(this IScheduler scheduler)
        {
            try
            {
                if (scheduler is null) return;
                if (scheduler.IsShutdown) return;
                await scheduler.Shutdown(false);
                await scheduler.DestroyAsync();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"任务调度{scheduler.SchedulerName}强制停止失败");
                throw;
            }
        }

    }
}
