using SqlSugar.IOC;
using TheresaBot.Main.Business;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Pixiv;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Model.Step;
using TheresaBot.Main.Model.Subscribe;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;
using TheresaBot.Main.Type.StepOption;

namespace TheresaBot.Main.Handler
{
    internal class PixivSubscribeHandler : SetuHandler
    {
        private PixivBusiness pixivBusiness;
        private SubscribeBusiness subscribeBusiness;

        public PixivSubscribeHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            pixivBusiness = new PixivBusiness();
            subscribeBusiness = new SubscribeBusiness();
        }

        /// <summary>
        /// 订阅pixiv画师
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SubscribeUserAsync(GroupCommand command)
        {
            try
            {
                var userIds = new string[0];
                var pushType = GroupPushType.CurrentGroup;
                if (command.Params.Length >= 2)
                {
                    userIds = await CheckUserIdsAsync(command.Params[0]);
                    pushType = await CheckPushTypeAsync(command.Params[1]);
                }
                else
                {
                    ProcessInfo processInfo = ProcessCache.CreateProcessAsync(command);
                    StepInfo uidStep = processInfo.CreateStep("请在60秒内发送要订阅用户的id，多个id之间可以用逗号或者换行隔开", CheckUserIdsAsync);
                    StepInfo groupStep = processInfo.CreateStep($"请在60秒内发送数字选择目标群：\r\n{EnumHelper.PixivSyncGroupOption()}", CheckPushTypeAsync);
                    await processInfo.StartProcessing();
                    userIds = uidStep.Answer.splitParams();
                    pushType = uidStep.AnswerForEnum<GroupPushType>();
                }

                if (userIds.Length > 1)
                {
                    await command.ReplyGroupMessageWithAtAsync("检测到多个id，开始批量订阅~");
                    await Task.Delay(1000);
                }

                foreach (var userId in userIds)
                {
                    await SubscribeUserAsync(command, userId, pushType);
                    await Task.Delay(1000);
                }
                SubscribeDatas.LoadSubscribeTask();
            }
            catch (ProcessException ex)
            {
                await command.ReplyGroupMessageWithAtAsync(ex.RemindMessage);
            }
            catch (Exception ex)
            {
                await LogAndReportError(command, ex, $"pixiv用户订阅异常");
            }
        }

