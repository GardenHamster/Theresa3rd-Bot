using Quartz;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Handler;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Timers
{
    [DisallowConcurrentExecution]
    internal class CookieJob : IJob
    {
        private BaseReporter reporter;

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                JobDataMap dataMap = context.MergedJobDataMap;
                reporter = (BaseReporter)dataMap["BaseReporter"];
                BaseSession session = (BaseSession)dataMap["BaseSession"];
                await new CookieHandler(session, reporter).CheckAndWarn(WebsiteDatas.Pixiv, 2, "Pixiv Cookie");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "CookieWarningJob异常");
                await reporter.SendError(ex, "CookieWarningJob异常");
            }
        }

    }
}
