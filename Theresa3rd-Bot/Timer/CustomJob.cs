using Mirai.CSharp.Models.ChatMessages;
using Quartz;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Theresa3rd_Bot.Model.Config;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Timer
{
    public class CustomJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                JobDataMap dataMap = context.MergedJobDataMap;
                ReminderTimer reminderTimer = (ReminderTimer)dataMap["ReminderTimer"];
                if (reminderTimer == null) return;
                if (reminderTimer.Groups == null || reminderTimer.Groups.Count == 0) return;
                List<IChatMessage> chainList = new List<IChatMessage>();
                if (reminderTimer.AtAll == true) chainList.Add(new Mirai.CSharp.HttpApi.Models.ChatMessages.AtAllMessage());
                if (reminderTimer.AtMembers != null && reminderTimer.AtMembers.Count > 0)
                {
                    foreach (var memberId in reminderTimer.AtMembers) chainList.Add(new Mirai.CSharp.HttpApi.Models.ChatMessages.AtMessage(memberId, ""));
                }
                chainList.AddRange(BusinessHelper.SplitToChainAsync(MiraiHelper.Session, reminderTimer.Template).Result);
                foreach (var groupId in reminderTimer.Groups)
                {
                    if (!BusinessHelper.IsHandleMessage(groupId)) continue;
                    await MiraiHelper.Session.SendGroupMessageAsync(groupId, chainList.ToArray());
                    await Task.Delay(1000);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "CustomJob异常");
            }
        }
    }
}
