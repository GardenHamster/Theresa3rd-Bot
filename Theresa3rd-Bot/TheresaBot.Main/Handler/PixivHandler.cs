using SqlSugar.IOC;
using TheresaBot.Main.BotPlatform.Base.Command;
using TheresaBot.Main.Business;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Common;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Model.Cache;
using TheresaBot.Main.Model.Pixiv;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Model.Subscribe;
using TheresaBot.Main.Type;
using TheresaBot.Main.Type.StepOption;
using TheresaBot.Main.Helper;

namespace TheresaBot.Main.Handler
{
    public class PixivHandler : SetuHandler
    {
        private PixivBusiness pixivBusiness;
        private SubscribeBusiness subscribeBusiness;

        public PixivHandler()
        {
            pixivBusiness = new PixivBusiness();
            subscribeBusiness = new SubscribeBusiness();
        }

        public async Task pixivSearchAsync(GroupCommand command)
        {
            try
            {
                long memberId = args.Sender.Id;
                long groupId = args.Sender.Group.Id;
                DateTime startDateTime = DateTime.Now;
                CoolingCache.SetHanding(groupId, memberId);//请求处理中

                bool isShowAI = groupId.IsShowAISetu();
                bool isShowR18 = groupId.IsShowR18Setu();
                PixivResult<PixivWorkInfo> pixivWorkInfoDto = null;
                string keyword = command.KeyWord;
                if (await CheckSetuTagEnableAsync(session, args, keyword) == false) return;

                if (string.IsNullOrWhiteSpace(BotConfig.SetuConfig.ProcessingMsg) == false)
                {
                    await session.SendTemplateWithAtAsync(args, BotConfig.SetuConfig.ProcessingMsg, null);
                    await Task.Delay(1000);
                }

                if (StringHelper.isPureNumber(keyword))
                {
                    if (await CheckSetuCustomEnableAsync(session, args) == false) return;
                    pixivWorkInfoDto = await pixivBusiness.getPixivWorkInfoAsync(keyword);//根据作品id获取作品
                }
                else if (string.IsNullOrEmpty(keyword) && BotConfig.SetuConfig.Pixiv.RandomMode == PixivRandomMode.RandomSubscribe)
                {
                    pixivWorkInfoDto = await pixivBusiness.getRandomWorkInSubscribeAsync(groupId, isShowR18, isShowAI);//获取随机一个订阅中的画师的作品
                }
                else if (string.IsNullOrEmpty(keyword) && BotConfig.SetuConfig.Pixiv.RandomMode == PixivRandomMode.RandomFollow)
                {
                    pixivWorkInfoDto = await pixivBusiness.getRandomWorkInFollowAsync(isShowR18, isShowAI);//获取随机一个关注中的画师的作品
                }
                else if (string.IsNullOrEmpty(keyword) && BotConfig.SetuConfig.Pixiv.RandomMode == PixivRandomMode.RandomBookmark)
                {
                    pixivWorkInfoDto = await pixivBusiness.getRandomWorkInBookmarkAsync(isShowR18, isShowAI);//获取随机一个收藏中的作品
                }
                else if (string.IsNullOrEmpty(keyword))
                {
                    pixivWorkInfoDto = await pixivBusiness.getRandomWorkInTagsAsync(isShowR18, isShowAI);//获取随机一个标签中的作品
                }
                else
                {
                    if (await CheckSetuCustomEnableAsync(session, args) == false) return;
                    pixivWorkInfoDto = await pixivBusiness.getRandomWorkAsync(keyword, isShowR18, isShowAI);//获取随机一个作品
                }

                if (pixivWorkInfoDto is null || pixivWorkInfoDto.body is null)
                {
                    await session.SendTemplateWithAtAsync(args, BotConfig.SetuConfig.NotFoundMsg, " 找不到这类型的图片或者收藏比过低，换个标签试试吧~");
                    return;
                }

                PixivWorkInfo pixivWorkInfo = pixivWorkInfoDto.body;
                if (await CheckSetuSendable(session, args, pixivWorkInfo, isShowR18, isShowAI) == false) return;

                long todayLeft = GetSetuLeftToday(groupId, memberId);
                bool isShowImg = groupId.IsShowSetuImg(pixivWorkInfo.IsR18);
                List<FileInfo> setuFiles = isShowImg ? await pixivBusiness.downPixivImgsAsync(pixivWorkInfo) : null;

                string remindTemplate = BotConfig.SetuConfig.Pixiv.Template;
                string pixivTemplate = BotConfig.PixivConfig.Template;
                List<IChatMessage> workMsgs = new List<IChatMessage>();
                if (string.IsNullOrWhiteSpace(remindTemplate) == false)
                {
                    workMsgs.Add(new PlainMessage(pixivBusiness.getSetuRemindMsg(remindTemplate, todayLeft)));
                }

                if (string.IsNullOrWhiteSpace(pixivTemplate))
                {
                    workMsgs.Add(new PlainMessage(pixivBusiness.getDefaultWorkInfo(pixivWorkInfo, startDateTime)));
                }
                else
                {
                    workMsgs.Add(new PlainMessage(pixivBusiness.getWorkInfo(pixivWorkInfo, startDateTime, pixivTemplate)));
                }

                Task sendGroupTask = session.SendGroupSetuAndRevokeAsync(args, workMsgs, setuFiles, BotConfig.SetuConfig.RevokeInterval, true);
                if (BotConfig.SetuConfig.SendPrivate)
                {
                    await Task.Delay(1000);
                    Task sendTempTask = session.SendTempSetuAsync(args, workMsgs, setuFiles);
                }

                CoolingCache.SetMemberSetuCooling(args.Sender.Group.Id, memberId);//进入CD状态
            }
            catch (ApiException ex)
            {
                string errMsg = $"pixivSearchAsync异常";
                LogHelper.Error(ex, errMsg);
                await session.ReplyGroupMessageWithAtAsync(args, $"获取涩图出错了，{ex.Message}");
                ReportHelper.SendError(ex, errMsg);
            }
            catch (Exception ex)
            {
                string errMsg = $"pixivSearchAsync异常";
                LogHelper.Error(ex, errMsg);
                await session.SendTemplateWithAtAsync(args, BotConfig.SetuConfig.ErrorMsg, "获取涩图出错了，再试一次吧~");
                ReportHelper.SendError(ex, errMsg);
            }
            finally
            {
                CoolingCache.SetHandFinish(args.Sender.Group.Id, args.Sender.Id);//请求处理完成
            }
        }

