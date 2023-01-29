using Mirai.CSharp.HttpApi.Models.ChatMessages;
using System.Collections.Generic;
using System.Threading.Tasks;
using Theresa3rd_Bot.BotPlatform.Base.Command;
using Theresa3rd_Bot.BotPlatform.Mirai.Util;
using Theresa3rd_Bot.Model.Config;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Handler
{
    public class ReminderHandler : BaseHandler
    {
        public async Task SendRemindAsync(ReminderTimer reminderTimer)
        {
            List<IChatMessage> chainList = new List<IChatMessage>();
            if (reminderTimer.AtAll == true) chainList.Add(new AtAllMessage());
            if (reminderTimer.AtMembers != null && reminderTimer.AtMembers.Count > 0)
            {
                foreach (var memberId in reminderTimer.AtMembers) chainList.Add(new AtMessage(memberId));
            }
            chainList.AddRange(BusinessHelper.SplitToChainAsync(MiraiHelper.Session, reminderTimer.Template).Result);
            foreach (var groupId in reminderTimer.Groups)
            {
                await MiraiHelper.Session.SendGroupMessageAsync(groupId, chainList.ToArray());
                await Task.Delay(1000);
            }
        }


    }
}
