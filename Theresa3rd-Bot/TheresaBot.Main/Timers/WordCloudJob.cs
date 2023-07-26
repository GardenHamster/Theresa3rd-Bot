using Quartz;
using TheresaBot.Main.Common;
using TheresaBot.Main.Handler;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Timers
{
    [DisallowConcurrentExecution]
    internal class WordCloudJob : IJob
    {
        private BaseReporter reporter;

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                JobDataMap dataMap = context.MergedJobDataMap;
                reporter = (BaseReporter)dataMap["BaseReporter"];
                BaseSession session = (BaseSession)dataMap["BaseSession"];
                WordCloudTimer wordCloudTimer = (WordCloudTimer)dataMap["WordCloudTimer"];
                if (wordCloudTimer is null) return;
                if (wordCloudTimer.Enable == false) return;
                if (wordCloudTimer.Groups is null) return;
                if (wordCloudTimer.Groups.Count == 0) return;
                await new WordCloudHandler(session, reporter).pushWordCloudAsync(wordCloudTimer);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "WordCloudJob异常");
                await reporter.SendError(ex, "WordCloudJob异常");
            }
        }

    }
}
