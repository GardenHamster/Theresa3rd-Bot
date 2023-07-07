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
    internal class TimingRankingJob : IJob
    {
        private BaseReporter reporter;

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                JobDataMap dataMap = context.MergedJobDataMap;
                reporter = (BaseReporter)dataMap["BaseReporter"];
                BaseSession session = (BaseSession)dataMap["BaseSession"];
                PixivRankingTimer rankingTimer = (PixivRankingTimer)dataMap["PixivRankingTimer"];
                if (rankingTimer is null) return;
                if (rankingTimer.Groups is null || rankingTimer.Groups.Count == 0) return;
                foreach (var content in rankingTimer.Contents)
                {
                    LogHelper.Info($"开始执行【{content}】日榜推送任务...");
                    await HandleTiming(session, reporter, rankingTimer, content);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "TimingRankingJob异常");
                await reporter.SendError(ex, "TimingRankingJob异常");
            }
        }

        private async Task HandleTiming(BaseSession session, BaseReporter reporter, PixivRankingTimer rankingTimer, string content)
        {
            string rankingName = content.Trim().ToLower();
            PixivRankingHandler rankingHandler = new PixivRankingHandler(session, reporter);
            if (rankingName == "daily")
            {
                await rankingHandler.handleRankingSubscribeAsync(rankingTimer, BotConfig.PixivRankingConfig.Daily, PixivRankingMode.Daily);
                return;
            }
            if (rankingName == "dailyai")
            {
                await rankingHandler.handleRankingSubscribeAsync(rankingTimer, BotConfig.PixivRankingConfig.DailyAI, PixivRankingMode.DailyAI);
                return;
            }
            if (rankingName == "male")
            {
                await rankingHandler.handleRankingSubscribeAsync(rankingTimer, BotConfig.PixivRankingConfig.Male, PixivRankingMode.Male);
                return;
            }
            if (rankingName == "weekly")
            {
                await rankingHandler.handleRankingSubscribeAsync(rankingTimer, BotConfig.PixivRankingConfig.Weekly, PixivRankingMode.Weekly);
                return;
            }
            if (rankingName == "monthly")
            {
                await rankingHandler.handleRankingSubscribeAsync(rankingTimer, BotConfig.PixivRankingConfig.Monthly, PixivRankingMode.Monthly);
                return;
            }
            if (rankingName == "dailyr18")
            {
                await rankingHandler.handleRankingSubscribeAsync(rankingTimer, BotConfig.PixivRankingConfig.Daily, PixivRankingMode.Daily_R18);
                return;
            }
            if (rankingName == "dailyair18")
            {
                await rankingHandler.handleRankingSubscribeAsync(rankingTimer, BotConfig.PixivRankingConfig.DailyAI, PixivRankingMode.DailyAI_R18);
                return;
            }
            if (rankingName == "maler18")
            {
                await rankingHandler.handleRankingSubscribeAsync(rankingTimer, BotConfig.PixivRankingConfig.Male, PixivRankingMode.Male_R18);
                return;
            }
            if (rankingName == "weeklyr18")
            {
                await rankingHandler.handleRankingSubscribeAsync(rankingTimer, BotConfig.PixivRankingConfig.Weekly, PixivRankingMode.Weekly_R18);
                return;
            }

        }

    }
}
