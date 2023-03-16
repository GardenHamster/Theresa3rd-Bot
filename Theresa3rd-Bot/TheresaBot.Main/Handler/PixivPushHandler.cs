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
            int maxScan = BotConfig.SubscribeConfig.PixivTag.MaxScan;
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
                    List<PixivSubscribe> pixivSubscribeList = await pixivBusiness.getPixivTagSubscribeAsync(subscribeTask, scanReport, maxScan);
                    if (pixivSubscribeList is null || pixivSubscribeList.Count == 0) continue;
                    await sendTagPushAsync(subscribeTask, pixivSubscribeList);
                }
                catch (Exception ex)
                {
                    scanReport.ErrorTag++;
                    LogHelper.Error(ex, $"扫描pixiv标签[{subscribeTask.SubscribeCode}]订阅失败");
                }
                finally
                {
                    await Task.Delay(2000);
                }
            }
            return scanReport;
        }


        private async Task sendTagPushAsync(SubscribeTask subscribeTask, List<PixivSubscribe> pixivSubscribeList)
        {
            foreach (PixivSubscribe pixivSubscribe in pixivSubscribeList)
            {
                List<long> groupIds = subscribeTask.GroupIdList;
                PixivWorkInfo pixivWorkInfo = pixivSubscribe.PixivWorkInfo;
                if (pixivWorkInfo is null || pixivWorkInfo.IsImproper || pixivWorkInfo.hasBanTag() != null) continue;
                if (groupIds is null || groupIds.Count == 0) continue;
                if (pixivWorkInfo.IsAI && groupIds.IsShowAISetu() == false) continue;

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
                        SetuContent setuContent = new SetuContent(workMsgs, imgList);
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
                    List<PixivSubscribe> pixivSubscribeList = await pixivBusiness.getPixivUserSubscribeAsync(subscribeTask, scanReport);
                    if (pixivSubscribeList is null || pixivSubscribeList.Count == 0) continue;
                    await sendUserPushAsync(pixivSubscribeList, subscribeTask.GroupIdList);
                }
                catch (Exception ex)
                {
                    scanReport.ErrorWork++;
                    string message = $"扫描pixiv用户[{subscribeTask.SubscribeCode}]订阅失败";
                    LogHelper.Error(ex, message);
                    Reporter.SendError(ex, message);
                }
                finally
                {
                    await Task.Delay(2000);
                }
            }
            return scanReport;
        }


        public async Task<PixivUserScanReport> HandleFollowPushAsync()
        {
            PixivUserScanReport scanReport = new PixivUserScanReport();

            try
            {
                List<PixivSubscribe> pixivFollowLatestList = await pixivBusiness.getPixivFollowLatestAsync(scanReport);
                if (pixivFollowLatestList is null || pixivFollowLatestList.Count == 0) return scanReport;
                await sendUserPushAsync(pixivFollowLatestList, BotConfig.PermissionsConfig.SubscribeGroups);
            }
            catch (Exception ex)
            {
                string message = $"扫描pixiv关注用户最新作品失败";
                LogHelper.Error(ex, message);
                Reporter.SendError(ex, message);
            }
            return scanReport;
        }


        private async Task sendUserPushAsync(List<PixivSubscribe> pixivSubscribeList, List<long> groupIds)
        {
            foreach (PixivSubscribe pixivSubscribe in pixivSubscribeList)
            {
                PixivWorkInfo pixivWorkInfo = pixivSubscribe.PixivWorkInfo;
                if (pixivWorkInfo is null || pixivWorkInfo.IsImproper || pixivWorkInfo.hasBanTag() != null) continue;
                if (groupIds is null || groupIds.Count == 0) continue;
                if (pixivWorkInfo.IsAI && groupIds.IsShowAISetu() == false) continue;

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
                        SetuContent setuContent = new SetuContent(workMsgs, imgList);
                        await Session.SendGroupMessageAsync(groupId, setuContent, BotConfig.PixivConfig.SendImgBehind);
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
}
