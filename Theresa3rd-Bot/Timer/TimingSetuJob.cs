using Quartz;
using System;
using System.Threading.Tasks;
using Theresa3rd_Bot.Handler;
using Theresa3rd_Bot.Model.Config;
using Theresa3rd_Bot.Type;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Timer
{
    public class TimingSetuJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                JobDataMap dataMap = context.MergedJobDataMap;
                TimingSetuTimer timingSetuTimer = (TimingSetuTimer)dataMap["TimingSetuTimer"];
                if (timingSetuTimer == null) return;
                if (timingSetuTimer.Groups == null || timingSetuTimer.Groups.Count == 0) return;
                if (timingSetuTimer.Quantity <= 0) throw new Exception("Quantity必须大于0");
                foreach (long groupId in timingSetuTimer.Groups)
                {
                    await HandleTiming(timingSetuTimer, groupId);
                    await Task.Delay(1000);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "TimingSetuJob异常");
            }
        }

        public async Task HandleTiming(TimingSetuTimer timingSetuTimer, long groupId)
        {
            try
            {
                TimingSetuSourceType sourceType = timingSetuTimer.Source;
                if (sourceType == TimingSetuSourceType.Lolicon)
                {
                    await new LoliconHandler().sendTimingSetu(MiraiHelper.Session, timingSetuTimer, groupId);
                }
                else if (sourceType == TimingSetuSourceType.Lolisuki)
                {
                    await new LolisukiHandler().sendTimingSetu(MiraiHelper.Session, timingSetuTimer, groupId);
                }
                else
                {
                    await new LocalSetuHandler().sendTimingSetu(MiraiHelper.Session, timingSetuTimer, groupId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"{timingSetuTimer.Name}定时任务异常");
            }
        }

    }
}
