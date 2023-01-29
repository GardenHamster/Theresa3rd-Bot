using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Handler
{
    public class ReminderHandler : BaseHandler
    {
        public ReminderHandler(BaseSession session) : base(session)
        {
        }

        public async Task SendRemindAsync(ReminderTimer reminderTimer)
        {
            List<BaseContent> chainList = BusinessHelper.SplitToChainAsync(reminderTimer.Template);
            foreach (var groupId in reminderTimer.Groups)
            {
                await Session.SendGroupMessageAsync(groupId, chainList, reminderTimer.AtMembers, reminderTimer.AtAll);
                await Task.Delay(1000);
            }
        }


    }
}
