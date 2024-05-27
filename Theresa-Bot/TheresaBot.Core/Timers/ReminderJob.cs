using Quartz;
using TheresaBot.Core.Handler;
using TheresaBot.Core.Helper;
using TheresaBot.Core.Model.Config;
using TheresaBot.Core.Reporter;
using TheresaBot.Core.Session;

namespace TheresaBot.Core.Timers
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