        /// <summary>
        /// 订阅pixiv画师
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task subscribeUserAsync(GroupCommand command)
        {
            try
            {
                string pixivUserIds = null;
                SubscribeGroupType? groupType = null;
                long memberId = args.Sender.Id;
                long groupId = args.Sender.Group.Id;

                string[] paramArr = command.KeyWords;
                if (paramArr != null && paramArr.Length >= 2)
                {
                    pixivUserIds = paramArr.Length > 0 ? paramArr[0] : null;
                    string groupTypeStr = paramArr.Length > 1 ? paramArr[1] : null;
                    if (await CheckPixivUserIdsAsync(session, args, pixivUserIds) == false) return;
                    if (await CheckSubscribeGroupAsync(session, args, groupTypeStr) == false) return;
                    groupType = (SubscribeGroupType)Convert.ToInt32(groupTypeStr);
                }
                else
                {
                    StepInfo stepInfo = await StepCache.CreateStepAsync(session, args);
                    if (stepInfo is null) return;
                    StepDetail uidStep = new StepDetail(60, " 请在60秒内发送要订阅用户的id，多个id之间可以用逗号或者换行隔开", CheckPixivUserIdsAsync);
                    StepDetail groupStep = new StepDetail(60, $" 请在60秒内发送数字选择目标群：\r\n{EnumHelper.PixivSyncGroupOption()}", CheckSubscribeGroupAsync);
                    stepInfo.AddStep(uidStep);
                    stepInfo.AddStep(groupStep);
                    if (await stepInfo.HandleStep(session, args) == false) return;
                    pixivUserIds = uidStep.Answer;
                    groupType = (SubscribeGroupType)Convert.ToInt32(groupStep.Answer);
                }

                string[] pixivUserIdArr = pixivUserIds.splitParams();
                if (pixivUserIdArr.Length > 1)
                {
                    await session.ReplyGroupMessageWithAtAsync(args, "检测到多个id，开始批量订阅~");
                    await Task.Delay(1000);
                }

                foreach (var item in pixivUserIdArr)
                {
                    string pixivUserId = item.Trim();
                    try
                    {
                        SubscribePO dbSubscribe = subscribeBusiness.getSubscribe(pixivUserId, SubscribeType.P站画师);
                        if (dbSubscribe is null)
                        {
                            //添加订阅
                            PixivResult<PixivUserInfo> pixivUserInfoDto = await PixivHelper.GetPixivUserInfoAsync(pixivUserId);
                            dbSubscribe = subscribeBusiness.insertSurscribe(pixivUserInfoDto, pixivUserId);
                        }

                        long subscribeGroupId = groupType == SubscribeGroupType.All ? 0 : args.Sender.Group.Id;
                        if (subscribeBusiness.isExistsSubscribeGroup(subscribeGroupId, dbSubscribe.Id))
                        {
                            //关联订阅
                            await session.ReplyGroupMessageWithAtAsync(args, $"画师id[{pixivUserId}]已经被订阅了~");
                            continue;
                        }

                        SubscribeGroupPO subscribeGroup = subscribeBusiness.insertSubscribeGroup(subscribeGroupId, dbSubscribe.Id);
                        await session.ReplyGroupMessageWithAtAsync(args, $"画师id[{dbSubscribe.SubscribeCode}]订阅成功，正在读取最新作品~");

                        await Task.Delay(1000);
                        await sendPixivUserNewestWorkAsync(session, args, dbSubscribe, groupId.IsShowR18Setu(), groupId.IsShowAISetu());
                    }
                    catch (Exception ex)
                    {
                        string errMsg = $"pixiv画师[{pixivUserId}]订阅失败";
                        LogHelper.Error(ex, errMsg);
                        await session.ReplyGroupMessageWithAtAsync(args, errMsg);
                        ReportHelper.SendErrorForce(ex, errMsg);
                    }
                    finally
                    {
                        Thread.Sleep(2000);
                    }
                }
                if (pixivUserIdArr.Length > 1)
                {
                    await session.ReplyGroupMessageWithAtAsync(args, $"所有画师订阅完毕");
                }
                ConfigHelper.loadSubscribeTask();
            }
            catch (Exception ex)
            {
                string errMsg = $"订阅pixiv用户失败";
                LogHelper.Error(ex, errMsg);
                await session.ReplyGroupMessageWithAtAsync(args, errMsg);
                ReportHelper.SendError(ex, errMsg);
            }
        }


