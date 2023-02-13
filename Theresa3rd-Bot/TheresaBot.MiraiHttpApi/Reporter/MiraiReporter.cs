using Mirai.CSharp.HttpApi.Models.ChatMessages;
using System.Threading.Tasks;
using TheresaBot.Main.Reporter;
using TheresaBot.MiraiHttpApi.Helper;

namespace TheresaBot.MiraiHttpApi.Reporter
{
    public class MiraiReporter : BaseReporter
    {
        protected override Task<int> SendReport(long groupId, string message)
        {
            return MiraiHelper.Session.SendGroupMessageAsync(groupId, new PlainMessage(message));
        }
    }
}
