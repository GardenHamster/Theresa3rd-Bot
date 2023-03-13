using Quartz;
using TheresaBot.Main.Handler;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Timers
{
    [DisallowConcurrentExecution]
    internal class ReminderJob : IJob
    {
        private BaseReporter reporter;

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                JobDataMap dataMap = context.MergedJobDataMap;
                reporter = (BaseReporter)dataMap["BaseReporter"];
                BaseSession session = (BaseSession)dataMap["BaseSession"];
                ReminderTimer reminderTimer = (ReminderTimer)dataMap["ReminderTimer"];
                if (reminderTimer is null) return;
                if (reminderTimer.Groups is null || reminderTimer.Groups.Count == 0) return;
                await new ReminderHandler(session, reporter).SendRemindAsync(reminderTimer);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "ReminderJob异常");
                reporter.SendError(ex, "ReminderJob异常");
            }
        }
    }
}
