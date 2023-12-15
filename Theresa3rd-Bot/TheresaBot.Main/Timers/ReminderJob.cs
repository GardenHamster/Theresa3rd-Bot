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
        private BaseReporter Reporter;

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var dataMap = context.MergedJobDataMap;
                var session = (BaseSession)dataMap["BaseSession"];
                var reminderTimer = (ReminderTimer)dataMap["ReminderTimer"];
                Reporter = (BaseReporter)dataMap["BaseReporter"];
                if (reminderTimer is null) return;
                if (reminderTimer.PushGroups.Count == 0) return;
                if (reminderTimer.Templates.Count == 0) return;
                var handler = new ReminderHandler(session, Reporter);
                await handler.SendRemindAsync(reminderTimer);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "ReminderJob异常");
                await Reporter.SendError(ex, "ReminderJob异常");
            }
        }

    }
}