        /// <summary>
        /// 订阅pixiv画师
        /// </summary>
        /// <param name="command"></param>
        /// <param name="userId"></param>
        /// <param name="pushType"></param>
        /// <returns></returns>
        private async Task SubscribeUserAsync(GroupCommand command, string userId, GroupPushType pushType)
        {
            try
            {
                SubscribePO dbSubscribe = subscribeBusiness.getSubscribe(userId, SubscribeType.P站画师);
                if (dbSubscribe is null)
                {
                    //添加订阅
                    PixivUserProfileTop pixivUserInfo = await PixivHelper.GetPixivUserProfileTopAsync(userId);
                    dbSubscribe = subscribeBusiness.insertSurscribe(pixivUserInfo, userId);
                }

                long subscribeGroupId = pushType == GroupPushType.AllGroup ? 0 : command.GroupId;
                if (subscribeBusiness.isExistsSubscribeGroup(subscribeGroupId, dbSubscribe.Id))
                {
                    //关联订阅
                    await command.ReplyGroupMessageWithQuoteAsync($"画师id[{userId}]已经被订阅了~");
                    return;
                }

                SubscribeGroupPO subscribeGroup = subscribeBusiness.insertSubscribeGroup(subscribeGroupId, dbSubscribe.Id);
                await command.ReplyGroupMessageWithQuoteAsync($"画师id[{dbSubscribe.SubscribeCode}]订阅成功，正在读取最新作品~");

                await Task.Delay(1000);
                await sendUserNewestAsync(command, dbSubscribe, command.GroupId.IsShowR18Setu(), command.GroupId.IsShowAISetu());
            }
            catch (Exception ex)
            {
                await LogAndReportError(command, ex, $"pixiv画师[{userId}]订阅失败");
            }
            finally
            {
                await Task.Delay(2000);
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
                ProcessInfo stepInfo = await ProcessCache.CreateProcessAsync(command);
                if (stepInfo is null) return;

                StepInfo modeStep = new StepDetail(60, $"请在60秒内发送数字选择模式：\r\n{EnumHelper.PixivSyncModeOption()}", CheckSyncModeAsync);
                StepInfo groupStep = new StepDetail(60, $"请在60秒内发送数字选择目标群：\r\n{EnumHelper.PixivSyncGroupOption()}", CheckGroupTypeAsync);
                stepInfo.AddStep(modeStep);
                stepInfo.AddStep(groupStep);
                if (await stepInfo.StartProcessing() == false) return;

                PixivSyncType syncMode = (PixivSyncType)Convert.ToInt32(modeStep.Answer);
                GroupPushType syncGroup = (GroupPushType)Convert.ToInt32(groupStep.Answer);

                await command.ReplyGroupMessageWithAtAsync("正在获取pixiv账号中已关注的画师列表...");
                await Task.Delay(1000);

                List<PixivFollowUser> followUserList = await pixivBusiness.getFollowUserList();
                if (followUserList is null || followUserList.Count == 0)
                {
                    await command.ReplyGroupMessageWithAtAsync("pixiv账号还没有关注任何画师");
                    return;
                }

                await command.ReplyGroupMessageWithAtAsync($"已获取{followUserList.Count}个画师，正在录入数据...");
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

                long subscribeGroupId = syncGroup == GroupPushType.AllGroup ? 0 : command.GroupId;
                if (syncMode == PixivSyncType.Overwrite)
                {
                    List<SubscribePO> subscribeList = subscribeBusiness.getSubscribes(SubscribeType.P站画师);
                    foreach (var item in subscribeList) subscribeBusiness.cancleSubscribe(item.Id);//覆盖情况下,删除所有这个订阅的数据
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
                await command.ReplyGroupMessageWithAtAsync("订阅pixiv关注画师列表完毕");
                SubscribeDatas.LoadSubscribeTask();
            }
            catch (Exception ex)
            {
                string errMsg = $"订阅pixiv关注画师列表失败";
                LogHelper.Error(ex, errMsg);
                DbScoped.SugarScope.RollbackTran();//事务回滚
                await command.ReplyError(ex);
                await Task.Delay(1000);
                await Reporter.SendError(ex, errMsg);
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
                string pixivUserIds;
                string paramStr = command.KeyWord;
                if (string.IsNullOrWhiteSpace(paramStr))
                {
                    ProcessInfo stepInfo = await ProcessCache.CreateProcessAsync(command);
                    if (stepInfo is null) return;
                    StepInfo uidStep = new StepDetail(60, "请在60秒内发送要退订用户的id，多个id之间可以用逗号或者换行隔开", CheckPixivUserIdsAsync);
                    stepInfo.AddStep(uidStep);
                    if (await stepInfo.StartProcessing() == false) return;
                    pixivUserIds = uidStep.Answer;
                }
                else
                {
                    pixivUserIds = paramStr.Trim();
                    if (await CheckUserIdsAsync(command, pixivUserIds) == false) return;
                }

                string[] pixivUserIdArr = pixivUserIds.splitParams();
                foreach (string pixivUserId in pixivUserIdArr)
                {
                    SubscribePO dbSubscribe = subscribeBusiness.getSubscribe(pixivUserId, SubscribeType.P站画师);
                    if (dbSubscribe is null)
                    {
                        await command.ReplyGroupMessageWithAtAsync($"退订失败，userId={pixivUserId}的订阅不存在");
                        return;
                    }
                    subscribeBusiness.cancleSubscribe(dbSubscribe.Id);
                }

                await command.ReplyGroupMessageWithAtAsync($"已为所有群退订了pixiv用户[{pixivUserIds}]~");
                SubscribeDatas.LoadSubscribeTask();
            }
            catch (Exception ex)
            {
                string errMsg = $"取消pixiv画师订阅失败";
                LogHelper.Error(ex, errMsg);
                await command.ReplyError(ex);
                await Task.Delay(1000);
                await Reporter.SendError(ex, errMsg);
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
                string pixivTags;
                GroupPushType? groupType;

                string[] paramArr = command.Params;
                if (paramArr != null && paramArr.Length >= 2)
                {
                    pixivTags = paramArr.Length > 0 ? paramArr[0] : string.Empty;
                    string groupTypeStr = paramArr.Length > 1 ? paramArr[1] : string.Empty;
                    if (await CheckPixivTagAsync(command, pixivTags) == false) return;
                    if (await CheckPushTypeAsync(command, groupTypeStr) == false) return;
                    groupType = (GroupPushType)Convert.ToInt32(groupTypeStr);
                }
                else
                {
                    ProcessInfo stepInfo = await ProcessCache.CreateProcessAsync(command);
                    if (stepInfo is null) return;
                    StepInfo tagStep = new StepDetail(60, $"请在60秒内发送要订阅的标签名", CheckPixivTagAsync);
                    StepInfo groupStep = new StepDetail(60, $"请在60秒内发送数字选择目标群：\r\n{EnumHelper.PixivSyncGroupOption()}", CheckGroupTypeAsync);
                    stepInfo.AddStep(tagStep);
                    stepInfo.AddStep(groupStep);
                    if (await stepInfo.StartProcessing() == false) return;
                    pixivTags = tagStep.Answer;
                    groupType = (GroupPushType)Convert.ToInt32(groupStep.Answer);
                }

                string searchWord = pixivBusiness.toPixivSearchWords(pixivTags.ToActualPixivTags());
                PixivSearch pageOne = await PixivHelper.GetPixivSearchAsync(searchWord, 1, false, command.GroupId.IsShowR18Setu());
                if (pageOne is null || pageOne.getIllust().data.Count == 0)
                {
                    await command.ReplyGroupMessageWithAtAsync("该标签中没有任何作品，订阅失败");
                    return;
                }

                SubscribePO dbSubscribe = subscribeBusiness.getSubscribe(pixivTags, SubscribeType.P站标签);
                if (dbSubscribe is null) dbSubscribe = subscribeBusiness.insertSurscribe(pixivTags);

                long subscribeGroupId = groupType == GroupPushType.AllGroup ? 0 : command.GroupId;
                if (subscribeBusiness.isExistsSubscribeGroup(subscribeGroupId, dbSubscribe.Id))
                {
                    //关联订阅
                    await command.ReplyGroupMessageWithAtAsync($"这个标签已经被订阅了~");
                    return;
                }

                SubscribeGroupPO subscribeGroup = subscribeBusiness.insertSubscribeGroup(subscribeGroupId, dbSubscribe.Id);
                await command.ReplyGroupMessageWithAtAsync($"标签[{pixivTags}]订阅成功,该标签总作品数为:{pageOne.illust.total}");
                SubscribeDatas.LoadSubscribeTask();
            }
            catch (Exception ex)
            {
                string errMsg = $"订阅pixiv标签失败";
                LogHelper.Error(ex, errMsg);
                await command.ReplyError(ex);
                await Task.Delay(1000);
                await Reporter.SendError(ex, errMsg);
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
                string pixivTag;
                string paramStr = command.KeyWord;
                if (string.IsNullOrWhiteSpace(paramStr))
                {
                    ProcessInfo stepInfo = await ProcessCache.CreateProcessAsync(command);
                    if (stepInfo is null) return;
                    StepInfo tagStep = new StepDetail(60, "请在60秒内发送要退订的标签名", CheckPixivTagAsync);
                    stepInfo.AddStep(tagStep);
                    if (await stepInfo.StartProcessing() == false) return;
                    pixivTag = tagStep.Answer;
                }
                else
                {
                    pixivTag = paramStr.Trim();
                    if (await CheckPixivTagAsync(command, pixivTag) == false) return;
                }

                SubscribePO dbSubscribe = subscribeBusiness.getSubscribe(pixivTag, SubscribeType.P站标签);
                if (dbSubscribe is null)
                {
                    await command.ReplyGroupMessageWithAtAsync($"退订失败，标签为[{pixivTag}]的订阅不存在");
                    return;
                }

                subscribeBusiness.cancleSubscribe(dbSubscribe.Id);
                await command.ReplyGroupMessageWithAtAsync($"已为所有群退订了pixiv标签[{pixivTag}]~");
                SubscribeDatas.LoadSubscribeTask();
            }
            catch (Exception ex)
            {
                string errMsg = $"退订pixiv标签失败";
                LogHelper.Error(ex, errMsg);
                await command.ReplyError(ex);
                await Task.Delay(1000);
                await Reporter.SendError(ex, errMsg);
            }
        }

        /// <summary>
        /// 发送画师的最新作品
        /// </summary>
        /// <param name="command"></param>
        /// <param name="dbSubscribe"></param>
        /// <param name="isShowR18"></param>
        /// <param name="isShowAI"></param>
        /// <returns></returns>
        private async Task sendUserNewestAsync(GroupCommand command, SubscribePO dbSubscribe, bool isShowR18, bool isShowAI)
        {
            try
            {
                List<PixivSubscribe> pixivSubscribeList = await pixivBusiness.getUserNewestAsync(dbSubscribe.SubscribeCode, dbSubscribe.Id, 1);
                if (pixivSubscribeList is null || pixivSubscribeList.Count == 0)
                {
                    await command.ReplyGroupMessageWithAtAsync($"画师[{dbSubscribe.SubscribeName}]还没有发布任何作品~");
                    return;
                }

                PixivSubscribe pixivSubscribe = pixivSubscribeList.First();
                PixivWorkInfo pixivWorkInfo = pixivSubscribe.PixivWorkInfo;
                if (await CheckSetuSendable(command, pixivWorkInfo, isShowR18) == false) return;

                List<BaseContent> workMsgs = new List<BaseContent>();
                List<FileInfo> setuFiles = await GetSetuFilesAsync(pixivWorkInfo, command.GroupId);
                workMsgs.Add(new PlainContent($"pixiv画师[{pixivWorkInfo.userName}]的最新作品："));
                workMsgs.Add(new PlainContent(pixivBusiness.getWorkInfo(pixivWorkInfo)));
                PixivSetuContent setuContent = new PixivSetuContent(workMsgs, setuFiles, pixivWorkInfo);
                await SendGroupSetuAsync(setuContent, command.GroupId);
            }
            catch (Exception ex)
            {
                string errMsg = $"读取画师[{dbSubscribe.SubscribeName}]的最新作品失败";
                LogHelper.Error(ex, errMsg);
                await command.ReplyError(ex);
                await Task.Delay(1000);
                await Reporter.SendError(ex, errMsg);
            }
        }

        private async Task<string[]> CheckUserIdsAsync(string value)
        {
            string[] idArr = value.splitParams();
            if (idArr.Length == 0)
            {
                throw new ProcessException("没有检测到用户id");
            }
            foreach (var idStr in idArr)
            {
                long userId = 0;
                if (long.TryParse(idStr, out userId) == false)
                {
                    throw new ProcessException($"用户id{idStr}必须为数字");
                }
                if (userId <= 0)
                {
                    throw new ProcessException($"用户id{idStr}无效");
                }
            }
            return await Task.FromResult(idArr);
        }

        private async Task<PixivSyncType> CheckSyncModeAsync(string value)
        {
            int modeId;
            if (int.TryParse(value, out modeId) == false)
            {
                throw new ProcessException("模式不在范围内");
            }
            if (Enum.IsDefined(typeof(PixivSyncType), modeId) == false)
            {
                throw new ProcessException("模式不在范围内");
            }
            return await Task.FromResult((PixivSyncType)modeId);
        }

        private async Task<GroupPushType> CheckPushTypeAsync(string value)
        {
            int typeId = 0;
            if (int.TryParse(value, out typeId) == false)
            {
                throw new ProcessException("目标不在范围内");
            }
            if (Enum.IsDefined(typeof(GroupPushType), typeId) == false)
            {
                throw new ProcessException("目标不在范围内");
            }
            return await Task.FromResult((GroupPushType)typeId);
        }


    }
}
