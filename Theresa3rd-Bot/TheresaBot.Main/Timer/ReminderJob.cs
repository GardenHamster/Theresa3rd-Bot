using Quartz;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheresaBot.Main.Handler;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Helper;

namespace TheresaBot.Main.Timer
{
    [DisallowConcurrentExecution]
    public class ReminderJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                JobDataMap dataMap = context.MergedJobDataMap;
                ReminderTimer reminderTimer = (ReminderTimer)dataMap["ReminderTimer"];
                if (reminderTimer is null) return;
                if (reminderTimer.Groups is null || reminderTimer.Groups.Count == 0) return;
                await new ReminderHandler().SendRemindAsync(reminderTimer);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "ReminderJob异常");
            }
        }
    }
}
