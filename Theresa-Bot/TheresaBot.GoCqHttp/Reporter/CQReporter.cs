using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;
using TheresaBot.Core.Reporter;
using TheresaBot.GoCqHttp.Helper;

namespace TheresaBot.GoCqHttp.Reporter
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
