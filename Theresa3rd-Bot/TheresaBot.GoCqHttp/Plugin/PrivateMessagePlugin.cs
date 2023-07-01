using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Post;

namespace TheresaBot.GoCqHttp.Plugin
{
    public class PrivateMessagePlugin : CqPostPlugin
    {

        public override async void OnPrivateMessageReceived(CqPrivateMessagePostContext context)
        {
            // 判断是否能够发送 Action
            if (context.Session is not ICqActionSession actionSession) return;
        }

    }
}
