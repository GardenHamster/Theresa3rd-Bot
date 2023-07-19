using TheresaBot.Main.Business;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Common;
using TheresaBot.Main.Drawer;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Handler
{
    internal class WordCloudHandler : BaseHandler
    {
        private RecordBusiness recordBusiness;

        public WordCloudHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            recordBusiness = new RecordBusiness();
        }

        public async Task sendDailyWordCloud()
        {
            DateTime startTime = DateTimeHelper.GetTodayStart();
            DateTime endTime = DateTimeHelper.GetTodayEnd();
            var subscribeModel = BotConfig.WordCloudConfig.Subscribes.Daily;
            var groupIds = subscribeModel.Groups;
            var remindMsg = subscribeModel.Template;
            foreach (var groupId in groupIds)
            {
                Task task = sendWordCloudAsync(groupId, startTime, endTime, remindMsg);
                await Task.Delay(1000);
            }
        }

        public async Task sendWeeklyWordCloud()
        {
            DateTime startTime = DateTimeHelper.GetWeekStart();
            DateTime endTime = DateTimeHelper.GetWeekEnd();
            var subscribeModel = BotConfig.WordCloudConfig.Subscribes.Weekly;
            var groupIds = subscribeModel.Groups;
            var remindMsg = subscribeModel.Template;
            foreach (var groupId in groupIds)
            {
                Task task = sendWordCloudAsync(groupId, startTime, endTime, remindMsg);
                await Task.Delay(1000);
            }
        }

        public async Task sendMonthlyWordCloud()
        {
            DateTime startTime = DateTimeHelper.GetMonthStart();
            DateTime endTime = DateTimeHelper.GetMonthEnd();
            var subscribeModel = BotConfig.WordCloudConfig.Subscribes.Monthly;
            var groupIds = subscribeModel.Groups;
            var remindMsg = subscribeModel.Template;
            foreach (var groupId in groupIds)
            {
                Task task = sendWordCloudAsync(groupId, startTime, endTime, remindMsg);
                await Task.Delay(1000);
            }
        }

        private async Task sendWordCloudAsync(long groupId, DateTime startTime, DateTime endTime, string remindMsg)
        {
            try
            {
                CoolingCache.SetWordCloudHanding(groupId);
                List<string> words = recordBusiness.getCloudWords(groupId, startTime, endTime);
                if (words is null || words.Count == 0) return;
                FileInfo wordCloudFile = await new WordCloudDrawer().DrawWordCloud(words);
                if (string.IsNullOrWhiteSpace(remindMsg) == false)
                {
                    await Session.SendGroupMessageAsync(groupId, remindMsg);
                    await Task.Delay(1000);
                }
                List<BaseContent> wordCloudContnts = new List<BaseContent>();
                wordCloudContnts.Add(new LocalImageContent(wordCloudFile));
                await Session.SendGroupMessageAsync(groupId, wordCloudContnts);
            }
            catch (Exception ex)
            {
                string errMsg = $"sendWordCloudAsync异常";
                LogHelper.Error(ex, errMsg);
                await Reporter.SendError(ex, errMsg);
            }
            finally
            {
                CoolingCache.SetWordCloudHandFinish(groupId);
            }
        }





    }
}
