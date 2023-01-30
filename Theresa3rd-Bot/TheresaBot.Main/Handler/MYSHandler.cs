using TheresaBot.Main.Business;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Mys;
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
    public class MYSHandler : BaseHandler
    {
        private MYSBusiness mysBusiness;
        private SubscribeBusiness subscribeBusiness;

        public MYSHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            mysBusiness = new MYSBusiness();
            subscribeBusiness = new SubscribeBusiness();
        }

        /// <summary>
        /// 订阅米游社用户
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task subscribeMYSUserAsync(GroupCommand command)
        {
            try
            {
                string userId;
                SubscribeGroupType? groupType = null;
                string[] paramArr = command.Params;
                if (paramArr != null && paramArr.Length >= 2)
                {
                    userId = paramArr.Length > 0 ? paramArr[0] : string.Empty;
                    string groupTypeStr = paramArr.Length > 1 ? paramArr[1] : string.Empty;
                    if (await CheckUserIdAsync(command, userId) == false) return;
                    if (await CheckSubscribeGroupAsync(command, groupTypeStr) == false) return;
                    groupType = (SubscribeGroupType)Convert.ToInt32(groupTypeStr);
                }
                else
                {
                    StepInfo stepInfo = await StepCache.CreateStepAsync(command);
                    if (stepInfo is null) return;
                    StepDetail uidStep = new StepDetail(60, " 请在60秒内发送要订阅用户的id", CheckUserIdAsync);
                    StepDetail groupStep = new StepDetail(60, $" 请在60秒内发送数字选择目标群：\r\n{EnumHelper.PixivSyncGroupOption()}", CheckSubscribeGroupAsync);
                    stepInfo.AddStep(uidStep);
                    stepInfo.AddStep(groupStep);
                    if (await stepInfo.HandleStep() == false) return;
                    userId = uidStep.Answer;
                    groupType = (SubscribeGroupType)Convert.ToInt32(groupStep.Answer);
                }

                MysResult<MysUserFullInfoDto> userInfoDto = await mysBusiness.geMysUserFullInfoDtoAsync(userId);
                if (userInfoDto is null || userInfoDto.retcode != 0)
                {
                    await command.ReplyGroupMessageWithAtAsync("订阅失败，目标用户不存在");
                    return;
                }

                long subscribeGroupId = groupType == SubscribeGroupType.All ? 0 : command.GroupId;
                SubscribePO dbSubscribe = subscribeBusiness.getSubscribe(userId, SubscribeType.米游社用户);
                if (dbSubscribe is null) dbSubscribe = subscribeBusiness.insertSurscribe(userInfoDto.data.user_info, userId);
                if (subscribeBusiness.isExistsSubscribeGroup(subscribeGroupId, dbSubscribe.Id))
                {
                    await command.ReplyGroupMessageWithAtAsync($"已订阅了该用户~");
                    return;
                }
                subscribeBusiness.insertSubscribeGroup(subscribeGroupId, dbSubscribe.Id);

                List<BaseContent> chailList = new List<BaseContent>();
                chailList.Add(new PlainContent($"米游社用户[{dbSubscribe.SubscribeName}]订阅成功!\r\n"));
                chailList.Add(new PlainContent($"目标群：{Enum.GetName(typeof(SubscribeGroupType), groupType)}\r\n"));
                chailList.Add(new PlainContent($"uid：{dbSubscribe.SubscribeCode}\r\n"));
                chailList.Add(new PlainContent($"签名：{dbSubscribe.SubscribeDescription}\r\n"));
                FileInfo fileInfo = string.IsNullOrEmpty(userInfoDto.data.user_info.avatar_url) ? null : await HttpHelper.DownImgAsync(userInfoDto.data.user_info.avatar_url);
                if (fileInfo != null) chailList.Add(new LocalImageContent(SendTarget.Group, fileInfo.FullName));
                await command.ReplyGroupMessageWithAtAsync(chailList);
                ConfigHelper.loadSubscribeTask();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "订阅米游社用户异常");
                Reporter.SendError(ex, "订阅米游社用户异常");
            }
        }

        /// <summary>
        /// 退订米游社用户
        /// </summary>
        /// <param name="e"></param>
        /// <param name="message"></param>
        /// <param name="isGroupSubscribe"></param>
        /// <returns></returns>
        public async Task cancleSubscribeMysUserAsync(GroupCommand command)
        {
            try
            {
                string userId = null;
                string paramStr = command.KeyWord;
                if (string.IsNullOrWhiteSpace(paramStr))
                {
                    StepInfo stepInfo = await StepCache.CreateStepAsync(command);
                    if (stepInfo is null) return;
                    StepDetail uidStep = new StepDetail(60, "请在60秒内发送要退订用户的id", CheckUserIdAsync);
                    stepInfo.AddStep(uidStep);
                    if (await stepInfo.HandleStep() == false) return;
                    userId = uidStep.Answer;
                }
                else
                {
                    userId = paramStr.Trim();
                    if (await CheckUserIdAsync(command, userId) == false) return;
                }

                List<SubscribePO> subscribeList = mysBusiness.getSubscribeList(userId);
                if (subscribeList is null || subscribeList.Count == 0)
                {
                    await command.ReplyGroupMessageWithAtAsync("并没有订阅这个用户哦~");
                    return;
                }

                foreach (var item in subscribeList)
                {
                    subscribeBusiness.delSubscribeGroup(item.Id);
                }

                await command.ReplyGroupMessageWithAtAsync($"已为所有群退订了id为{userId}的米游社用户~");
                ConfigHelper.loadSubscribeTask();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "退订米游社用户异常");
                Reporter.SendError(ex, "退订米游社用户异常");
            }
        }

        public async Task HandleUserSubscribeAsync()
        {
            SubscribeType subscribeType = SubscribeType.米游社用户;
            if (BotConfig.SubscribeTaskMap.ContainsKey(subscribeType) == false) return;
            List<SubscribeTask> subscribeTaskList = BotConfig.SubscribeTaskMap[subscribeType];
            if (subscribeTaskList is null || subscribeTaskList.Count == 0) return;
            foreach (SubscribeTask subscribeTask in subscribeTaskList)
            {
                try
                {
                    if (subscribeTask.SubscribeSubType != 0) continue;
                    List<MysSubscribe> mysSubscribeList = await mysBusiness.getMysUserSubscribeAsync(subscribeTask);
                    if (mysSubscribeList is null || mysSubscribeList.Count == 0) continue;
                    await sendGroupSubscribeAsync(subscribeTask, mysSubscribeList);
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

        private async Task sendGroupSubscribeAsync(SubscribeTask subscribeTask, List<MysSubscribe> mysSubscribeList)
        {
            foreach (MysSubscribe mysSubscribe in mysSubscribeList)
            {
                FileInfo fileInfo = null;
                string coverUrl = mysSubscribe.SubscribeRecord.CoverUrl;
                if (subscribeTask.GroupIdList is null || subscribeTask.GroupIdList.Count == 0) continue;

                List<BaseContent> msgList = new List<BaseContent>();
                msgList.Add(new PlainContent(mysBusiness.getPostInfoAsync(mysSubscribe, BotConfig.SubscribeConfig.Mihoyo.Template)));

                if (string.IsNullOrEmpty(coverUrl) == false)
                {
                    fileInfo = await HttpHelper.DownImgAsync(mysSubscribe.SubscribeRecord.CoverUrl);
                }
                if (fileInfo != null)
                {
                    msgList.Add(new LocalImageContent(SendTarget.Group, fileInfo.FullName));
                }


                foreach (long groupId in subscribeTask.GroupIdList)
                {
                    try
                    {
                        await Session.SendGroupMessageAsync(groupId, msgList.ToArray());
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(ex, "米游社订阅消息发送失败");
                    }
                    finally
                    {
                        await Task.Delay(1000);
                    }
                }
            }
        }

        private async Task<bool> CheckUserIdAsync(GroupCommand command, GroupRelay relay)
        {
            return await CheckUserIdAsync(command, relay.Message);
        }

        private async Task<bool> CheckSubscribeGroupAsync(GroupCommand command, GroupRelay relay)
        {
            return await CheckSubscribeGroupAsync(command, relay.Message);
        }

        private async Task<bool> CheckUserIdAsync(GroupCommand command, string value)
        {
            long userId = 0;
            if (string.IsNullOrWhiteSpace(value))
            {
                await command.ReplyGroupMessageWithAtAsync("用户id不可以为空");
                return false;
            }
            if (long.TryParse(value, out userId) == false)
            {
                await command.ReplyGroupMessageWithAtAsync("用户id必须为数字");
                return false;
            }
            if (userId <= 0)
            {
                await command.ReplyGroupMessageWithAtAsync("用户id无效");
                return false;
            }
            return true;
        }

        private async Task<bool> CheckSubscribeGroupAsync(GroupCommand command, string value)
        {
            int typeId = 0;
            if (string.IsNullOrWhiteSpace(value))
            {
                await command.ReplyGroupMessageWithAtAsync("未指定目标群");
                return false;
            }
            if (int.TryParse(value, out typeId) == false)
            {
                await command.ReplyGroupMessageWithAtAsync("目标必须为数字");
                return false;
            }
            if (Enum.IsDefined(typeof(SubscribeGroupType), typeId) == false)
            {
                await command.ReplyGroupMessageWithAtAsync("目标不在范围内");
                return false;
            }
            return true;
        }




    }
}
