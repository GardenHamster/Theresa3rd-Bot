using TheresaBot.Main.Common;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Pixiv;
using TheresaBot.Main.Model.Subscribe;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Services;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Handler
{
    internal class PixivPushHandler : SetuHandler
    {
        private PixivService pixivService;

        public PixivPushHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            pixivService = new PixivService();
        }

        public async Task<PixivTagScanReport> HandleTagPushAsync()
        {
            var subscribeType = SubscribeType.P站标签;
            var scanReport = new PixivTagScanReport();
            var sendMerge = BotConfig.SubscribeConfig.PixivTag.SendMerge;
            var subscribeTasks = SubscribeDatas.GetSubscribeTasks(subscribeType);
            if (subscribeTasks.Count == 0) return scanReport;
            Func<PixivSubscribe, Task> pushAsync = sendMerge ? null : PushTagWorkAsync;
            List<PixivSubscribe> pushList = new List<PixivSubscribe>();
            foreach (SubscribeTask subscribeTask in subscribeTasks)
            {
                try
                {
                    if (subscribeTask.SubscribeSubType != 0) continue;
                    if (subscribeTask.SubscribeGroups.Count == 0) continue;
                    scanReport.ScanTag++;
                    var workList = await pixivService.scanTagWorkAsync(subscribeTask, scanReport, pushAsync);
                    pushList.AddRange(workList);
                }
                catch (Exception ex)
                {
                    scanReport.ErrorTag++;
                    await LogAndReportError(ex, $"扫描pixiv标签[{subscribeTask.SubscribeCode}]最新作品失败");
                }
            }
            if (pushList.Count > 0)
            {
                await pixivService.insertSubscribeRecord(pushList);
                await PushMergeAsync(pushList, o => pixivService.getTagPushRemindMsg(o));
            }
            return scanReport;
        }

        public async Task<PixivUserScanReport> HandleUserPushAsync()
        {
            var subscribeType = SubscribeType.P站画师;
            var scanReport = new PixivUserScanReport();
            var sendMerge = BotConfig.SubscribeConfig.PixivUser.SendMerge;
            var subscribeTasks = SubscribeDatas.GetSubscribeTasks(subscribeType);
            if (subscribeTasks.Count == 0) return scanReport;
            Func<PixivSubscribe, Task> pushAsync = sendMerge ? null : PushUserWorkAsync;
            List<PixivSubscribe> pushList = new List<PixivSubscribe>();
            foreach (SubscribeTask subscribeTask in subscribeTasks)
            {
                try
                {
                    if (subscribeTask.SubscribeSubType != 0) continue;
                    if (subscribeTask.SubscribeGroups.Count == 0) continue;
                    scanReport.ScanUser++;
                    var workList = await pixivService.scanUserWorkAsync(subscribeTask, scanReport, pushAsync);
                    pushList.AddRange(workList);
                }
                catch (Exception ex)
                {
                    scanReport.ErrorUser++;
                    await LogAndReportError(ex, $"扫描pixiv用户[{subscribeTask.SubscribeCode}]最新作品失败");
                }
            }
            if (pushList.Count > 0)
            {
                await pixivService.insertSubscribeRecord(pushList);
                await PushMergeAsync(pushList, o => pixivService.getUserPushRemindMsg(o));
            }
            return scanReport;
        }

        public async Task<PixivScanReport> HandleFollowPushAsync()
        {
            var scanReport = new PixivScanReport();
            var sendMerge = BotConfig.SubscribeConfig.PixivUser.SendMerge;
            Func<PixivSubscribe, Task> pushAsync = sendMerge ? null : PushFollowWorkAsync;
            var pushList = await pixivService.scanFollowWorkAsync(scanReport, pushAsync);
            if (pushList.Count == 0) return scanReport;
            await pixivService.insertSubscribeRecord(pushList);
            foreach (var groupId in BotConfig.SubscribeGroups)
            {
                await PushMergeAsync(pushList, o => pixivService.getUserPushRemindMsg(o), groupId);
            }
            return scanReport;
        }

        /// <summary>
        /// 逐条推送
        /// </summary>
        /// <param name="pixivSubscribe"></param>
        /// <returns></returns>
        private async Task PushTagWorkAsync(PixivSubscribe pixivSubscribe)
        {
            foreach (long groupId in pixivSubscribe.SubscribeTask.SubscribeGroups)
            {
                await PushAsync(pixivSubscribe, o => pixivService.getTagPushRemindMsg(pixivSubscribe), groupId);
                await Task.Delay(1000);
            }
        }

        /// <summary>
        /// 逐条推送
        /// </summary>
        /// <param name="pixivSubscribe"></param>
        /// <returns></returns>
        private async Task PushUserWorkAsync(PixivSubscribe pixivSubscribe)
        {
            foreach (long groupId in pixivSubscribe.SubscribeTask.SubscribeGroups)
            {
                await PushAsync(pixivSubscribe, o => pixivService.getUserPushRemindMsg(pixivSubscribe), groupId);
                await Task.Delay(1000);
            }
        }

        /// <summary>
        /// 逐条推送
        /// </summary>
        /// <param name="pixivSubscribe"></param>
        /// <returns></returns>
        private async Task PushFollowWorkAsync(PixivSubscribe pixivSubscribe)
        {
            foreach (long groupId in BotConfig.SubscribeGroups)
            {
                await PushAsync(pixivSubscribe, o => pixivService.getUserPushRemindMsg(pixivSubscribe), groupId);
                await Task.Delay(1000);
            }
        }

        /// <summary>
        /// 合并推送
        /// </summary>
        /// <param name="pixivSubscribes"></param>
        /// <returns></returns>
        private async Task PushMergeAsync(List<PixivSubscribe> pixivSubscribes, Func<PixivSubscribe, string> remindMsg)
        {
            var sendDic = new Dictionary<long, List<PixivSubscribe>>();
            foreach (var item in pixivSubscribes)
            {
                if (item.SubscribeTask is null) continue;
                var groupIds = item.SubscribeTask.SubscribeGroups;
                if (groupIds.Count == 0) continue;
                foreach (var groupId in groupIds)
                {
                    if (!sendDic.ContainsKey(groupId)) sendDic[groupId] = new();
                    sendDic[groupId].Add(item);
                }
            }
            foreach (var item in sendDic)
            {
                var groupId = item.Key;
                var workInfos = item.Value;
                if (workInfos.Count == 0) continue;
                await PushMergeAsync(workInfos, remindMsg, groupId);
            }
        }

        /// <summary>
        /// 逐条推送
        /// </summary>
        /// <param name="pixivSubscribe"></param>
        /// <param name="remindMsg"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        private async Task PushAsync(PixivSubscribe pixivSubscribe, Func<PixivSubscribe, string> remindMsg, long groupId)
        {
            try
            {
                var workInfo = pixivSubscribe.PixivWorkInfo;
                var isAISetu = workInfo.IsAI;
                var isR18Img = workInfo.IsR18;
                if (isR18Img && groupId.IsShowR18() == false) return;
                if (isAISetu && groupId.IsShowAISetu() == false) return;
                var workMsgs = new List<BaseContent>
                {
                    new PlainContent(remindMsg(pixivSubscribe)),
                    new PlainContent(pixivService.getWorkInfo(workInfo))
                };
                var isShowImg = groupId.IsShowSetuImg(isR18Img);
                var imgList = isShowImg ? await GetSetuFilesAsync(workInfo, groupId) : new();
                var setuContent = new PixivSetuContent(workMsgs, imgList, workInfo);
                await SendGroupSetuAsync(setuContent, groupId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "pixiv订阅消息发送失败");
            }
        }

        /// <summary>
        /// 合并推送
        /// </summary>
        /// <param name="pixivSubscribes"></param>
        /// <param name="remindMsg"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        private async Task PushMergeAsync(List<PixivSubscribe> pixivSubscribes, Func<PixivSubscribe, string> remindMsg, long groupId)
        {
            try
            {
                var eachPage = 10;
                if (pixivSubscribes.Count == 0) return;
                var pixivContents = new List<PixivSetuContent>();
                foreach (var pixivSubscribe in pixivSubscribes)
                {
                    var workInfo = pixivSubscribe.PixivWorkInfo;
                    var isAISetu = workInfo.IsAI;
                    var isR18Img = workInfo.IsR18;
                    if (isR18Img && groupId.IsShowR18() == false) continue;
                    if (isAISetu && groupId.IsShowAISetu() == false) continue;
                    var remindTemplate = BotConfig.SubscribeConfig.PixivUser.Template;
                    var pixivTemplate = BotConfig.PixivConfig.Template;
                    var setuFiles = await GetSetuFilesAsync(workInfo, groupId);
                    var workMsgs = new List<BaseContent>
                    {
                        new PlainContent(remindMsg(pixivSubscribe)),
                        new PlainContent(pixivService.getWorkInfo(workInfo))
                    };
                    var isShowImg = groupId.IsShowSetuImg(isR18Img);
                    var imgList = isShowImg ? setuFiles : new();
                    var setuContent = new PixivSetuContent(workMsgs, imgList, workInfo);
                    pixivContents.Add(setuContent);
                }
                var setuContents = pixivContents.Cast<SetuContent>().ToList();
                var results = SendGroupMergeSetuAsync(setuContents, new(), groupId, eachPage);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "pixiv订阅消息发送失败");
            }
        }

    }
}
