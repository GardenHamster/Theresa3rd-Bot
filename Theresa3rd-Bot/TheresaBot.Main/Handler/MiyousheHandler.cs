using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Mys;
using TheresaBot.Main.Model.Subscribe;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Services;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;
using TheresaBot.Main.Type.StepOptions;

namespace TheresaBot.Main.Handler
{
    internal class MiyousheHandler : BaseHandler
    {
        private MiyousheService miyousheService;
        private SubscribeService subscribeService;
        private SubscribeGroupService subscribeGroupService;

        public MiyousheHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            miyousheService = new MiyousheService();
            subscribeService = new SubscribeService();
            subscribeGroupService = new SubscribeGroupService();
        }

        public async Task SubscribeUserAsync(GroupCommand command)
        {
            try
            {
                var userId = 0L;
                var pushType = PushType.CurrentGroup;
                if (command.Params.Length == 2)
                {
                    userId = await CheckUserIdAsync(command.Params[0]);
                    pushType = await CheckPushTypeAsync(command.Params[1]);
                }
                else
                {
                    var processInfo = ProcessCache.CreateProcess(command);
                    var uidStep = processInfo.CreateStep("请在60秒内发送要订阅的用户ID", CheckUserIdAsync);
                    var groupStep = processInfo.CreateStep($"请在60秒内发送数字选择目标群：\r\n{EnumHelper.GroupPushOptions.JoinToString()}", CheckPushTypeAsync);
                    await processInfo.StartProcessing();
                    userId = uidStep.Answer;
                    pushType = groupStep.Answer;
                }

                var userData = await miyousheService.FetchUserInfoAsync(userId.ToString());
                if (userData is null || userData.retcode != 0)
                {
                    await command.ReplyGroupMessageWithAtAsync("订阅失败，目标用户不存在");
                    return;
                }

                var userInfo = userData.data.user_info;
                var subscribe = subscribeService.GetSubscribe(userId.ToString(), SubscribeType.米游社用户);
                if (subscribe is null)
                {
                    subscribe = subscribeService.InsertSubscribe(userInfo, userId.ToString());
                }

                if (subscribeGroupService.IsSubscribed(subscribe.Id, command.GroupId))
                {
                    await command.ReplyGroupMessageWithAtAsync($"已订阅这个用户了~");
                    return;
                }

                subscribeGroupService.InsertGroupSubscribe(subscribe.Id, pushType, command.GroupId);
                SubscribeDatas.LoadSubscribeTask();
                await command.ReplyGroupMessageWithAtAsync($"米游社用户【{subscribe.SubscribeName}】订阅成功!");
                await Task.Delay(1000);
                await SendSubscribeMessage(userInfo, pushType, command.GroupId);
            }
            catch (ProcessException ex)
            {
                await command.ReplyGroupMessageWithAtAsync(ex.RemindMessage);
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "米游社用户订阅异常");
            }
        }

        private async Task SendSubscribeMessage(MysUserInfo userInfo, PushType pushType, long groupId)
        {
            var contentList = new List<BaseContent>
            {
                new PlainContent($"UID：{userInfo.uid}"),
                new PlainContent($"昵称：{userInfo.nickname}"),
                new PlainContent($"目标群：{EnumHelper.GroupPushOptions.GetOptionName(pushType)}"),
                new PlainContent($"签名：{userInfo.introduce}")
            };
            var avatarUrl = userInfo.avatar_url;
            if (string.IsNullOrWhiteSpace(avatarUrl) == false)
            {
                var fullImgSavePath = FilePath.GetMiyousheImgSavePath(avatarUrl);
                var fileInfo = await HttpHelper.DownImgAsync(avatarUrl, fullImgSavePath);
                contentList.Add(new LocalImageContent(fileInfo));
            }
            await Session.SendGroupMessageAsync(groupId, contentList);
        }

        public async Task UnsubscribeUserAsync(GroupCommand command)
        {
            try
            {
                var userId = 0L;
                if (command.Params.Length >= 1)
                {
                    userId = await CheckUserIdAsync(command.Params[0]);
                }
                else
                {
                    var processInfo = ProcessCache.CreateProcess(command);
                    var uidStep = processInfo.CreateStep("请在60秒内发送要退订的用户ID", CheckUserIdAsync);
                    await processInfo.StartProcessing();
                    userId = uidStep.Answer;
                }
                var subscribeInfos = subscribeGroupService.GetSubscribes(userId.ToString(), SubscribeType.米游社用户);
                if (subscribeInfos.Count == 0)
                {
                    await command.ReplyGroupMessageWithAtAsync("并没有订阅这个用户哦~");
                    return;
                }
                var subscribeIds = subscribeInfos.Select(o => o.SubscribeId).Distinct().ToList();
                subscribeGroupService.DeleteBySubscribeId(subscribeIds);
                await command.ReplyGroupMessageWithAtAsync($"已为所有群退订了ID为【{userId}】的米游社用户~");
                SubscribeDatas.LoadSubscribeTask();
            }
            catch (ProcessException ex)
            {
                await command.ReplyGroupMessageWithAtAsync(ex.RemindMessage);
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "退订米游社用户异常");
            }
        }

        public async Task HandlePushAsync()
        {
            var subscribeType = SubscribeType.米游社用户;
            var subscribeTasks = SubscribeDatas.GetSubscribeTasks(subscribeType);
            foreach (SubscribeTask subscribeTask in subscribeTasks)
            {
                try
                {
                    if (subscribeTask.SubscribeSubType != 0) continue;
                    var postList = await miyousheService.ScanPostsAsync(subscribeTask);
                    await PushSubscribeAsync(subscribeTask, postList);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex, $"获取米游社[{subscribeTask.SubscribeCode}]订阅失败");
                }
                finally
                {
                    await Task.Delay(3000);
                }
            }
        }

        private async Task PushSubscribeAsync(SubscribeTask subscribeTask, List<MysSubscribe> postList)
        {
            foreach (MysSubscribe subscribe in postList)
            {
                await PushSubscribeAsync(subscribeTask, subscribe);
                await Task.Delay(1000);
            }
        }

        private async Task PushSubscribeAsync(SubscribeTask subscribeTask, MysSubscribe mysSubscribe)
        {
            var msgList = new List<BaseContent>()
            {
                new PlainContent(miyousheService.GetPostInfoAsync(mysSubscribe))
            };
            var coverUrl = mysSubscribe.SubscribeRecord.CoverUrl;
            if (string.IsNullOrWhiteSpace(coverUrl) == false)
            {
                var fullImgSavePath = FilePath.GetMiyousheImgSavePath(coverUrl);
                var fileInfo = await HttpHelper.DownImgAsync(coverUrl, fullImgSavePath);
                msgList.Add(new LocalImageContent(fileInfo));
            }
            foreach (long groupId in subscribeTask.SubscribeGroups)
            {
                await Session.SendGroupMessageAsync(groupId, msgList);
                await Task.Delay(2000);
            }
        }

        private async Task<long> CheckUserIdAsync(string value)
        {
            long userId = 0;
            if (long.TryParse(value, out userId) == false)
            {
                throw new ProcessException("用户id无效");
            }
            if (userId <= 0)
            {
                throw new ProcessException("用户id无效");
            }
            return await Task.FromResult(userId);
        }

        private async Task<PushType> CheckPushTypeAsync(string value)
        {
            int typeId = 0;
            if (int.TryParse(value, out typeId) == false)
            {
                throw new ProcessException("推送类型不在范围内");
            }
            if (Enum.IsDefined(typeof(PushType), typeId) == false)
            {
                throw new ProcessException("推送类型不在范围内");
            }
            return await Task.FromResult((PushType)typeId);
        }

    }
}
