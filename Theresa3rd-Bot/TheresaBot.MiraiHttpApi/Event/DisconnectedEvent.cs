using Mirai.CSharp.HttpApi.Handlers;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;

namespace TheresaBot.MiraiHttpApi.Event
{
    public class DisconnectedEvent : IMiraiHttpMessageHandler<IDisconnectedEventArgs>
    {
        public async Task HandleMessageAsync(IMiraiHttpSession session, IDisconnectedEventArgs e)
        {
            int retryTimes = 0;
            LogHelper.FATAL("与mcl失去连接，正在尝试重连...");
            while (true)
            {
                try
                {
                    await session.ConnectAsync(BotConfig.BotQQ);
                    e.BlockRemainingHandlers = true;
                    LogHelper.Info("已重新连接到mcl...");
                    break;
                }
                catch (ObjectDisposedException)
                {
                    LogHelper.Info("已成功连接到mcl...");
                    break;
                }
                catch (Exception ex)
                {
                    LogHelper.FATAL(ex.Message);
                    int delaySeconds = getDelaySeconds(retryTimes++);
                    LogHelper.FATAL($"重新连接到mcl失败，将在 {delaySeconds} 秒后重试...");
                    await Task.Delay(delaySeconds * 1000);
                }
            }
        }

        private int getDelaySeconds(int retryTimes)
        {
            int delaySeconds = 5 + (int)Math.Floor(Convert.ToDouble(retryTimes) / 5) * 5;
            return delaySeconds > 60 ? 60 : delaySeconds;
        }

    }
}
