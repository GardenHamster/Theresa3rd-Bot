using Quartz;
using TheresaBot.Main.Common;
using TheresaBot.Main.Handler;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Timers
{
    [DisallowConcurrentExecution]
    public class CookieJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                JobDataMap dataMap = context.MergedJobDataMap;
                BaseSession session = (BaseSession)dataMap["BaseSession"];
                BaseReporter reporter = (BaseReporter)dataMap["BaseReporter"];
                await new CookieHandler(session, reporter).CheckAndWarn(BotConfig.WebsiteConfig.Pixiv, 2, "Pixiv Cookie");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "CookieWarningJob异常");
            }
        }

    }
}
