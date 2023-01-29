using Quartz;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Handler;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Timer
{
    [DisallowConcurrentExecution]
    public class TimingSetuJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                int coolingMinutes = 10;
                if (CoolingCache.IsSetuTimingCooling(coolingMinutes))
                {
                    throw new Exception($"距离上一个定时涩图任务触发时间小于{coolingMinutes}分钟，本次定时任务已取消");
                }
                CoolingCache.SetSetuTimingCooling();
                JobDataMap dataMap = context.MergedJobDataMap;
                BaseSession session = (BaseSession)dataMap["TimingSetuSession"];
                TimingSetuTimer timingSetuTimer = (TimingSetuTimer)dataMap["TimingSetuTimer"];
                if (timingSetuTimer is null) return;
                if (timingSetuTimer.Groups is null || timingSetuTimer.Groups.Count == 0) return;
                if (timingSetuTimer.Quantity <= 0) throw new Exception("Quantity必须大于0");
                List<long> groupIds = timingSetuTimer.Groups.Distinct().Take(5).ToList();
                foreach (long groupId in groupIds)
                {
                    Task timingTask = HandleTiming(session, timingSetuTimer, groupId);
                    await Task.Delay(3000);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "TimingSetuJob异常");
            }
        }

        public async Task HandleTiming(BaseSession session, TimingSetuTimer timingSetuTimer, long groupId)
        {
            try
            {
                TimingSetuSourceType sourceType = timingSetuTimer.Source;
                if (sourceType == TimingSetuSourceType.Lolicon)
                {
                    await new LoliconHandler(session).sendTimingSetuAsync(timingSetuTimer, groupId);
                }
                else if (sourceType == TimingSetuSourceType.Lolisuki)
                {
                    await new LolisukiHandler(session).sendTimingSetuAsync(timingSetuTimer, groupId);
                }
                else
                {
                    await new LocalSetuHandler(session).sendTimingSetuAsync(timingSetuTimer, groupId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"{timingSetuTimer.Name}定时任务异常");
            }
        }

    }
}