        /// <summary>
        /// 订阅pixiv关注画师列表
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task subscribeFollowUserAsync(GroupCommand command)
        {
            try
            {
                long memberId = args.Sender.Id;
                long groupId = args.Sender.Group.Id;

                StepInfo stepInfo = await StepCache.CreateStepAsync(session, args);
                if (stepInfo is null) return;

                StepDetail modeStep = new StepDetail(60, $" 请在60秒内发送数字选择模式：\r\n{EnumHelper.PixivSyncModeOption()}", CheckSyncModeAsync);
                StepDetail groupStep = new StepDetail(60, $" 请在60秒内发送数字选择目标群：\r\n{EnumHelper.PixivSyncGroupOption()}", CheckSubscribeGroupAsync);
                stepInfo.AddStep(modeStep);
                stepInfo.AddStep(groupStep);
                if (await stepInfo.HandleStep(session, args) == false) return;

                PixivSyncModeType syncMode = (PixivSyncModeType)Convert.ToInt32(modeStep.Answer);
                SubscribeGroupType syncGroup = (SubscribeGroupType)Convert.ToInt32(groupStep.Answer);

                await session.ReplyGroupMessageWithAtAsync(args, "正在获取pixiv账号中已关注的画师列表...");
                await Task.Delay(1000);

                List<PixivFollowUser> followUserList = await pixivBusiness.getFollowUserList();
                if (followUserList is null || followUserList.Count == 0)
                {
                    await session.ReplyGroupMessageWithAtAsync(args, "pixiv账号还没有关注任何画师");
                    return;
                }

                await session.ReplyGroupMessageWithAtAsync(args, $"已获取{followUserList.Count}个画师，正在录入数据...");
                await Task.Delay(1000);

                //插入Subscribe数据
                DateTime syncDate = DateTime.Now;
                DbScoped.SugarScope.BeginTran();//开始事务
                List<SubscribePO> dbSubscribeList = new List<SubscribePO>();

                foreach (var item in followUserList)
                {
                    SubscribePO dbSubscribe = subscribeBusiness.getSubscribe(item.userId, SubscribeType.P站画师);
                    if (dbSubscribe is null) dbSubscribe = subscribeBusiness.insertSurscribe(item, syncDate);
                    dbSubscribeList.Add(dbSubscribe);
                }

                long subscribeGroupId = syncGroup == SubscribeGroupType.All ? 0 : groupId;
                if (syncMode == PixivSyncModeType.Overwrite)
                {
                    List<SubscribePO> subscribeList = subscribeBusiness.getSubscribes(SubscribeType.P站画师);
                    foreach (var item in subscribeList) subscribeBusiness.delSubscribeGroup(item.Id);//覆盖情况下,删除所有这个订阅的数据
                    foreach (var item in dbSubscribeList) subscribeBusiness.insertSubscribeGroup(subscribeGroupId, item.Id);
                }
                else
                {
                    foreach (var item in dbSubscribeList)
                    {
                        SubscribeGroupPO subscribeGroup = subscribeBusiness.getSubscribeGroup(subscribeGroupId, item.Id);
                        if (subscribeGroup is null) subscribeBusiness.insertSubscribeGroup(subscribeGroupId, item.Id);
                    }
                }
                DbScoped.SugarScope.CommitTran();//提交事务
                await session.ReplyGroupMessageWithAtAsync(args, "订阅pixiv关注画师列表完毕");
                ConfigHelper.loadSubscribeTask();
            }
            catch (Exception ex)
            {
                string errMsg = $"订阅pixiv关注画师列表失败";
                LogHelper.Error(ex, errMsg);
                DbScoped.SugarScope.RollbackTran();//事务回滚
                await session.ReplyGroupMessageWithAtAsync(args, errMsg);
                ReportHelper.SendError(ex, errMsg);
            }
        }


