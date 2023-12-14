using SqlSugar.IOC;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Pixiv;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Services;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;
using TheresaBot.Main.Type.StepOptions;

namespace TheresaBot.Main.Handler
{
    internal class PixivSubscribeHandler : SetuHandler
    {
        private PixivService pixivService;
        private SubscribeService subscribeService;
        private SubscribeGroupService subscribeGroupService;

        public PixivSubscribeHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            pixivService = new PixivService();
            subscribeService = new SubscribeService();
            subscribeGroupService = new SubscribeGroupService();
        }

        /// <summary>
        /// 订阅pixiv画师
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task SubscribeUserAsync(GroupCommand command)
        {
            try
            {
                var userIds = new long[0];
                var pushType = PushType.CurrentGroup;
                if (command.Params.Length == 2)
                {
                    userIds = await CheckUserIdsAsync(command.Params[0]);
                    pushType = await CheckPushTypeAsync(command.Params[1]);
                }
                else
                {
                    var processInfo = ProcessCache.CreateProcess(command);
                    var uidStep = processInfo.CreateStep("请在60秒内发送要订阅用户的id，多个id之间可以用逗号或者换行隔开", CheckUserIdsAsync);
                    var typeStep = processInfo.CreateStep($"请在60秒内发送数字选择目标群：\r\n{EnumHelper.GroupPushOptions.JoinToString()}", CheckPushTypeAsync);
                    await processInfo.StartProcessing();
                    userIds = uidStep.Answer;
                    pushType = typeStep.Answer;
                }

                if (userIds.Length > 1)
                {
                    await command.ReplyGroupMessageWithQuoteAsync("检测到多个id，开始批量订阅~");
                    await Task.Delay(1000);
                }

                foreach (var userId in userIds)
                {
                    await SubscribeUserAsync(command, userId.ToString(), pushType);
                    await Task.Delay(1000);
                }

                SubscribeDatas.LoadSubscribeTask();

                if (userIds.Length > 1)
                {
                    await command.ReplyGroupMessageWithAtAsync("订阅完毕~");
                }
            }
            catch (ProcessException ex)
            {
                await command.ReplyGroupMessageWithQuoteAsync(ex.RemindMessage);
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, $"pixiv用户订阅异常");
            }
        }

        /// <summary>
        /// 订阅pixiv画师
        /// </summary>
        /// <param name="command"></param>
        /// <param name="userId"></param>
        /// <param name="pushType"></param>
        /// <returns></returns>
        private async Task SubscribeUserAsync(GroupCommand command, string userId, PushType pushType)
        {
            try
            {
                var subscribe = subscribeService.GetSubscribe(userId, SubscribeType.P站画师);
                if (subscribe is null)
                {
                    PixivUserProfileTop pixivUserInfo = await PixivHelper.GetPixivUserProfileTopAsync(userId);
                    subscribe = subscribeService.AddSurscribe(pixivUserInfo, userId);
                }

                if (subscribeGroupService.IsSubscribed(subscribe.Id, command.GroupId))
                {
                    await command.ReplyGroupMessageWithQuoteAsync($"画师【{userId}】已经被订阅了~");
                    return;
                }

                subscribeGroupService.AddGroupSubscribe(subscribe.Id, pushType, command.GroupId);
                SubscribeDatas.LoadSubscribeTask();
                await command.ReplyGroupMessageWithAtAsync($"画师【{subscribe.SubscribeName}】订阅成功，正在读取最新作品~");
                await Task.Delay(1000);
                await ReplyUserNewestWorkAsync(command, subscribe);
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, $"pixiv画师[{userId}]订阅失败");
            }
            finally
            {
                await Task.Delay(2000);
            }
        }

        /// <summary>
        /// 订阅pixiv关注画师
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task SubscribeFollowUserAsync(GroupCommand command)
        {
            try
            {
                var processInfo = ProcessCache.CreateProcess(command);
                var modeStep = processInfo.CreateStep($"请在60秒内发送数字选择模式：\r\n{EnumHelper.PixivSyncOptions.JoinToString()}", CheckSyncModeAsync);
                var groupStep = processInfo.CreateStep($"请在60秒内发送数字选择目标群：\r\n{EnumHelper.GroupPushOptions.JoinToString()}", CheckPushTypeAsync);
                await processInfo.StartProcessing();
                var syncType = modeStep.Answer;
                var pushType = groupStep.Answer;

                await command.ReplyGroupMessageWithAtAsync("正在获取Pixiv账号中已关注的画师列表...");
                await Task.Delay(1000);

                var followUsers = await pixivService.getFollowUserList();
                if (followUsers.Count == 0)
                {
                    await command.ReplyGroupMessageWithQuoteAsync("Pixiv账号中还没有关注任何画师");
                    return;
                }

                await command.ReplyGroupMessageWithAtAsync($"已获取{followUsers.Count}个画师，正在开始导入...");
                await Task.Delay(1000);
                SyncUserAsync(followUsers, syncType, pushType, command.GroupId);
                SubscribeDatas.LoadSubscribeTask();
                await command.ReplyGroupMessageWithQuoteAsync("Pixiv画师关注列表同步完毕~");
            }
            catch (ProcessException ex)
            {
                await command.ReplyGroupMessageWithQuoteAsync(ex.RemindMessage);
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, $"Pixiv画师关注列表同步失败");
            }
        }

        public void SyncUserAsync(List<PixivFollowUser> followUsers, PixivSyncType syncType, PushType pushType, long groupId)
        {
            try
            {
                var syncDate = DateTime.Now;
                var subscribeList = new List<SubscribePO>();
                DbScoped.SugarScope.BeginTran();//开始事务

                foreach (var item in followUsers)
                {
                    var subscribe = subscribeService.GetSubscribe(item.userId, SubscribeType.P站画师);
                    if (subscribe is null) subscribe = subscribeService.AddSurscribe(item, syncDate);
                    subscribeList.Add(subscribe);
                }

                var subscribeIds = subscribeList.Select(o => o.Id).ToList();
                if (syncType == PixivSyncType.Overwrite)//覆盖情况下，移除所有列表中的关联数据
                {
                    subscribeGroupService.DeleteBySubscribeId(subscribeIds);
                }

                //插入关联数据，如果数据已存在则跳过
                foreach (var subscribeId in subscribeIds)
                {
                    subscribeGroupService.AddGroupSubscribe(subscribeId, pushType, groupId);
                }

                DbScoped.SugarScope.CommitTran();//提交事务
            }
            catch (Exception)
            {
                DbScoped.SugarScope.RollbackTran();
                throw;
            }
        }


        /// <summary>
        /// 移除画师订阅
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task UnsubscribeUserAsync(GroupCommand command)
        {
            try
            {
                var userId = 0L;
                if (command.KeyWord.Length > 0)
                {
                    userId = await CheckUserIdAsync(command.KeyWord);
                }
                else
                {
                    var processInfo = ProcessCache.CreateProcess(command);
                    var uidStep = processInfo.CreateStep("请在60秒内发送要退订的用户Id", CheckUserIdAsync);
                    await processInfo.StartProcessing();
                    userId = uidStep.Answer;
                }

                var subscribe = subscribeService.GetSubscribe(userId.ToString(), SubscribeType.P站画师);
                if (subscribe is null)
                {
                    await command.ReplyGroupMessageWithQuoteAsync($"退订失败，订阅不存在");
                    return;
                }

                subscribeGroupService.DeleteBySubscribeId(subscribe.Id);
                SubscribeDatas.LoadSubscribeTask();
                await command.ReplyGroupMessageWithQuoteAsync($"已为所有群进行了退订~");
            }
            catch (ProcessException ex)
            {
                await command.ReplyGroupMessageWithQuoteAsync(ex.RemindMessage);
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "取消画师订阅异常");
            }
        }

        /// <summary>
        /// 添加标签订阅
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task SubscribeTagAsync(GroupCommand command)
        {
            try
            {
                var pixivTag = string.Empty;
                var pushType = PushType.CurrentGroup;
                if (command.Params.Length == 2)
                {
                    pixivTag = command.Params[0].Trim();
                    pushType = await CheckPushTypeAsync(command.Params[1]);
                }
                else
                {
                    var processInfo = ProcessCache.CreateProcess(command);
                    var tagStep = processInfo.CreateStep($"请在60秒内发送要订阅的标签名");
                    var typeStep = processInfo.CreateStep($"请在60秒内发送数字选择目标群：\r\n{EnumHelper.GroupPushOptions.JoinToString()}", CheckPushTypeAsync);
                    await processInfo.StartProcessing();
                    pixivTag = tagStep.Answer;
                    pushType = typeStep.Answer;
                }

                var searchWord = pixivService.toPixivSearchWords(pixivTag.ToActualPixivTags());
                var pageOne = await PixivHelper.GetPixivSearchAsync(searchWord, 1, false, true);
                if (pageOne?.getIllust()?.data is null || pageOne.getIllust().data.Count == 0)
                {
                    await command.ReplyGroupMessageWithQuoteAsync($"标签【{pixivTag}】中没有任何作品，订阅失败");
                    return;
                }

                var subscribe = subscribeService.GetSubscribe(pixivTag, SubscribeType.P站标签);
                if (subscribe is null)
                {
                    subscribe = subscribeService.AddSurscribe(pixivTag);
                }

                if (subscribeGroupService.IsSubscribed(subscribe.Id, command.GroupId))
                {
                    await command.ReplyGroupMessageWithQuoteAsync($"标签【{pixivTag}】已经订阅了~");
                    return;
                }

                subscribeGroupService.AddGroupSubscribe(subscribe.Id, pushType, command.GroupId);
                SubscribeDatas.LoadSubscribeTask();
                await command.ReplyGroupMessageWithQuoteAsync($"标签【{pixivTag}】订阅成功，作品总数为：{pageOne.illust.total}");
            }
            catch (ProcessException ex)
            {
                await command.ReplyGroupMessageWithQuoteAsync(ex.RemindMessage);
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "Pixiv标签订阅异常");
            }
        }

        /// <summary>
        /// 移除标签订阅
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task UnsubscribeTagAsync(GroupCommand command)
        {
            try
            {
                var pixivTag = command.KeyWord;
                if (command.KeyWord.Length == 0)
                {
                    var processInfo = ProcessCache.CreateProcess(command);
                    var tagStep = processInfo.CreateStep("请在60秒内发送要退订的标签", CheckTextAsync);
                    await processInfo.StartProcessing();
                    pixivTag = tagStep.Answer;
                }

                var subscribe = subscribeService.GetSubscribe(pixivTag, SubscribeType.P站标签);
                if (subscribe is null)
                {
                    await command.ReplyGroupMessageWithQuoteAsync($"退订失败，订阅不存在");
                    return;
                }

                subscribeGroupService.DeleteBySubscribeId(subscribe.Id);
                SubscribeDatas.LoadSubscribeTask();
                await command.ReplyGroupMessageWithQuoteAsync($"已为所有群退订了标签[{pixivTag}]~");
            }
            catch (ProcessException ex)
            {
                await command.ReplyGroupMessageWithQuoteAsync(ex.RemindMessage);
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "Pixiv标签退订异常");
            }
        }

        /// <summary>
        /// 回复画师的最新作品
        /// </summary>
        /// <param name="command"></param>
        /// <param name="dbSubscribe"></param>
        /// <returns></returns>
        private async Task ReplyUserNewestWorkAsync(GroupCommand command, SubscribePO dbSubscribe)
        {
            try
            {
                var isShowR18 = command.GroupId.IsShowR18();
                var pixivSubscribeList = await pixivService.getUserNewestAsync(dbSubscribe.SubscribeCode, dbSubscribe.Id, 1);
                if (pixivSubscribeList is null || pixivSubscribeList.Count == 0)
                {
                    await command.ReplyGroupMessageWithAtAsync($"画师【{dbSubscribe.SubscribeName}】还没有发布任何作品~");
                    return;
                }
                var pixivSubscribe = pixivSubscribeList.First();
                var pixivWorkInfo = pixivSubscribe.PixivWorkInfo;

                var unSendableMsg = CheckSendable(pixivWorkInfo, isShowR18);
                if (string.IsNullOrWhiteSpace(unSendableMsg) == false)
                {
                    await Session.SendGroupMessageAsync(command.GroupId, unSendableMsg);
                    return;
                }

                var workMsgs = new List<BaseContent>
                {
                    new PlainContent($"pixiv画师【{pixivWorkInfo.userName}】的最新作品："),
                    new PlainContent(pixivService.getWorkInfo(pixivWorkInfo))
                };
                var setuFiles = await GetSetuFilesAsync(pixivWorkInfo, command.GroupId);
                var setuContent = new PixivSetuContent(workMsgs, setuFiles, pixivWorkInfo);
                await SendGroupSetuAsync(setuContent, command.GroupId);
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, $"读取画师【{dbSubscribe.SubscribeName}】最新作品失败");
            }
        }

        private async Task<long[]> CheckUserIdsAsync(string value)
        {
            var userIds = new List<long>();
            var splitArr = value.SplitParams();
            if (splitArr.Length == 0)
            {
                throw new ProcessException("没有检测到用户id");
            }
            foreach (var idStr in splitArr)
            {
                userIds.Add(await CheckUserIdAsync(idStr));
            }
            return await Task.FromResult(userIds.ToArray());
        }

        private async Task<long> CheckUserIdAsync(string value)
        {
            long userId = 0;
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ProcessException("没有检测到用户id");
            }
            if (long.TryParse(value, out userId) == false)
            {
                throw new ProcessException($"用户id{value}必须为数字");
            }
            if (userId <= 0)
            {
                throw new ProcessException($"用户id{value}无效");
            }
            return await Task.FromResult(userId);
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

        private async Task<PushType> CheckPushTypeAsync(string value)
        {
            int typeId = 0;
            if (int.TryParse(value, out typeId) == false)
            {
                throw new ProcessException("目标不在范围内");
            }
            if (Enum.IsDefined(typeof(PushType), typeId) == false)
            {
                throw new ProcessException("目标不在范围内");
            }
            return await Task.FromResult((PushType)typeId);
        }


    }
}
