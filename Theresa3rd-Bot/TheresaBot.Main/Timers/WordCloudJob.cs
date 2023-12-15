using Quartz;
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