        /// <summary>
        /// 取消订阅pixiv画师
        /// </summary>
        /// <param name="e"></param>
        /// <param name="message"></param>
        /// <param name="isGroupSubscribe"></param>
        /// <returns></returns>
        public async Task cancleSubscribeUserAsync(GroupCommand command)
        {
            try
            {
                string pixivUserIds = null;
                long memberId = args.Sender.Id;
                long groupId = args.Sender.Group.Id;

                string paramStr = command.KeyWord;
                if (string.IsNullOrWhiteSpace(paramStr))
                {
                    StepInfo stepInfo = await StepCache.CreateStepAsync(session, args);
                    if (stepInfo is null) return;
                    StepDetail uidStep = new StepDetail(60, " 请在60秒内发送要退订用户的id，多个id之间可以用逗号或者换行隔开", CheckPixivUserIdsAsync);
                    stepInfo.AddStep(uidStep);
                    if (await stepInfo.HandleStep(session, args) == false) return;
                    pixivUserIds = uidStep.Answer;
                }
                else
                {
                    pixivUserIds = paramStr.Trim();
                    if (await CheckPixivUserIdsAsync(session, args, pixivUserIds) == false) return;
                }

                string[] pixivUserIdArr = pixivUserIds.splitParams();
                foreach (string pixivUserId in pixivUserIdArr)
                {
                    SubscribePO dbSubscribe = subscribeBusiness.getSubscribe(pixivUserId, SubscribeType.P站画师);
                    if (dbSubscribe is null)
                    {
                        await session.ReplyGroupMessageWithAtAsync(args, $"退订失败，userId={pixivUserId}的订阅不存在");
                        return;
                    }
                    subscribeBusiness.delSubscribeGroup(dbSubscribe.Id);
                }

                await session.ReplyGroupMessageWithAtAsync(args, $"已为所有群退订了pixiv用户[{pixivUserIds}]~");
                ConfigHelper.loadSubscribeTask();
            }
            catch (Exception ex)
            {
                string errMsg = $"取消pixiv画师订阅失败";
                LogHelper.Error(ex, errMsg);
                await session.ReplyGroupMessageWithAtAsync(args, errMsg);
                ReportHelper.SendError(ex, errMsg);
            }
        }

