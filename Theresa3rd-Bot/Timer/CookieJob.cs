using Quartz;
using System;
using System.Threading.Tasks;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Handler;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Timer
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
