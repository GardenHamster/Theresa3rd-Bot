using Mirai.CSharp.HttpApi.Handlers;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using System;
using System.Threading.Tasks;
using Theresa3rd_Bot.Model.Config;

namespace Theresa3rd_Bot.Event
{
    public class DisconnectedEvent : IMiraiHttpMessageHandler<IDisconnectedEventArgs>
    {
        public async Task HandleMessageAsync(IMiraiHttpSession session, IDisconnectedEventArgs e)
        {
            while (true)
            {
                try
                {
                    await session.ConnectAsync(BotConfig.MiraiConfig.BotQQ);
                    e.BlockRemainingHandlers = true;
                }
                catch (Exception)
                {
                    await Task.Delay(1000);
                }
            }
        }
    }
}