        /// <summary>
        /// 订阅pixiv标签
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task subscribeTagAsync(GroupCommand command)
        {
            try
            {
                string pixivTags = null;
                SubscribeGroupType? groupType = null;
                long memberId = args.Sender.Id;
                long groupId = args.Sender.Group.Id;

                string[] paramArr = command.KeyWords;
                if (paramArr != null && paramArr.Length >= 2)
                {
                    pixivTags = paramArr.Length > 0 ? paramArr[0] : null;
                    string groupTypeStr = paramArr.Length > 1 ? paramArr[1] : null;
                    if (await CheckPixivTagAsync(session, args, pixivTags) == false) return;
                    if (await CheckSubscribeGroupAsync(session, args, groupTypeStr) == false) return;
                    groupType = (SubscribeGroupType)Convert.ToInt32(groupTypeStr);
                }
                else
                {
                    StepInfo stepInfo = await StepCache.CreateStepAsync(session, args);
                    if (stepInfo is null) return;
                    StepDetail tagStep = new StepDetail(60, $"请在60秒内发送要订阅的标签名", CheckPixivTagAsync);
                    StepDetail groupStep = new StepDetail(60, $"请在60秒内发送数字选择目标群：\r\n{EnumHelper.PixivSyncGroupOption()}", CheckSubscribeGroupAsync);
                    stepInfo.AddStep(tagStep);
                    stepInfo.AddStep(groupStep);
                    if (await stepInfo.HandleStep(session, args) == false) return;
                    pixivTags = tagStep.Answer;
                    groupType = (SubscribeGroupType)Convert.ToInt32(groupStep.Answer);
                }

                string searchWord = pixivBusiness.toPixivSearchWord(pixivTags);
                PixivResult<PixivSearch> pageOne = await PixivHelper.GetPixivSearchAsync(searchWord, 1, false, groupId.IsShowR18Setu());
                if (pageOne is null || pageOne.body.getIllust().data.Count == 0)
                {
                    await session.ReplyGroupMessageWithAtAsync(args, "该标签中没有任何作品，订阅失败");
                    return;
                }

                SubscribePO dbSubscribe = subscribeBusiness.getSubscribe(pixivTags, SubscribeType.P站标签);
                if (dbSubscribe is null) dbSubscribe = subscribeBusiness.insertSurscribe(pixivTags);

                long subscribeGroupId = groupType == SubscribeGroupType.All ? 0 : groupId;
                if (subscribeBusiness.isExistsSubscribeGroup(subscribeGroupId, dbSubscribe.Id))
                {
                    //关联订阅
                    await session.ReplyGroupMessageWithAtAsync(args, $"这个标签已经被订阅了~");
                    return;
                }

                SubscribeGroupPO subscribeGroup = subscribeBusiness.insertSubscribeGroup(subscribeGroupId, dbSubscribe.Id);
                await session.ReplyGroupMessageWithAtAsync(args, $"标签[{pixivTags}]订阅成功,该标签总作品数为:{pageOne.body.illust.total}");
                ConfigHelper.loadSubscribeTask();
            }
            catch (Exception ex)
            {
                string errMsg = $"订阅pixiv标签失败";
                LogHelper.Error(ex, errMsg);
                await session.ReplyGroupMessageWithAtAsync(args, errMsg);
                ReportHelper.SendError(ex, errMsg);
            }
        }

