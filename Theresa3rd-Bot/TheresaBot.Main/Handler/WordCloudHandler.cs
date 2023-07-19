using System.Text.RegularExpressions;
using TheresaBot.Main.Business;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
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

        /// <summary>
        /// 回复自定义词云
        /// </summary>
        /// <param name="groupCommand"></param>
        /// <returns></returns>
        public async Task replyCustomWordCloudAsync(GroupCommand groupCommand)
        {
            if (groupCommand.Params.Length == 0)
            {
                await replyDailyWordCloudAsync(groupCommand);
                return;
            }

            string startTimeStr = string.Empty;
            string endTimeStr = string.Empty;
            string[] paramArr = groupCommand.Params;
            if (paramArr.Length == 2)
            {
                startTimeStr = paramArr[0] + " 00:00:00";
                endTimeStr = paramArr[1] + " 23:59:59";
            }
            else if (paramArr.Length == 4)
            {
                startTimeStr = paramArr[0] + " " + paramArr[1];
                endTimeStr = paramArr[2] + " " + paramArr[3];
            }

            if (startTimeStr.Length == 0 || endTimeStr.Length == 0)
            {
                await groupCommand.ReplyGroupMessageAsync($"错误的日期格式");
                return;
            }

            DateTime? startTime = startTimeStr.ToDateTime();
            DateTime? endTime = endTimeStr.ToDateTime();
            if (startTime is null || endTime is null)
            {
                await groupCommand.ReplyGroupMessageAsync($"错误的日期格式");
                return;
            }

            var remindMsg = $"自定义词云如下，统计时间段为：{startTime.Value.ToSimpleString()}-{endTime.Value.ToSimpleString()}";
            await replyWordCloudAsync(groupCommand, startTime.Value, endTime.Value, remindMsg);
        }

        /// <summary>
        /// 回复今日词云
        /// </summary>
        /// <param name="groupCommand"></param>
        /// <returns></returns>
        public async Task replyDailyWordCloudAsync(GroupCommand groupCommand)
        {
            DateTime startTime = DateTimeHelper.GetDayStart();
            DateTime endTime = DateTimeHelper.GetDayEnd();
            var remindMsg = $"今日词云如下，统计时间段为：{startTime.ToSimpleString()}-{endTime.ToSimpleString()}";
            await replyWordCloudAsync(groupCommand, startTime, endTime, remindMsg);
        }

        /// <summary>
        /// 回复本周词云
        /// </summary>
        /// <param name="groupCommand"></param>
        /// <returns></returns>
        public async Task replyWeeklyWordCloudAsync(GroupCommand groupCommand)
        {
            DateTime startTime = DateTimeHelper.GetWeekStart();
            DateTime endTime = DateTimeHelper.GetWeekEnd();
            var remindMsg = $"本周词云如下，统计时间段为：{startTime.ToSimpleString()}-{endTime.ToSimpleString()}";
            await replyWordCloudAsync(groupCommand, startTime, endTime, remindMsg);
        }

        /// <summary>
        /// 回复本月词云
        /// </summary>
        /// <param name="groupCommand"></param>
        /// <returns></returns>
        public async Task replyMonthlyWordCloudAsync(GroupCommand groupCommand)
        {
            DateTime startTime = DateTimeHelper.GetWeekStart();
            DateTime endTime = DateTimeHelper.GetWeekEnd();
            var remindMsg = $"本月词云如下，统计时间段为：{startTime.ToSimpleString()}-{endTime.ToSimpleString()}";
            await replyWordCloudAsync(groupCommand, startTime, endTime, remindMsg);
        }

        /// <summary>
        /// 回复本年词云
        /// </summary>
        /// <param name="groupCommand"></param>
        /// <returns></returns>
        public async Task replyYearlyWordCloudAsync(GroupCommand groupCommand)
        {
            DateTime startTime = DateTimeHelper.GetYearStart();
            DateTime endTime = DateTimeHelper.GetYearEnd();
            var remindMsg = $"本年词云如下，统计时间段为：{startTime.ToSimpleString()}-{endTime.ToSimpleString()}";
            await replyWordCloudAsync(groupCommand, startTime, endTime, remindMsg);
        }

        /// <summary>
        /// 回复昨日词云
        /// </summary>
        /// <param name="groupCommand"></param>
        /// <returns></returns>
        public async Task replyYesterdayWordCloudAsync(GroupCommand groupCommand)
        {
            DateTime startTime = DateTimeHelper.GetYesterdayStart();
            DateTime endTime = DateTimeHelper.GetYesterdayEnd();
            var remindMsg = $"昨日词云如下，统计时间段为：{startTime.ToSimpleString()}-{endTime.ToSimpleString()}";
            await replyWordCloudAsync(groupCommand, startTime, endTime, remindMsg);
        }

        /// <summary>
        /// 回复上周词云
        /// </summary>
        /// <param name="groupCommand"></param>
        /// <returns></returns>
        public async Task replyLastWeekWordCloudAsync(GroupCommand groupCommand)
        {
            DateTime startTime = DateTimeHelper.GetLastWeekStart();
            DateTime endTime = DateTimeHelper.GetLastWeekEnd();
            var remindMsg = $"上周词云如下，统计时间段为：{startTime.ToSimpleString()}-{endTime.ToSimpleString()}";
            await replyWordCloudAsync(groupCommand, startTime, endTime, remindMsg);
        }

        /// <summary>
        /// 回复上月词云
        /// </summary>
        /// <param name="groupCommand"></param>
        /// <returns></returns>
        public async Task replyLastMonthWordCloudAsync(GroupCommand groupCommand)
        {
            DateTime startTime = DateTimeHelper.GetLastMonthStart();
            DateTime endTime = DateTimeHelper.GetLastMonthEnd();
            var remindMsg = $"上月词云如下，统计时间段为：{startTime.ToSimpleString()}-{endTime.ToSimpleString()}";
            await replyWordCloudAsync(groupCommand, startTime, endTime, remindMsg);
        }

        /// <summary>
        /// 合成词云并回复
        /// </summary>
        /// <param name="groupCommand"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="remindMsg"></param>
        /// <returns></returns>
        private async Task replyWordCloudAsync(GroupCommand groupCommand, DateTime startTime, DateTime endTime, string remindMsg)
        {
            try
            {
                long groupId = groupCommand.GroupId;
                CoolingCache.SetWordCloudHanding(groupId);
                List<string> words = recordBusiness.getCloudWords(groupId, startTime, endTime);
                if (words is null || words.Count == 0)
                {
                    await groupCommand.ReplyGroupMessageWithAtAsync("未能获取足够数量的聊天记录，词云生成失败了");
                    return;
                }
                FileInfo wordCloudFile = await new WordCloudDrawer().DrawWordCloud(words);
                if (string.IsNullOrWhiteSpace(remindMsg) == false)
                {
                    await groupCommand.ReplyGroupMessageWithAtAsync(remindMsg);
                    await Task.Delay(1000);
                }
                List<BaseContent> wordCloudContnts = new List<BaseContent>();
                wordCloudContnts.Add(new LocalImageContent(wordCloudFile));
                await groupCommand.ReplyGroupMessageAsync(wordCloudContnts);
            }
            catch (Exception ex)
            {
                string errMsg = $"replyWordCloudAsync异常";
                LogHelper.Error(ex, errMsg);
                await Reporter.SendError(ex, errMsg);
            }
            finally
            {
                CoolingCache.SetWordCloudHandFinish(groupCommand.GroupId);
            }
        }

        /// <summary>
        /// 推送每日词云
        /// </summary>
        /// <returns></returns>
        public async Task pushDailyWordCloudAsync()
        {
            DateTime startTime = DateTime.Now;
            DateTime endTime = startTime.AddHours(-24);
            var subscribeModel = BotConfig.WordCloudConfig.Subscribes.Daily;
            var groupIds = subscribeModel.Groups;
            var remindMsg = subscribeModel.Template;
            foreach (var groupId in groupIds)
            {
                Task task = pushWordCloudAsync(groupId, startTime, endTime, remindMsg);
                await Task.Delay(1000);
            }
        }

        /// <summary>
        /// 推送每周词云
        /// </summary>
        /// <returns></returns>
        public async Task pushWeeklyWordCloudAsync()
        {
            DateTime startTime = DateTime.Now;
            DateTime endTime = startTime.AddDays(-7);
            var subscribeModel = BotConfig.WordCloudConfig.Subscribes.Weekly;
            var groupIds = subscribeModel.Groups;
            var remindMsg = subscribeModel.Template;
            foreach (var groupId in groupIds)
            {
                Task task = pushWordCloudAsync(groupId, startTime, endTime, remindMsg);
                await Task.Delay(1000);
            }
        }

        /// <summary>
        /// 推送每月词云
        /// </summary>
        /// <returns></returns>
        public async Task pushMonthlyWordCloudAsync()
        {
            DateTime startTime = DateTime.Now;
            DateTime endTime = startTime.AddDays(-30);
            var subscribeModel = BotConfig.WordCloudConfig.Subscribes.Monthly;
            var groupIds = subscribeModel.Groups;
            var remindMsg = subscribeModel.Template;
            foreach (var groupId in groupIds)
            {
                Task task = pushWordCloudAsync(groupId, startTime, endTime, remindMsg);
                await Task.Delay(1000);
            }
        }

        /// <summary>
        /// 合成并推送词云
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="remindMsg"></param>
        /// <returns></returns>
        private async Task pushWordCloudAsync(long groupId, DateTime startTime, DateTime endTime, string remindMsg)
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
