using Quartz;
using TheresaBot.Core.Common;
using TheresaBot.Core.Handler;
using TheresaBot.Core.Helper;
using TheresaBot.Core.Mode;
using TheresaBot.Core.Model.Config;
using TheresaBot.Core.Reporter;
using TheresaBot.Core.Session;

namespace TheresaBot.Core.Timers
{
    [DisallowConcurrentExecution]
    internal class TimingRankingJob : IJob
    {
        private BaseReporter Reporter;

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var dataMap = context.MergedJobDataMap;
                var session = (BaseSession)dataMap["BaseSession"];
                var rankingTimer = (PixivRankingTimer)dataMap["PixivRankingTimer"];
                Reporter = (BaseReporter)dataMap["BaseReporter"];
                if (rankingTimer is null) return;
                if (rankingTimer.PushGroups.Count == 0) return;
                foreach (var content in rankingTimer.Contents)
                {
                    LogHelper.Info($"开始执行【{content}】Pixiv榜单推送任务...");
                    await HandleTiming(session, Reporter, rankingTimer, content);
                    LogHelper.Info($"Pixiv榜单【{content}】推送任务执行完毕...");
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "TimingRankingJob异常");
                await Reporter.SendError(ex, "TimingRankingJob异常");
            }
        }

        private async Task HandleTiming(BaseSession session, BaseReporter reporter, PixivRankingTimer rankingTimer, string content)
        {
            var rankingName = content.Trim().ToLower();
            var rankingHandler = new PixivRankingHandler(session, reporter);
            if (rankingName == "daily")
            {
                await rankingHandler.PushRankingAsync(rankingTimer, BotConfig.PixivRankingConfig.Daily, PixivRankingMode.Daily);
                return;
            }
            if (rankingName == "dailyai")
            {
                await rankingHandler.PushRankingAsync(rankingTimer, BotConfig.PixivRankingConfig.DailyAI, PixivRankingMode.DailyAI);
                return;
            }
            if (rankingName == "male")
            {
                await rankingHandler.PushRankingAsync(rankingTimer, BotConfig.PixivRankingConfig.Male, PixivRankingMode.Male);
                return;
            }
            if (rankingName == "weekly")
            {
                await rankingHandler.PushRankingAsync(rankingTimer, BotConfig.PixivRankingConfig.Weekly, PixivRankingMode.Weekly);
                return;
            }
            if (rankingName == "monthly")
            {
                await rankingHandler.PushRankingAsync(rankingTimer, BotConfig.PixivRankingConfig.Monthly, PixivRankingMode.Monthly);
                return;
            }
            if (rankingName == "dailyr18")
            {
                await rankingHandler.PushRankingAsync(rankingTimer, BotConfig.PixivRankingConfig.Daily, PixivRankingMode.Daily_R18);
                return;
            }
            if (rankingName == "dailyair18")
            {
                await rankingHandler.PushRankingAsync(rankingTimer, BotConfig.PixivRankingConfig.DailyAI, PixivRankingMode.DailyAI_R18);
                return;
            }
            if (rankingName == "maler18")
            {
                await rankingHandler.PushRankingAsync(rankingTimer, BotConfig.PixivRankingConfig.Male, PixivRankingMode.Male_R18);
                return;
            }
            if (rankingName == "weeklyr18")
            {
                await rankingHandler.PushRankingAsync(rankingTimer, BotConfig.PixivRankingConfig.Weekly, PixivRankingMode.Weekly_R18);
                return;
            }
        }

    }
}
