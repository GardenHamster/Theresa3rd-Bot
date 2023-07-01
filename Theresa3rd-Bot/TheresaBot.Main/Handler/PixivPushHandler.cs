using TheresaBot.Main.Business;
using TheresaBot.Main.Common;
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
            SubscribeType subscribeType = SubscribeType.P站标签;
            PixivTagScanReport scanReport = new PixivTagScanReport();
            if (BotConfig.SubscribeTaskMap.ContainsKey(subscribeType) == false) return scanReport;
            List<SubscribeTask> subscribeTaskList = BotConfig.SubscribeTaskMap[subscribeType];
            if (subscribeTaskList is null || subscribeTaskList.Count == 0) return scanReport;
            foreach (SubscribeTask subscribeTask in subscribeTaskList)
            {
                try
                {
                    if (subscribeTask.SubscribeSubType != 0) continue;
                    scanReport.ScanTag++;
                    await pixivBusiness.scanAndPushTagAsync(subscribeTask, scanReport, PushTagWorkAsync);
                }
                catch (Exception ex)
                {
                    scanReport.ErrorTag++;
                    LogHelper.Error(ex, $"扫描pixiv标签[{subscribeTask.SubscribeCode}]订阅失败");
                }
            }
            return scanReport;
        }

        private async Task PushTagWorkAsync(SubscribeTask subscribeTask, PixivSubscribe pixivSubscribe)
        {
            List<long> groupIds = subscribeTask.GroupIdList;
            PixivWorkInfo pixivWorkInfo = pixivSubscribe.PixivWorkInfo;
            if (pixivWorkInfo is null || pixivWorkInfo.IsImproper || pixivWorkInfo.hasBanTag() != null) return;
            if (groupIds is null || groupIds.Count == 0) return;
            if (pixivWorkInfo.IsAI && groupIds.IsShowAISetu() == false) return;

            bool isAISetu = pixivWorkInfo.IsAI;
            bool isR18Img = pixivWorkInfo.IsR18;
            string tagName = subscribeTask.SubscribeName;
            string remindTemplate = BotConfig.SubscribeConfig.PixivTag.Template;
            string pixivTemplate = BotConfig.PixivConfig.Template;
            List<FileInfo> setuFiles = await GetSetuFilesAsync(pixivWorkInfo, groupIds);

            List<BaseContent> workMsgs = new List<BaseContent>();
            if (string.IsNullOrWhiteSpace(remindTemplate))
            {
                workMsgs.Add(new PlainContent($"pixiv标签[{tagName}]发布了新作品："));
            }
            else
            {
                workMsgs.Add(new PlainContent(pixivBusiness.getTagPushRemindMsg(remindTemplate, tagName)));
            }

            workMsgs.Add(new PlainContent(pixivBusiness.getWorkInfo(pixivWorkInfo, pixivTemplate)));

            foreach (long groupId in groupIds)
            {
                try
                {
                    if (isR18Img && groupId.IsShowR18Setu() == false) continue;
                    if (isAISetu && groupId.IsShowAISetu() == false) continue;
                    bool isShowImg = groupId.IsShowSetuImg(isR18Img);
                    List<FileInfo> imgList = isShowImg ? setuFiles : new();
                    PixivSetuContent setuContent = new PixivSetuContent(workMsgs, imgList, pixivWorkInfo);
                    await SendGroupSetuAsync(setuContent, groupId);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex, "pixiv标签订阅消息发送失败");
                }
                finally
                {
                    await Task.Delay(1000);
                }
            }
        }

        public async Task<PixivUserScanReport> HandleUserPushAsync()
        {
            SubscribeType subscribeType = SubscribeType.P站画师;
            PixivUserScanReport scanReport = new PixivUserScanReport();
            if (BotConfig.SubscribeTaskMap.ContainsKey(subscribeType) == false) return scanReport;
            List<SubscribeTask> subscribeTaskList = BotConfig.SubscribeTaskMap[subscribeType];
            if (subscribeTaskList is null || subscribeTaskList.Count == 0) return scanReport;
            foreach (SubscribeTask subscribeTask in subscribeTaskList)
            {
                try
                {
                    if (subscribeTask.SubscribeSubType != 0) continue;
                    scanReport.ScanUser++;
                    await pixivBusiness.scanAndPushUserAsync(subscribeTask, scanReport, PushUserWorkAsync);
                }
                catch (Exception ex)
                {
                    scanReport.ErrorWork++;
                    string message = $"扫描pixiv用户[{subscribeTask.SubscribeCode}]订阅失败";
                    LogHelper.Error(ex, message);
                    Reporter.SendError(ex, message);
                }
            }
            return scanReport;
        }

        public async Task<PixivUserScanReport> HandleFollowPushAsync()
        {
            PixivUserScanReport scanReport = new PixivUserScanReport();
            try
            {
                await pixivBusiness.scanAndPushFollowAsync(scanReport, PushUserWorkAsync);
            }
            catch (Exception ex)
            {
                string message = $"扫描pixiv关注用户最新作品失败";
                LogHelper.Error(ex, message);
                Reporter.SendError(ex, message);
            }
            return scanReport;
        }

        private async Task PushUserWorkAsync(PixivSubscribe pixivSubscribe, List<long> groupIds)
        {
            PixivWorkInfo pixivWorkInfo = pixivSubscribe.PixivWorkInfo;
            if (pixivWorkInfo is null || pixivWorkInfo.IsImproper || pixivWorkInfo.hasBanTag() != null) return;
            if (groupIds is null || groupIds.Count == 0) return;
            if (pixivWorkInfo.IsAI && groupIds.IsShowAISetu() == false) return;

            bool isAISetu = pixivWorkInfo.IsAI;
            bool isR18Img = pixivWorkInfo.IsR18;
            string remindTemplate = BotConfig.SubscribeConfig.PixivUser.Template;
            string pixivTemplate = BotConfig.PixivConfig.Template;
            List<FileInfo> setuFiles = await GetSetuFilesAsync(pixivWorkInfo, groupIds);

            List<BaseContent> workMsgs = new List<BaseContent>();
            if (string.IsNullOrWhiteSpace(remindTemplate))
            {
                workMsgs.Add(new PlainContent($"pixiv画师[{pixivWorkInfo.userName}]发布了新作品："));
            }
            else
            {
                workMsgs.Add(new PlainContent(pixivBusiness.getUserPushRemindMsg(remindTemplate, pixivWorkInfo.userName)));
            }

            workMsgs.Add(new PlainContent(pixivBusiness.getWorkInfo(pixivWorkInfo, pixivTemplate)));

            foreach (long groupId in groupIds)
            {
                try
                {
                    if (isR18Img && groupId.IsShowR18Setu() == false) continue;
                    if (isAISetu && groupId.IsShowAISetu() == false) continue;
                    bool isShowImg = groupId.IsShowSetuImg(isR18Img);
                    List<FileInfo> imgList = isShowImg ? setuFiles : new();
                    PixivSetuContent setuContent = new PixivSetuContent(workMsgs, imgList, pixivWorkInfo);
                    long[] msgIds = await Session.SendGroupMessageAsync(groupId, setuContent, BotConfig.PixivConfig.SendImgBehind);
                    Task recordTask = recordBusiness.AddPixivRecord(setuContent, msgIds, groupId);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex, "pixiv画师订阅消息发送失败");
                }
                finally
                {
                    await Task.Delay(1000);
                }
            }
        }

    }
}
