﻿using TheresaBot.Core.Helper;
using TheresaBot.Core.Model.Config;
using TheresaBot.Core.Model.Content;
using TheresaBot.Core.Reporter;
using TheresaBot.Core.Session;

namespace TheresaBot.Core.Handler
{
    internal class ReminderHandler : BaseHandler
    {
        public ReminderHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
        }

        public async Task SendRemindAsync(ReminderTimer reminderTimer)
        {
            var randomTemplate = reminderTimer.Templates.RandomItem()?.Template ?? string.Empty;
            List<BaseContent> chainList = BusinessHelper.SplitToChainAsync(randomTemplate);
            foreach (var groupId in reminderTimer.PushGroups)
            {
                await Session.SendGroupMessageAsync(groupId, chainList, reminderTimer.AtMembers, reminderTimer.AtAll);
                await Task.Delay(1000);
            }
        }


    }
}