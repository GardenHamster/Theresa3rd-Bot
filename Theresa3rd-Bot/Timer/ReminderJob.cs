using Quartz;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Theresa3rd_Bot.Handler;
using Theresa3rd_Bot.Model.Config;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Timer
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
