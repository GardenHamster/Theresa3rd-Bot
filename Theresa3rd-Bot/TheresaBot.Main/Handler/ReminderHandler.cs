using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Helper;

namespace TheresaBot.Main.Handler
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
