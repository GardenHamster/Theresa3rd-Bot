using TheresaBot.Main.Business;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Mys;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Model.Step;
using TheresaBot.Main.Model.Subscribe;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;
using TheresaBot.Main.Type.StepOption;


namespace TheresaBot.Main.Handler
{
    internal class MiyousheHandler : BaseHandler
    {
        private MiyousheBusiness miyousheBusiness;
        private SubscribeBusiness subscribeBusiness;

        public MiyousheHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            miyousheBusiness = new MiyousheBusiness();
            subscribeBusiness = new SubscribeBusiness();
        }

        public async Task SubscribeUserAsync(GroupCommand command)
        {
            try
            {
                long userId = 0;
                GroupPushType groupType = GroupPushType.CurrentGroup;
                if (command.Params.Length >= 2)
                {
                    userId = await CheckUserIdAsync(command.Params[0]);
                    groupType = await CheckPushTypeAsync(command.Params[1]);
                }
                else
                {
                    ProcessInfo processInfo = ProcessCache.CreateProcessAsync(command);
                    StepInfo uidStep = processInfo.CreateStep("请在60秒内发送要订阅用户的id", CheckUserIdAsync);
                    StepInfo groupStep = processInfo.CreateStep($"请在60秒内发送数字选择目标群：\r\n{EnumHelper.PixivSyncGroupOption()}", CheckPushTypeAsync);
                    await processInfo.StartProcessing();
                    userId = uidStep.AnswerForLong();
                    groupType = groupStep.AnswerForEnum<GroupPushType>();
                }

                MysResult<MysUserFullInfoDto> userInfoDto = await miyousheBusiness.geMysUserFullInfoDtoAsync(userId.ToString());
                if (userInfoDto is null || userInfoDto.retcode != 0)
                {
                    await command.ReplyGroupMessageWithAtAsync("订阅失败，目标用户不存在");
                    return;
                }

                long subscribeGroupId = groupType == GroupPushType.AllGroup ? 0 : command.GroupId;
                SubscribePO dbSubscribe = subscribeBusiness.getSubscribe(userId.ToString(), SubscribeType.米游社用户);
                if (dbSubscribe is null) dbSubscribe = subscribeBusiness.insertSurscribe(userInfoDto.data.user_info, userId.ToString());
                if (subscribeBusiness.isExistsSubscribeGroup(subscribeGroupId, dbSubscribe.Id))
                {
                    await command.ReplyGroupMessageWithAtAsync($"已订阅了该用户~");
                    return;
                }
                subscribeBusiness.insertSubscribeGroup(subscribeGroupId, dbSubscribe.Id);

                List<BaseContent> chailList = new List<BaseContent>();
                chailList.Add(new PlainContent($"米游社用户[{dbSubscribe.SubscribeName}]订阅成功!"));
                chailList.Add(new PlainContent($"目标群：{Enum.GetName(typeof(GroupPushType), groupType)}"));
                chailList.Add(new PlainContent($"uid：{dbSubscribe.SubscribeCode}"));
                chailList.Add(new PlainContent($"签名：{userInfoDto.data.user_info.introduce}"));

                string avatar_url = userInfoDto.data.user_info.avatar_url;
                if (string.IsNullOrWhiteSpace(avatar_url) == false)
                {
                    string fullImgSavePath = FilePath.GetMiyousheImgSavePath(avatar_url);
                    FileInfo fileInfo = await HttpHelper.DownImgAsync(avatar_url, fullImgSavePath);
                    chailList.Add(new LocalImageContent(fileInfo));
                }

                await command.ReplyGroupMessageWithAtAsync(chailList);
                SubscribeDatas.LoadSubscribeTask();
            }
            catch (ProcessException ex)
            {
                await command.ReplyGroupMessageWithAtAsync(ex.RemindMessage);
            }
            catch (Exception ex)
            {
                await LogAndReportError(command, ex, "订阅米游社用户异常");
            }
        }

        public async Task CancleSubscribeUserAsync(GroupCommand command)
        {
            try
            {
                long userId = await CheckUserIdAsync(command.KeyWord);
                List<SubscribePO> subscribeList = miyousheBusiness.getSubscribeList(userId.ToString());
                if (subscribeList.Count == 0)
                {
                    await command.ReplyGroupMessageWithAtAsync("并没有订阅这个用户哦~");
                    return;
                }
                foreach (var item in subscribeList)
                {
                    subscribeBusiness.cancleSubscribe(item.Id);
                }
                await command.ReplyGroupMessageWithAtAsync($"已为所有群退订了ID为{userId}的米游社用户~");
                SubscribeDatas.LoadSubscribeTask();
            }
            catch (ProcessException ex)
            {
                await command.ReplyGroupMessageWithAtAsync(ex.RemindMessage);
            }
            catch (Exception ex)
            {
                await LogAndReportError(command, ex, "退订米游社用户异常");
            }
        }

        public async Task HandleSubscribeAsync()
        {
            var subscribeType = SubscribeType.米游社用户;
            var subscribeTaskList = SubscribeDatas.GetSubscribeTasks(subscribeType);
            foreach (SubscribeTask subscribeTask in subscribeTaskList)
            {
                try
                {
                    if (subscribeTask.SubscribeSubType != 0) continue;
                    List<MysSubscribe> mysSubscribeList = await miyousheBusiness.getMysUserSubscribeAsync(subscribeTask);
                    if (mysSubscribeList is null || mysSubscribeList.Count == 0) continue;
                    await PushSubscribeAsync(subscribeTask, mysSubscribeList);
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

        private async Task PushSubscribeAsync(SubscribeTask subscribeTask, List<MysSubscribe> mysSubscribeList)
        {
            foreach (MysSubscribe mysSubscribe in mysSubscribeList)
            {
                string coverUrl = mysSubscribe.SubscribeRecord.CoverUrl;
                if (subscribeTask.GroupIdList.Count == 0) continue;

                List<BaseContent> msgList = new List<BaseContent>();
                msgList.Add(new PlainContent(miyousheBusiness.getPostInfoAsync(mysSubscribe)));

                if (string.IsNullOrWhiteSpace(coverUrl) == false)
                {
                    string fullImgSavePath = FilePath.GetMiyousheImgSavePath(coverUrl);
                    FileInfo fileInfo = await HttpHelper.DownImgAsync(coverUrl, fullImgSavePath);
                    msgList.Add(new LocalImageContent(fileInfo));
                }

                foreach (long groupId in subscribeTask.GroupIdList)
                {
                    await Session.SendGroupMessageAsync(groupId, msgList);
                    await Task.Delay(1000);
                }
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

        private async Task<GroupPushType> CheckPushTypeAsync(string value)
        {
            int typeId = 0;
            if (int.TryParse(value, out typeId) == false)
            {
                throw new ProcessException("推送类型不在范围内");
            }
            if (Enum.IsDefined(typeof(GroupPushType), typeId) == false)
            {
                throw new ProcessException("推送类型不在范围内");
            }
            return await Task.FromResult((GroupPushType)typeId);
        }

    }
}
