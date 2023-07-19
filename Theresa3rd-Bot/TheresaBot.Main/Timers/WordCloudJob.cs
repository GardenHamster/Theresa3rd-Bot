using Quartz;
using TheresaBot.Main.Common;
using TheresaBot.Main.Handler;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Mode;
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
                if (wordCloudTimer.Groups is null) return;
                if (wordCloudTimer.Groups.Count == 0) return;
                if (wordCloudTimer == BotConfig.WordCloudConfig.Subscribes.Daily)
                {
                    LogHelper.Info($"开始执行每日词云推送任务...");
                    await new WordCloudHandler(session, reporter).pushDailyWordCloudAsync();
                    return;
                }
                if (wordCloudTimer == BotConfig.WordCloudConfig.Subscribes.Weekly)
                {
                    LogHelper.Info($"开始执行每周词云推送任务...");
                    await new WordCloudHandler(session, reporter).pushWeeklyWordCloudAsync();
                    return;
                }
                if (wordCloudTimer == BotConfig.WordCloudConfig.Subscribes.Monthly)
                {
                    LogHelper.Info($"开始执行每月词云推送任务...");
                    await new WordCloudHandler(session, reporter).pushMonthlyWordCloudAsync();
                    return;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "WordCloudJob异常");
                await reporter.SendError(ex, "WordCloudJob异常");
            }
        } 

    }
}
