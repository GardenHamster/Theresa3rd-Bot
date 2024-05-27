using Quartz;
using TheresaBot.Core.Handler;
using TheresaBot.Core.Helper;
using TheresaBot.Core.Model.Config;
using TheresaBot.Core.Reporter;
using TheresaBot.Core.Session;

namespace TheresaBot.Core.Timers
{
    [DisallowConcurrentExecution]
    internal class WordCloudJob : IJob
    {
        private BaseReporter Reporter;

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var dataMap = context.MergedJobDataMap;
                var session = (BaseSession)dataMap["BaseSession"];
                var wordCloudTimer = (WordCloudTimer)dataMap["WordCloudTimer"];
                Reporter = (BaseReporter)dataMap["BaseReporter"];
                if (wordCloudTimer is null) return;
                if (wordCloudTimer.Enable == false) return;
                if (wordCloudTimer.PushGroups.Count == 0) return;
                await new WordCloudHandler(session, Reporter).PushWordCloudAsync(wordCloudTimer);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "WordCloudJob异常");
                await Reporter.SendError(ex, "WordCloudJob异常");
            }
        }

    }
}
