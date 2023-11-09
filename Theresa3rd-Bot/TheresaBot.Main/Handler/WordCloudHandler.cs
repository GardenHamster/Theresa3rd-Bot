using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Drawer;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Services;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Handler
{
    internal class WordCloudHandler : BaseHandler
    {
        private RecordService recordService;
        private DictionaryService dictionaryService;
        private WordCloudService wordCloudService;

        public WordCloudHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            recordService = new RecordService();
            dictionaryService = new DictionaryService();
            wordCloudService = new WordCloudService();
        }

        /// <summary>
        /// 回复自定义词云
        /// </summary>
        /// <param name="groupCommand"></param>
        /// <returns></returns>
        public async Task ReplyCustomWordCloudAsync(GroupCommand groupCommand)
        {
            if (groupCommand.Params.Length == 0)
            {
                await ReplyDailyWordCloudAsync(groupCommand);
                return;
            }

            string startTimeStr = string.Empty;
            string endTimeStr = string.Empty;
            string[] paramArr = groupCommand.Params;
            if (paramArr.Length == 1)
            {
                startTimeStr = paramArr[0] + " 00:00:00";
                endTimeStr = paramArr[0] + " 23:59:59";
            }
            else if (paramArr.Length == 2)
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
                await groupCommand.ReplyGroupMessageWithQuoteAsync($"错误的日期格式");
                return;
            }

            DateTime? startTime = startTimeStr.ToDateTime();
            DateTime? endTime = endTimeStr.ToDateTime();
            if (startTime is null || endTime is null)
            {
                await groupCommand.ReplyGroupMessageWithQuoteAsync($"错误的日期格式");
                return;
            }

            var remindMsg = $"自定义词云如下，统计时间段为：{startTime.Value.ToSimpleString()} 至 {endTime.Value.ToSimpleString()}";
            await ReplyWordCloudAsync(groupCommand, startTime.Value, endTime.Value, remindMsg);
        }

        /// <summary>
        /// 回复今日词云
        /// </summary>
        /// <param name="groupCommand"></param>
        /// <returns></returns>
        public async Task ReplyDailyWordCloudAsync(GroupCommand groupCommand)
        {
            DateTime startTime = DateTimeHelper.GetDayStart();
            DateTime endTime = DateTimeHelper.GetDayEnd();
            var remindMsg = $"今日词云如下，统计时间段为：{startTime.ToSimpleString()} 至 {endTime.ToSimpleString()}";
            await ReplyWordCloudAsync(groupCommand, startTime, endTime, remindMsg);
        }

        /// <summary>
        /// 回复本周词云
        /// </summary>
        /// <param name="groupCommand"></param>
        /// <returns></returns>
        public async Task ReplyWeeklyWordCloudAsync(GroupCommand groupCommand)
        {
            DateTime startTime = DateTimeHelper.GetWeekStart();
            DateTime endTime = DateTimeHelper.GetWeekEnd();
            var remindMsg = $"本周词云如下，统计时间段为：{startTime.ToSimpleString()} 至 {endTime.ToSimpleString()}";
            await ReplyWordCloudAsync(groupCommand, startTime, endTime, remindMsg);
        }

        /// <summary>
        /// 回复本月词云
        /// </summary>
        /// <param name="groupCommand"></param>
        /// <returns></returns>
        public async Task ReplyMonthlyWordCloudAsync(GroupCommand groupCommand)
        {
            DateTime startTime = DateTimeHelper.GetWeekStart();
            DateTime endTime = DateTimeHelper.GetWeekEnd();
            var remindMsg = $"本月词云如下，统计时间段为：{startTime.ToSimpleString()} 至 {endTime.ToSimpleString()}";
            await ReplyWordCloudAsync(groupCommand, startTime, endTime, remindMsg);
        }

        /// <summary>
        /// 回复本年词云
        /// </summary>
        /// <param name="groupCommand"></param>
        /// <returns></returns>
        public async Task ReplyYearlyWordCloudAsync(GroupCommand groupCommand)
        {
            DateTime startTime = DateTimeHelper.GetYearStart();
            DateTime endTime = DateTimeHelper.GetYearEnd();
            var remindMsg = $"本年词云如下，统计时间段为：{startTime.ToSimpleString()} 至 {endTime.ToSimpleString()}";
            await ReplyWordCloudAsync(groupCommand, startTime, endTime, remindMsg);
        }

        /// <summary>
        /// 回复昨日词云
        /// </summary>
        /// <param name="groupCommand"></param>
        /// <returns></returns>
        public async Task ReplyYesterdayWordCloudAsync(GroupCommand groupCommand)
        {
            DateTime startTime = DateTimeHelper.GetYesterdayStart();
            DateTime endTime = DateTimeHelper.GetYesterdayEnd();
            var remindMsg = $"昨日词云如下，统计时间段为：{startTime.ToSimpleString()} 至 {endTime.ToSimpleString()}";
            await ReplyWordCloudAsync(groupCommand, startTime, endTime, remindMsg);
        }

        /// <summary>
        /// 回复上周词云
        /// </summary>
        /// <param name="groupCommand"></param>
        /// <returns></returns>
        public async Task ReplyLastWeekWordCloudAsync(GroupCommand groupCommand)
        {
            DateTime startTime = DateTimeHelper.GetLastWeekStart();
            DateTime endTime = DateTimeHelper.GetLastWeekEnd();
            var remindMsg = $"上周词云如下，统计时间段为：{startTime.ToSimpleString()} 至 {endTime.ToSimpleString()}";
            await ReplyWordCloudAsync(groupCommand, startTime, endTime, remindMsg);
        }

        /// <summary>
        /// 回复上月词云
        /// </summary>
        /// <param name="groupCommand"></param>
        /// <returns></returns>
        public async Task ReplyLastMonthWordCloudAsync(GroupCommand groupCommand)
        {
            DateTime startTime = DateTimeHelper.GetLastMonthStart();
            DateTime endTime = DateTimeHelper.GetLastMonthEnd();
            var remindMsg = $"上月词云如下，统计时间段为：{startTime.ToSimpleString()} 至 {endTime.ToSimpleString()}";
            await ReplyWordCloudAsync(groupCommand, startTime, endTime, remindMsg);
        }

        /// <summary>
        /// 合成词云并回复
        /// </summary>
        /// <param name="groupCommand"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="remindMsg"></param>
        /// <returns></returns>
        private async Task ReplyWordCloudAsync(GroupCommand groupCommand, DateTime startTime, DateTime endTime, string remindMsg)
        {
            try
            {
                long groupId = groupCommand.GroupId;
                CoolingCache.SetWordCloudHanding(groupId);
                await groupCommand.ReplyProcessingMessageAsync(BotConfig.WordCloudConfig.ProcessingMsg);
                List<string> words = wordCloudService.getCloudWords(groupId, startTime, endTime);
                if (words is null || words.Count == 0)
                {
                    await groupCommand.ReplyGroupMessageWithQuoteAsync("未能获取足够数量的聊天记录，词云生成失败了");
                    return;
                }
                List<BaseContent> contents = new List<BaseContent>();
                if (string.IsNullOrWhiteSpace(remindMsg) == false)
                {
                    contents.Add(new PlainContent(remindMsg));
                }
                var maskNames = BotConfig.WordCloudConfig.DefaultMasks ?? new();
                var maskItem = GetRandomMaskItem(maskNames);
                var wordCloudFile = await new WordCloudDrawer().DrawWordCloud(words, maskItem);
                contents.Add(new LocalImageContent(wordCloudFile));
                await groupCommand.ReplyGroupMessageWithQuoteAsync(contents);
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
        /// 获取随机的可用的蒙版文件
        /// </summary>
        /// <param name="maskNames"></param>
        /// <returns></returns>
        private WordCloudMask GetRandomMaskItem(List<string> maskNames)
        {
            var maskItems = maskNames.Select(o => GetMaskItem(o)).Where(o => o is not null).ToList();
            if (maskItems.Count == 0) return null;
            return maskItems.RandomItem();
        }

        /// <summary>
        /// 获取蒙版文件，如果蒙版不存在，返回null
        /// </summary>
        /// <param name="maskName"></param>
        /// <returns></returns>
        private WordCloudMask GetMaskItem(string maskName)
        {
            if (string.IsNullOrWhiteSpace(maskName)) return null;
            return BotConfig.WordCloudConfig.Masks?.FirstOrDefault(o => o.Name == maskName);
        }

        /// <summary>
        /// 推送词云
        /// </summary>
        /// <returns></returns>
        public async Task PushWordCloudAsync(WordCloudTimer timer)
        {
            var groupList = timer.Groups.ToSendableGroups();
            foreach (var groupId in groupList)
            {
                Task task = PushWordCloudAsync(timer, groupId);
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
        private async Task PushWordCloudAsync(WordCloudTimer timer, long groupId)
        {
            try
            {
                CoolingCache.SetWordCloudHanding(groupId);
                DateTime endTime = DateTime.Now;
                DateTime startTime = endTime.AddHours(-1 * timer.HourRange);
                List<string> words = wordCloudService.getCloudWords(groupId, startTime, endTime);
                if (words is null || words.Count == 0) return;
                List<BaseContent> contents = new List<BaseContent>();
                if (string.IsNullOrWhiteSpace(timer.Template) == false)
                {
                    contents.Add(new PlainContent(timer.Template));
                }
                var maskNames = timer.Masks ?? new();
                var maskItem = GetRandomMaskItem(maskNames);
                FileInfo wordCloudFile = await new WordCloudDrawer().DrawWordCloud(words, maskItem);
                contents.Add(new LocalImageContent(wordCloudFile));
                await Session.SendGroupMessageAsync(groupId, contents);
            }
            catch (Exception ex)
            {
                string errMsg = $"pushWordCloudAsync异常";
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
