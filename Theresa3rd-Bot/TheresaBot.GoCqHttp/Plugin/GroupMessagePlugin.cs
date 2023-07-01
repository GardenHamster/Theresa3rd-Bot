using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;
using EleCho.GoCqHttpSdk.Post;

namespace TheresaBot.GoCqHttp.Plugin
{
    public class GroupMessagePlugin : CqPostPlugin
    {
        public override async void OnGroupMessageReceived(CqGroupMessagePostContext context)
        {
            // 判断是否能够发送 Action
            if (context.Session is not ICqActionSession actionSession) return;

            //string text = context.Message.GetText();
            //if (text.StartsWith("TTS:", StringComparison.OrdinalIgnoreCase))
            //{
            //    await actionSession.SendGroupMessageAsync(context.GroupId, new CqTtsMsg(text[4..]));
            //}

        }




    }
}
