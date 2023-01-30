using Quartz;
using TheresaBot.Main.Handler;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Timers
{
    [DisallowConcurrentExecution]
    public class ReminderJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                JobDataMap dataMap = context.MergedJobDataMap;
                BaseSession session = (BaseSession)dataMap["BaseSession"];
                BaseReporter reporter = (BaseReporter)dataMap["BaseReporter"];
                ReminderTimer reminderTimer = (ReminderTimer)dataMap["ReminderTimer"];
                if (reminderTimer is null) return;
                if (reminderTimer.Groups is null || reminderTimer.Groups.Count == 0) return;
                await new ReminderHandler(session, reporter).SendRemindAsync(reminderTimer);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "ReminderJob异常");
            }
        }
    }
}
