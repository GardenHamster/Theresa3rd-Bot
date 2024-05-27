using Quartz;
using TheresaBot.Core.Datas;
using TheresaBot.Core.Handler;
using TheresaBot.Core.Helper;
using TheresaBot.Core.Reporter;
using TheresaBot.Core.Session;

namespace TheresaBot.Core.Timers
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