        /// <summary>
        /// 取消订阅pixiv标签
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task cancleSubscribeTagAsync(GroupCommand command)
        {
            try
            {
                string pixivTag = null;
                long memberId = args.Sender.Id;
                long groupId = args.Sender.Group.Id;

                string paramStr = command.KeyWord;
                if (string.IsNullOrWhiteSpace(paramStr))
                {
                    StepInfo stepInfo = await StepCache.CreateStepAsync(session, args);
                    if (stepInfo is null) return;
                    StepDetail tagStep = new StepDetail(60, "请在60秒内发送要退订的标签名", CheckPixivTagAsync);
                    stepInfo.AddStep(tagStep);
                    if (await stepInfo.HandleStep(session, args) == false) return;
                    pixivTag = tagStep.Answer;
                }
                else
                {
                    pixivTag = paramStr.Trim();
                    if (await CheckPixivTagAsync(session, args, pixivTag) == false) return;
                }

                SubscribePO dbSubscribe = subscribeBusiness.getSubscribe(pixivTag, SubscribeType.P站标签);
                if (dbSubscribe is null)
                {
                    await session.ReplyGroupMessageWithAtAsync(args, "退订失败，标签为[{pixivTag}]的订阅不存在");
                    return;
                }

                subscribeBusiness.delSubscribeGroup(dbSubscribe.Id);
                await session.ReplyGroupMessageWithAtAsync(args, $" 已为所有群退订了pixiv标签[{pixivTag}]~");
                ConfigHelper.loadSubscribeTask();
            }
            catch (Exception ex)
            {
                string errMsg = $"退订pixiv标签失败";
                LogHelper.Error(ex, errMsg);
                await session.ReplyGroupMessageWithAtAsync(args, errMsg);
                ReportHelper.SendError(ex, errMsg);
            }
        }


        public async Task<PixivTagScanReport> HandleTagSubscribeAsync()
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
                    await sendGroupSubscribeAsync(subscribeTask, pixivSubscribeList);
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

