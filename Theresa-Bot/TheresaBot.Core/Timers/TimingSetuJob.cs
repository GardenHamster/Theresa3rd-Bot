using Quartz;
using TheresaBot.Core.Cache;
using TheresaBot.Core.Handler;
using TheresaBot.Core.Helper;
using TheresaBot.Core.Model.Config;
using TheresaBot.Core.Reporter;
using TheresaBot.Core.Session;
using TheresaBot.Core.Type;

namespace TheresaBot.Core.Timers
{
    [DisallowConcurrentExecution]
    internal class TimingSetuJob : IJob
    {
        private BaseReporter Reporter;
        private int CoolingMinutes = 10;

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                if (CoolingCache.IsSetuTimingCooling(CoolingMinutes))
                {
                    throw new Exception($"距离上一个定时涩图任务触发时间小于{CoolingMinutes}分钟，本次定时任务已取消");
                }
                CoolingCache.SetSetuTimingCooling();
                var dataMap = context.MergedJobDataMap;
                var session = (BaseSession)dataMap["BaseSession"];
                var timingSetuTimer = (TimingSetuTimer)dataMap["TimingSetuTimer"];
                Reporter = (BaseReporter)dataMap["BaseReporter"];
                if (timingSetuTimer is null) return;
                if (timingSetuTimer.PushGroups.Count == 0) return;
                if (timingSetuTimer.Quantity <= 0) throw new Exception("Quantity必须大于0");
                LogHelper.Info($"开始执行【{timingSetuTimer.Name}】定时涩图任务...");
                var groupIds = timingSetuTimer.PushGroups.Take(5).ToList();
                foreach (long groupId in groupIds)
                {
                    Task timingTask = HandleTiming(session, Reporter, timingSetuTimer, groupId);
                    await Task.Delay(3000);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "TimingSetuJob异常");
                await Reporter.SendError(ex, "TimingSetuJob异常");
            }
        }

        public async Task HandleTiming(BaseSession session, BaseReporter reporter, TimingSetuTimer timingSetuTimer, long groupId)
        {
            try
            {
                TimingSetuSourceType sourceType = timingSetuTimer.Source;
                if (sourceType == TimingSetuSourceType.Lolicon)
                {
                    await new LoliconHandler(session, reporter).SendTimingSetuAsync(timingSetuTimer, groupId);
                }
                else if (sourceType == TimingSetuSourceType.Lolisuki)
                {
                    await new LolisukiHandler(session, reporter).SendTimingSetuAsync(timingSetuTimer, groupId);
                }
                else if (sourceType == TimingSetuSourceType.Local)
                {
                    await new LocalSetuHandler(session, reporter).SendTimingSetuAsync(timingSetuTimer, groupId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"{timingSetuTimer.Name}定时任务异常");
                await reporter.SendError(ex, $"{timingSetuTimer.Name}定时任务异常");
            }
        }

    }
}
