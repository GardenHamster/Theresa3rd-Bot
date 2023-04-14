using SqlSugar.IOC;
using TheresaBot.Main.Business;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Pixiv;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Model.Step;
using TheresaBot.Main.Model.Subscribe;
using TheresaBot.Main.Relay;
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
        public async Task subscribeUserAsync(GroupCommand command)
        {
            try
            {
                string pixivUserIds;
                SubscribeGroupType? groupType = null;

                string[] paramArr = command.Params;
                if (paramArr != null && paramArr.Length >= 2)
                {
                    pixivUserIds = paramArr.Length > 0 ? paramArr[0] : string.Empty;
                    string groupTypeStr = paramArr.Length > 1 ? paramArr[1] : string.Empty;
                    if (await CheckPixivUserIdsAsync(command, pixivUserIds) == false) return;
                    if (await CheckSubscribeGroupAsync(command, groupTypeStr) == false) return;
                    groupType = (SubscribeGroupType)Convert.ToInt32(groupTypeStr);
                }
                else
                {
                    StepInfo stepInfo = await StepCache.CreateStepAsync(command);
                    if (stepInfo is null) return;
                    StepDetail uidStep = new StepDetail(60, "请在60秒内发送要订阅用户的id，多个id之间可以用逗号或者换行隔开", CheckPixivUserIdsAsync);
                    StepDetail groupStep = new StepDetail(60, $"请在60秒内发送数字选择目标群：\r\n{EnumHelper.PixivSyncGroupOption()}", CheckSubscribeGroupAsync);
                    stepInfo.AddStep(uidStep);
                    stepInfo.AddStep(groupStep);
                    if (await stepInfo.HandleStep() == false) return;
                    pixivUserIds = uidStep.Answer;
                    groupType = (SubscribeGroupType)Convert.ToInt32(groupStep.Answer);
                }

                string[] pixivUserIdArr = pixivUserIds.splitParams();
                if (pixivUserIdArr.Length > 1)
                {
                    await command.ReplyGroupMessageWithAtAsync("检测到多个id，开始批量订阅~");
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
                            PixivUserProfileTop pixivUserInfo = await PixivHelper.GetPixivUserProfileTopAsync(pixivUserId);
                            dbSubscribe = subscribeBusiness.insertSurscribe(pixivUserInfo, pixivUserId);
                        }

                        long subscribeGroupId = groupType == SubscribeGroupType.All ? 0 : command.GroupId;
                        if (subscribeBusiness.isExistsSubscribeGroup(subscribeGroupId, dbSubscribe.Id))
                        {
                            //关联订阅
                            await command.ReplyGroupMessageWithAtAsync($"画师id[{pixivUserId}]已经被订阅了~");
                            continue;
                        }

                        SubscribeGroupPO subscribeGroup = subscribeBusiness.insertSubscribeGroup(subscribeGroupId, dbSubscribe.Id);
                        await command.ReplyGroupMessageWithAtAsync($"画师id[{dbSubscribe.SubscribeCode}]订阅成功，正在读取最新作品~");

                        await Task.Delay(1000);
                        await sendUserNewestAsync(command, dbSubscribe, command.GroupId.IsShowR18Setu(), command.GroupId.IsShowAISetu());
                    }
                    catch (Exception ex)
                    {
                        string errMsg = $"pixiv画师[{pixivUserId}]订阅失败";
                        LogHelper.Error(ex, errMsg);
                        await command.ReplyError(ex);
                        await Task.Delay(1000);
                        await Reporter.SendErrorForce(ex, errMsg);
                    }
                    finally
                    {
                        Thread.Sleep(2000);
                    }
                }
                if (pixivUserIdArr.Length > 1)
                {
                    await command.ReplyGroupMessageWithAtAsync($"所有画师订阅完毕");
                }
                ConfigHelper.LoadSubscribeTask();
            }
            catch (Exception ex)
            {
                string errMsg = $"订阅pixiv用户失败";
                LogHelper.Error(ex, errMsg);
                await command.ReplyGroupMessageWithAtAsync(errMsg);
                Reporter.SendError(ex, errMsg);
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
                StepInfo stepInfo = await StepCache.CreateStepAsync(command);
                if (stepInfo is null) return;

                StepDetail modeStep = new StepDetail(60, $"请在60秒内发送数字选择模式：\r\n{EnumHelper.PixivSyncModeOption()}", CheckSyncModeAsync);
                StepDetail groupStep = new StepDetail(60, $"请在60秒内发送数字选择目标群：\r\n{EnumHelper.PixivSyncGroupOption()}", CheckSubscribeGroupAsync);
                stepInfo.AddStep(modeStep);
                stepInfo.AddStep(groupStep);
                if (await stepInfo.HandleStep() == false) return;

                PixivSyncModeType syncMode = (PixivSyncModeType)Convert.ToInt32(modeStep.Answer);
                SubscribeGroupType syncGroup = (SubscribeGroupType)Convert.ToInt32(groupStep.Answer);

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

                long subscribeGroupId = syncGroup == SubscribeGroupType.All ? 0 : command.GroupId;
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
                await command.ReplyGroupMessageWithAtAsync("订阅pixiv关注画师列表完毕");
                ConfigHelper.LoadSubscribeTask();
            }
            catch (Exception ex)
            {
                string errMsg = $"订阅pixiv关注画师列表失败";
                LogHelper.Error(ex, errMsg);
                DbScoped.SugarScope.RollbackTran();//事务回滚
                await command.ReplyError(ex);
                await Task.Delay(1000);
                Reporter.SendError(ex, errMsg);
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
                    StepInfo stepInfo = await StepCache.CreateStepAsync(command);
                    if (stepInfo is null) return;
                    StepDetail uidStep = new StepDetail(60, "请在60秒内发送要退订用户的id，多个id之间可以用逗号或者换行隔开", CheckPixivUserIdsAsync);
                    stepInfo.AddStep(uidStep);
                    if (await stepInfo.HandleStep() == false) return;
                    pixivUserIds = uidStep.Answer;
                }
                else
                {
                    pixivUserIds = paramStr.Trim();
                    if (await CheckPixivUserIdsAsync(command, pixivUserIds) == false) return;
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
                    subscribeBusiness.delSubscribeGroup(dbSubscribe.Id);
                }

                await command.ReplyGroupMessageWithAtAsync($"已为所有群退订了pixiv用户[{pixivUserIds}]~");
                ConfigHelper.LoadSubscribeTask();
            }
            catch (Exception ex)
            {
                string errMsg = $"取消pixiv画师订阅失败";
                LogHelper.Error(ex, errMsg);
                await command.ReplyError(ex);
                await Task.Delay(1000);
                Reporter.SendError(ex, errMsg);
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
                SubscribeGroupType? groupType;

                string[] paramArr = command.Params;
                if (paramArr != null && paramArr.Length >= 2)
                {
                    pixivTags = paramArr.Length > 0 ? paramArr[0] : string.Empty;
                    string groupTypeStr = paramArr.Length > 1 ? paramArr[1] : string.Empty;
                    if (await CheckPixivTagAsync(command, pixivTags) == false) return;
                    if (await CheckSubscribeGroupAsync(command, groupTypeStr) == false) return;
                    groupType = (SubscribeGroupType)Convert.ToInt32(groupTypeStr);
                }
                else
                {
                    StepInfo stepInfo = await StepCache.CreateStepAsync(command);
                    if (stepInfo is null) return;
                    StepDetail tagStep = new StepDetail(60, $"请在60秒内发送要订阅的标签名", CheckPixivTagAsync);
                    StepDetail groupStep = new StepDetail(60, $"请在60秒内发送数字选择目标群：\r\n{EnumHelper.PixivSyncGroupOption()}", CheckSubscribeGroupAsync);
                    stepInfo.AddStep(tagStep);
                    stepInfo.AddStep(groupStep);
                    if (await stepInfo.HandleStep() == false) return;
                    pixivTags = tagStep.Answer;
                    groupType = (SubscribeGroupType)Convert.ToInt32(groupStep.Answer);
                }

                string searchWord = pixivBusiness.toPixivSearchWord(pixivTags);
                PixivSearch pageOne = await PixivHelper.GetPixivSearchAsync(searchWord, 1, false, command.GroupId.IsShowR18Setu());
                if (pageOne is null || pageOne.getIllust().data.Count == 0)
                {
                    await command.ReplyGroupMessageWithAtAsync("该标签中没有任何作品，订阅失败");
                    return;
                }

                SubscribePO dbSubscribe = subscribeBusiness.getSubscribe(pixivTags, SubscribeType.P站标签);
                if (dbSubscribe is null) dbSubscribe = subscribeBusiness.insertSurscribe(pixivTags);

                long subscribeGroupId = groupType == SubscribeGroupType.All ? 0 : command.GroupId;
                if (subscribeBusiness.isExistsSubscribeGroup(subscribeGroupId, dbSubscribe.Id))
                {
                    //关联订阅
                    await command.ReplyGroupMessageWithAtAsync($"这个标签已经被订阅了~");
                    return;
                }

                SubscribeGroupPO subscribeGroup = subscribeBusiness.insertSubscribeGroup(subscribeGroupId, dbSubscribe.Id);
                await command.ReplyGroupMessageWithAtAsync($"标签[{pixivTags}]订阅成功,该标签总作品数为:{pageOne.illust.total}");
                ConfigHelper.LoadSubscribeTask();
            }
            catch (Exception ex)
            {
                string errMsg = $"订阅pixiv标签失败";
                LogHelper.Error(ex, errMsg);
                await command.ReplyError(ex);
                await Task.Delay(1000);
                Reporter.SendError(ex, errMsg);
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
                    StepInfo stepInfo = await StepCache.CreateStepAsync(command);
                    if (stepInfo is null) return;
                    StepDetail tagStep = new StepDetail(60, "请在60秒内发送要退订的标签名", CheckPixivTagAsync);
                    stepInfo.AddStep(tagStep);
                    if (await stepInfo.HandleStep() == false) return;
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
                    await command.ReplyGroupMessageWithAtAsync("退订失败，标签为[{pixivTag}]的订阅不存在");
                    return;
                }

                subscribeBusiness.delSubscribeGroup(dbSubscribe.Id);
                await command.ReplyGroupMessageWithAtAsync($"已为所有群退订了pixiv标签[{pixivTag}]~");
                ConfigHelper.LoadSubscribeTask();
            }
            catch (Exception ex)
            {
                string errMsg = $"退订pixiv标签失败";
                LogHelper.Error(ex, errMsg);
                await command.ReplyError(ex);
                await Task.Delay(1000);
                Reporter.SendError(ex, errMsg);
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
                    await command.ReplyGroupMessageAsync($"画师[{dbSubscribe.SubscribeName}]还没有发布任何作品~");
                    return;
                }

                PixivSubscribe pixivSubscribe = pixivSubscribeList.First();
                PixivWorkInfo pixivWorkInfo = pixivSubscribe.PixivWorkInfo;
                if (await CheckSetuSendable(command, pixivWorkInfo, isShowR18) == false) return;

                List<BaseContent> workMsgs = new List<BaseContent>();
                List<FileInfo> setuFiles = await GetSetuFilesAsync(pixivWorkInfo, command.GroupId);
                workMsgs.Add(new PlainContent($"pixiv画师[{pixivWorkInfo.userName}]的最新作品："));
                workMsgs.Add(new PlainContent(pixivBusiness.getWorkInfo(pixivWorkInfo, BotConfig.PixivConfig.Template)));
                SetuContent setuContent = new SetuContent(workMsgs, setuFiles);
                await Session.SendGroupMessageAsync(command.GroupId, setuContent, BotConfig.PixivConfig.SendImgBehind);
            }
            catch (Exception ex)
            {
                string errMsg = $"读取画师[{dbSubscribe.SubscribeName}]的最新作品失败";
                LogHelper.Error(ex, errMsg);
                await command.ReplyError(ex);
                await Task.Delay(1000);
                Reporter.SendError(ex, errMsg);
            }
        }


        private async Task<bool> CheckPixivTagAsync(GroupCommand command, GroupRelay relay)
        {
            return await CheckPixivTagAsync(command, relay.Message);
        }

        private async Task<bool> CheckPixivUserIdsAsync(GroupCommand command, GroupRelay relay)
        {
            return await CheckPixivUserIdsAsync(command, relay.Message);
        }

        private async Task<bool> CheckSyncModeAsync(GroupCommand command, GroupRelay relay)
        {
            return await CheckSyncModeAsync(command, relay.Message);
        }


        private async Task<bool> CheckSubscribeGroupAsync(GroupCommand command, GroupRelay relay)
        {
            return await CheckSubscribeGroupAsync(command, relay.Message);
        }


        private async Task<bool> CheckPixivTagAsync(GroupCommand command, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                await command.ReplyGroupMessageWithAtAsync("标签不可以为空");
                return false;
            }
            return true;
        }

        private async Task<bool> CheckPixivUserIdsAsync(GroupCommand command, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                await command.ReplyGroupMessageWithAtAsync("用户id不可以为空");
                return false;
            }

            string[] pixivUserIdArr = value.splitParams();
            if (pixivUserIdArr.Length == 0)
            {
                await command.ReplyGroupMessageWithAtAsync("没有检测到用户id");
                return false;
            }

            foreach (var userIdStr in pixivUserIdArr)
            {
                long userId = 0;
                if (long.TryParse(userIdStr, out userId) == false)
                {
                    await command.ReplyGroupMessageWithAtAsync($"用户id{userIdStr}必须为数字");
                    return false;
                }
                if (userId <= 0)
                {
                    await command.ReplyGroupMessageWithAtAsync($"用户id{userIdStr}无效");
                    return false;
                }
            }
            return true;
        }

        private async Task<bool> CheckSyncModeAsync(GroupCommand command, string value)
        {
            int modeId;
            if (int.TryParse(value, out modeId) == false)
            {
                await command.ReplyGroupMessageWithAtAsync("mode必须为数字");
                return false;
            }
            if (Enum.IsDefined(typeof(PixivSyncModeType), modeId) == false)
            {
                await command.ReplyGroupMessageWithAtAsync("mode不在范围内");
                return false;
            }
            return true;
        }

        private async Task<bool> CheckSubscribeGroupAsync(GroupCommand command, string value)
        {
            int typeId = 0;
            if (int.TryParse(value, out typeId) == false)
            {
                await command.ReplyGroupMessageWithAtAsync("target必须为数字");
                return false;
            }
            if (Enum.IsDefined(typeof(SubscribeGroupType), typeId) == false)
            {
                await command.ReplyGroupMessageWithAtAsync("target不在范围内");
                return false;
            }
            return true;
        }


    }
}
