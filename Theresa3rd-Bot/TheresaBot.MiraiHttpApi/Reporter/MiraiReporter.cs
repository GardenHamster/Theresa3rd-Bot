using Mirai.CSharp.HttpApi.Models.ChatMessages;
using TheresaBot.Main.Reporter;
using TheresaBot.MiraiHttpApi.Helper;

namespace TheresaBot.MiraiHttpApi.Reporter
{
    public class MiraiReporter : BaseReporter
    {
        protected override async Task<long> SendReport(long groupId, string message)
        {
            return await MiraiHelper.Session.SendGroupMessageAsync(groupId, new PlainMessage(message));
        }
    }
}
