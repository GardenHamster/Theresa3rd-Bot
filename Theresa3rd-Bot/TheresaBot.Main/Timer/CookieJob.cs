using Quartz;
using System;
using System.Threading.Tasks;
using TheresaBot.Main.Common;
using TheresaBot.Main.Handler;
using TheresaBot.Main.Helper;

namespace TheresaBot.Main.Timer
{
    [DisallowConcurrentExecution]
    public class CookieJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await new CookieHandler().CheckAndWarn(BotConfig.WebsiteConfig.Pixiv, 2, "Pixiv Cookie");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "CookieWarningJob异常");
            }
        }

    }
}