        private async Task sendGroupSubscribeAsync(SubscribeTask subscribeTask, List<PixivSubscribe> pixivSubscribeList)
        {
            foreach (PixivSubscribe pixivSubscribe in pixivSubscribeList)
            {
                DateTime startTime = DateTime.Now;
                List<long> groupIds = subscribeTask.GroupIdList;
                PixivWorkInfo pixivWorkInfo = pixivSubscribe.PixivWorkInfo;
                if (pixivWorkInfo is null || pixivWorkInfo.IsImproper || pixivWorkInfo.hasBanTag() != null) continue;
                if (groupIds is null || groupIds.Count == 0) continue;
                if (pixivWorkInfo.IsAI && groupIds.IsShowAISetu() == false) continue;

                bool isAISetu = pixivWorkInfo.IsAI;
                bool isR18Img = pixivWorkInfo.IsR18;
                bool isDownImg = groupIds.IsDownSetuImg(isR18Img);
                string tagName = subscribeTask.SubscribeName;
                string remindTemplate = BotConfig.SubscribeConfig.PixivTag.Template;
                string pixivTemplate = BotConfig.PixivConfig.Template;
                List<FileInfo> setuFiles = isDownImg ? await pixivBusiness.downPixivImgsAsync(pixivWorkInfo) : null;

                List<IChatMessage> workMsgs = new List<IChatMessage>();
                if (string.IsNullOrWhiteSpace(remindTemplate))
                {
                    workMsgs.Add(new PlainMessage($"pixiv标签[{tagName}]发布了新作品："));
                }
                else
                {
                    workMsgs.Add(new PlainMessage(pixivBusiness.getTagPushRemindMsg(remindTemplate, tagName)));
                }

                if (string.IsNullOrWhiteSpace(pixivTemplate))
                {
                    workMsgs.Add(new PlainMessage(pixivBusiness.getDefaultWorkInfo(pixivWorkInfo, startTime)));
                }
                else
                {
                    workMsgs.Add(new PlainMessage(pixivBusiness.getWorkInfo(pixivWorkInfo, startTime, pixivTemplate)));
                }

                foreach (long groupId in groupIds)
                {
                    try
                    {
                        if (isR18Img && groupId.IsShowR18Setu() == false) continue;
                        if (isAISetu && groupId.IsShowAISetu() == false) continue;
                        bool isShowImg = groupId.IsShowSetuImg(isR18Img);
                        await MiraiHelper.Session.SendGroupSetuAsync(workMsgs, setuFiles, groupId, isShowImg);
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


        public async Task<PixivUserScanReport> HandleUserSubscribeAsync()
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
                    DateTime startTime = DateTime.Now;
                    scanReport.ScanUser++;
                    List<PixivSubscribe> pixivSubscribeList = await pixivBusiness.getPixivUserSubscribeAsync(subscribeTask, scanReport);
                    if (pixivSubscribeList is null || pixivSubscribeList.Count == 0) continue;
                    await sendGroupSubscribeAsync(pixivSubscribeList, subscribeTask.GroupIdList, startTime);
                }
                catch (Exception ex)
                {
                    scanReport.ErrorWork++;
                    string message = $"扫描pixiv用户[{subscribeTask.SubscribeCode}]订阅失败";
                    LogHelper.Error(ex, message);
                    ReportHelper.SendError(ex, message);
                }
                finally
                {
                    await Task.Delay(2000);
                }
            }
            return scanReport;
        }


        public async Task<PixivUserScanReport> HandleFollowSubscribeAsync()
        {
            DateTime startTime = DateTime.Now;
            PixivUserScanReport scanReport = new PixivUserScanReport();

            try
            {
                List<PixivSubscribe> pixivFollowLatestList = await pixivBusiness.getPixivFollowLatestAsync(scanReport);
                if (pixivFollowLatestList is null || pixivFollowLatestList.Count == 0) return scanReport;
                await sendGroupSubscribeAsync(pixivFollowLatestList, BotConfig.PermissionsConfig.SubscribeGroups, startTime);
            }
            catch (Exception ex)
            {
                string message = $"扫描pixiv关注用户最新作品失败";
                LogHelper.Error(ex, message);
                ReportHelper.SendError(ex, message);
            }
            return scanReport;
        }


        private async Task sendGroupSubscribeAsync(List<PixivSubscribe> pixivSubscribeList, List<long> groupIds, DateTime startTime)
        {
            foreach (PixivSubscribe pixivSubscribe in pixivSubscribeList)
            {
                PixivWorkInfo pixivWorkInfo = pixivSubscribe.PixivWorkInfo;
                if (pixivWorkInfo is null || pixivWorkInfo.IsImproper || pixivWorkInfo.hasBanTag() != null) continue;
                if (groupIds is null || groupIds.Count == 0) continue;
                if (pixivWorkInfo.IsAI && groupIds.IsShowAISetu() == false) continue;

                bool isAISetu = pixivWorkInfo.IsAI;
                bool isR18Img = pixivWorkInfo.IsR18;
                bool isDownImg = groupIds.IsDownSetuImg(isR18Img);
                string remindTemplate = BotConfig.SubscribeConfig.PixivUser.Template;
                string pixivTemplate = BotConfig.PixivConfig.Template;
                List<FileInfo> setuFiles = isDownImg ? await pixivBusiness.downPixivImgsAsync(pixivWorkInfo) : null;

                List<IChatMessage> workMsgs = new List<IChatMessage>();
                if (string.IsNullOrWhiteSpace(remindTemplate))
                {
                    workMsgs.Add(new PlainMessage($"pixiv画师[{pixivWorkInfo.userName}]发布了新作品："));
                }
                else
                {
                    workMsgs.Add(new PlainMessage(pixivBusiness.getUserPushRemindMsg(remindTemplate, pixivWorkInfo.userName)));
                }

                if (string.IsNullOrWhiteSpace(pixivTemplate))
                {
                    workMsgs.Add(new PlainMessage(pixivBusiness.getDefaultWorkInfo(pixivWorkInfo, startTime)));
                }
                else
                {
                    workMsgs.Add(new PlainMessage(pixivBusiness.getWorkInfo(pixivWorkInfo, startTime, pixivTemplate)));
                }

                foreach (long groupId in groupIds)
                {
                    try
                    {
                        if (isR18Img && groupId.IsShowR18Setu() == false) continue;
                        if (isAISetu && groupId.IsShowAISetu() == false) continue;
                        bool isShowImg = groupId.IsShowSetuImg(isR18Img);
                        await MiraiHelper.Session.SendGroupSetuAsync(workMsgs, setuFiles, groupId, isShowImg);
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

        /// <summary>
        /// 发送画师的最新作品
        /// </summary>
        /// <param name="session"></param>
        /// <param name="userId"></param>
        /// <param name="subscribeId"></param>
        /// <returns></returns>
        private async Task sendPixivUserNewestWorkAsync(GroupCommand command, SubscribePO dbSubscribe, bool isShowR18, bool isShowAI)
        {
            try
            {
                DateTime startTime = DateTime.Now;
                long groupId = args.Sender.Group.Id;
                List<PixivSubscribe> pixivSubscribeList = await pixivBusiness.getPixivUserNewestAsync(dbSubscribe.SubscribeCode, dbSubscribe.Id, 1);
                if (pixivSubscribeList is null || pixivSubscribeList.Count == 0)
                {
                    await session.SendGroupMessageAsync(args, $"画师[{dbSubscribe.SubscribeName}]还没有发布任何作品~");
                    return;
                }

                PixivSubscribe pixivSubscribe = pixivSubscribeList.First();
                PixivWorkInfo pixivWorkInfo = pixivSubscribe.PixivWorkInfo;
                if (await CheckSetuSendable(session, args, pixivWorkInfo, isShowR18, isShowAI) == false) return;

                List<IChatMessage> workMsgs = new List<IChatMessage>();
                bool isShowImg = groupId.IsShowSetuImg(pixivWorkInfo.IsR18);
                List<FileInfo> setuFiles = isShowImg ? await pixivBusiness.downPixivImgsAsync(pixivWorkInfo) : null;
                workMsgs.Add(new PlainMessage($"pixiv画师[{pixivWorkInfo.userName}]的最新作品："));
                workMsgs.Add(new PlainMessage(pixivBusiness.getDefaultWorkInfo(pixivWorkInfo, startTime)));
                await session.SendGroupSetuAsync(workMsgs, setuFiles, groupId, isShowImg);
            }
            catch (Exception ex)
            {
                string errMsg = $"读取画师[{dbSubscribe.SubscribeName}]的最新作品失败";
                LogHelper.Error(ex, errMsg);
                await session.SendGroupMessageAsync(args, errMsg);
                ReportHelper.SendError(ex, errMsg);
            }
        }

        private async Task<bool> CheckPixivTagAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                await session.ReplyGroupMessageWithAtAsync(args, "标签不可以为空");
                return false;
            }
            return true;
        }

        private async Task<bool> CheckPixivUserIdsAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                await session.ReplyGroupMessageWithAtAsync(args, "用户id不可以为空");
                return false;
            }

            string[] pixivUserIdArr = value.splitParams();
            if (pixivUserIdArr.Length == 0)
            {
                await session.ReplyGroupMessageWithAtAsync(args, "没有检测到用户id");
                return false;
            }

            foreach (var userIdStr in pixivUserIdArr)
            {
                long userId = 0;
                if (long.TryParse(userIdStr, out userId) == false)
                {
                    await session.ReplyGroupMessageWithAtAsync(args, $"用户id{userIdStr}必须为数字");
                    return false;
                }
                if (userId <= 0)
                {
                    await session.ReplyGroupMessageWithAtAsync(args, $"用户id{userIdStr}无效");
                    return false;
                }
            }
            return true;
        }


        private async Task<bool> CheckSyncModeAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string value)
        {
            int modeId = 0;
            if (int.TryParse(value, out modeId) == false)
            {
                await session.ReplyGroupMessageWithAtAsync(args, "mode必须为数字");
                return false;
            }
            if (Enum.IsDefined(typeof(PixivSyncModeType), modeId) == false)
            {
                await session.ReplyGroupMessageWithAtAsync(args, "mode不在范围内");
                return false;
            }
            return true;
        }

        private async Task<bool> CheckSubscribeGroupAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string value)
        {
            int typeId = 0;
            if (int.TryParse(value, out typeId) == false)
            {
                await session.ReplyGroupMessageWithAtAsync(args, "target必须为数字");
                return false;
            }
            if (Enum.IsDefined(typeof(SubscribeGroupType), typeId) == false)
            {
                await session.ReplyGroupMessageWithAtAsync(args, "target不在范围内");
                return false;
            }
            return true;
        }


    }
}
