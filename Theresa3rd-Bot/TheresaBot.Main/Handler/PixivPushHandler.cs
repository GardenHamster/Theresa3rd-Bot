using TheresaBot.Main.Business;
using TheresaBot.Main.Common;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Pixiv;
using TheresaBot.Main.Model.Subscribe;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Handler
{
    internal class PixivPushHandler : SetuHandler
    {
        private PixivBusiness pixivBusiness;

        public PixivPushHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            pixivBusiness = new PixivBusiness();
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
                    scanReport.ScanTag++;
                    var workList = await pixivBusiness.scanTagWorkAsync(subscribeTask, scanReport, pushAsync);
                    pushList.AddRange(workList);
                }
                catch (Exception ex)
                {
                    scanReport.ErrorTag++;
                    LogHelper.Error(ex, $"扫描pixiv标签[{subscribeTask.SubscribeCode}]订阅失败");
                }
            }
            if (pushList.Count > 0)
            {
                Task pushTask = PushPixivWorkMergeAsync(pushList, o => pixivBusiness.getTagPushRemindMsg(o));
                Task insertTask = pixivBusiness.insertSubscribeRecord(pushList);
            }
            return scanReport;
        }

        public async Task<PixivUserScanReport> HandleUserPushAsync()
        {
            var subscribeType = SubscribeType.P站画师;
            var scanReport = new PixivUserScanReport();
            var sendMerge = BotConfig.SubscribeConfig.PixivTag.SendMerge;
            var subscribeTasks = SubscribeDatas.GetSubscribeTasks(subscribeType);
            if (subscribeTasks.Count == 0) return scanReport;
            Func<PixivSubscribe, Task> pushAsync = sendMerge ? null : PushUserWorkAsync;
            List<PixivSubscribe> pushList = new List<PixivSubscribe>();
            foreach (SubscribeTask subscribeTask in subscribeTasks)
            {
                try
                {
                    if (subscribeTask.SubscribeSubType != 0) continue;
                    scanReport.ScanUser++;
                    var workList = await pixivBusiness.scanUserWorkAsync(subscribeTask, scanReport, pushAsync);
                    pushList.AddRange(workList);
                }
                catch (Exception ex)
                {
                    scanReport.ErrorUser++;
                    string message = $"扫描pixiv用户[{subscribeTask.SubscribeCode}]订阅失败";
                    LogHelper.Error(ex, message);
                    await Reporter.SendError(ex, message);
                }
            }
            if (pushList.Count > 0)
            {
                Task pushTask = PushPixivWorkMergeAsync(pushList, o => pixivBusiness.getUserPushRemindMsg(o));
                Task insertTask = pixivBusiness.insertSubscribeRecord(pushList);
            }
            return scanReport;
        }

        public async Task<PixivUserScanReport> HandleFollowPushAsync()
        {
            PixivUserScanReport scanReport = new PixivUserScanReport();
            try
            {
                bool sendMerge = BotConfig.SubscribeConfig.PixivTag.SendMerge;
                Func<PixivSubscribe, Task> pushAsync = sendMerge ? null : PushUserWorkAsync;
                var pushList = await pixivBusiness.scanFollowWorkAsync(scanReport, pushAsync);
                if (pushList.Count > 0)
                {
                    Task pushTask = PushPixivWorkMergeAsync(pushList, o => pixivBusiness.getUserPushRemindMsg(o));
                    Task insertTask = pixivBusiness.insertSubscribeRecord(pushList);
                }
            }
            catch (Exception ex)
            {
                string message = $"扫描pixiv关注用户最新作品失败";
                LogHelper.Error(ex, message);
                await Reporter.SendError(ex, message);
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
            foreach (long groupId in pixivSubscribe.SubscribeTask.GroupIdList)
            {
                await PushPixivWorkAsync(pixivSubscribe, o => pixivBusiness.getTagPushRemindMsg(pixivSubscribe), groupId);
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
            foreach (long groupId in pixivSubscribe.SubscribeTask.GroupIdList)
            {
                await PushPixivWorkAsync(pixivSubscribe, o => pixivBusiness.getUserPushRemindMsg(pixivSubscribe), groupId);
                await Task.Delay(1000);
            }
        }


        /// <summary>
        /// 合并推送
        /// </summary>
        /// <param name="pixivSubscribes"></param>
        /// <returns></returns>
        private async Task PushPixivWorkMergeAsync(List<PixivSubscribe> pixivSubscribes, Func<PixivSubscribe, string> remindMsg)
        {
            var sendDic = new Dictionary<long, List<PixivSubscribe>>();
            foreach (var item in pixivSubscribes)
            {
                if (item.SubscribeTask is null) continue;
                List<long> groupIds = item.SubscribeTask.GroupIdList;
                if (groupIds is null || groupIds.Count == 0) continue;
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
                await PushPixivWorkMergeAsync(workInfos, remindMsg, groupId);
            }
        }

        /// <summary>
        /// 逐条推送
        /// </summary>
        /// <param name="pixivSubscribe"></param>
        /// <param name="remindMsg"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        private async Task PushPixivWorkAsync(PixivSubscribe pixivSubscribe, Func<PixivSubscribe, string> remindMsg, long groupId)
        {
            try
            {
                PixivWorkInfo workInfo = pixivSubscribe.PixivWorkInfo;
                bool isAISetu = workInfo.IsAI;
                bool isR18Img = workInfo.IsR18;
                if (isR18Img && groupId.IsShowR18Setu() == false) return;
                if (isAISetu && groupId.IsShowAISetu() == false) return;

                var workMsgs = new List<BaseContent>();
                workMsgs.Add(new PlainContent(remindMsg(pixivSubscribe)));
                workMsgs.Add(new PlainContent(pixivBusiness.getWorkInfo(workInfo)));

                bool isShowImg = groupId.IsShowSetuImg(isR18Img);
                List<FileInfo> imgList = isShowImg ? await GetSetuFilesAsync(workInfo, groupId) : new();
                PixivSetuContent setuContent = new PixivSetuContent(workMsgs, imgList, workInfo);
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
        private async Task PushPixivWorkMergeAsync(List<PixivSubscribe> pixivSubscribes, Func<PixivSubscribe, string> remindMsg, long groupId)
        {
            try
            {
                int eachPage = 10;
                if (pixivSubscribes.Count == 0) return;
                var pixivContents = new List<PixivSetuContent>();
                foreach (var pixivSubscribe in pixivSubscribes)
                {
                    PixivWorkInfo workInfo = pixivSubscribe.PixivWorkInfo;
                    bool isAISetu = workInfo.IsAI;
                    bool isR18Img = workInfo.IsR18;
                    if (isR18Img && groupId.IsShowR18Setu() == false) continue;
                    if (isAISetu && groupId.IsShowAISetu() == false) continue;
                    string remindTemplate = BotConfig.SubscribeConfig.PixivUser.Template;
                    string pixivTemplate = BotConfig.PixivConfig.Template;
                    List<FileInfo> setuFiles = await GetSetuFilesAsync(workInfo, groupId);

                    var workMsgs = new List<BaseContent>();
                    workMsgs.Add(new PlainContent(remindMsg(pixivSubscribe)));
                    workMsgs.Add(new PlainContent(pixivBusiness.getWorkInfo(workInfo)));

                    bool isShowImg = groupId.IsShowSetuImg(isR18Img);
                    List<FileInfo> imgList = isShowImg ? setuFiles : new();
                    PixivSetuContent setuContent = new PixivSetuContent(workMsgs, imgList, workInfo);
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
