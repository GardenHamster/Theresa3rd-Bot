using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;
using TheresaBot.OneBot11.Helper;
using TheresaBot.Core.Reporter;

namespace TheresaBot.OneBot11.Reporter
{
    public class CQReporter : BaseReporter
    {
        protected override async Task<long> SendReport(long groupId, string message)
        {
            var result = await CQHelper.Session.SendGroupMessageAsync(groupId, new CqMessage(message));
            return result is null ? 0 : result.MessageId;
        }
    }
}
