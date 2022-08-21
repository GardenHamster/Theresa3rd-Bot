using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Theresa3rd_Bot.Business;
using Theresa3rd_Bot.Cache;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Model.Cache;
using Theresa3rd_Bot.Model.Mys;
using Theresa3rd_Bot.Model.PO;
using Theresa3rd_Bot.Type;
using Theresa3rd_Bot.Type.StepOption;
using Theresa3rd_Bot.Util;


namespace Theresa3rd_Bot.Handler
{
    public class MYSHandler
    {
        private MYSBusiness mysBusiness;
        private SubscribeBusiness subscribeBusiness;

        public MYSHandler()
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
        public async Task subscribeMYSUserAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string message)
        {
            try
            {
                string userId = null;
                SubscribeGroupType? groupType = null;
                string[] paramArr = message.splitParams(BotConfig.SubscribeConfig.Mihoyo.AddCommand);
                if (paramArr != null && paramArr.Length >= 2)
                {
                    userId = paramArr.Length > 0 ? paramArr[0] : null;
                    string groupTypeStr = paramArr.Length > 1 ? paramArr[1] : null;
                    if (await CheckUserIdAsync(session, args, userId) == false) return;
                    if (await CheckSubscribeGroupAsync(session, args, groupTypeStr) == false) return;
                    groupType = (SubscribeGroupType)Convert.ToInt32(groupTypeStr);
                }
                else
                {
                    StepInfo stepInfo = await StepCache.CreateStepAsync(session, args);
                    if (stepInfo == null) return;
                    StepDetail uidStep = new StepDetail(60, " 请在60秒内发送要订阅用户的id", CheckUserIdAsync);
                    StepDetail groupStep = new StepDetail(60, $" 请在60秒内发送数字选择目标群：\r\n{EnumHelper.PixivSyncGroupOption()}", CheckSubscribeGroupAsync);
                    stepInfo.AddStep(uidStep);
                    stepInfo.AddStep(groupStep);
                    if (await stepInfo.HandleStep(session, args) == false) return;
                    userId = uidStep.Answer;
                    groupType = (SubscribeGroupType)Convert.ToInt32(groupStep.Answer);
                }

                MysResult<MysUserFullInfoDto> userInfoDto = await mysBusiness.geMysUserFullInfoDtoAsync(userId);
                if (userInfoDto == null || userInfoDto.retcode != 0)
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" 订阅失败，目标用户不存在"));
                    return;
                }

                long subscribeGroupId = groupType == SubscribeGroupType.All ? 0 : args.Sender.Group.Id;
                SubscribePO dbSubscribe = subscribeBusiness.getSubscribe(userId, SubscribeType.米游社用户);
                if (dbSubscribe == null) dbSubscribe = subscribeBusiness.insertSurscribe(userInfoDto.data.user_info, userId);
                if (subscribeBusiness.isExistsSubscribeGroup(subscribeGroupId, dbSubscribe.Id))
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage($" 已订阅了该用户~"));
                    return;
                }
                subscribeBusiness.insertSubscribeGroup(subscribeGroupId, dbSubscribe.Id);

                List<IChatMessage> chailList = new List<IChatMessage>();
                chailList.Add(new PlainMessage($"米游社用户[{dbSubscribe.SubscribeName}]订阅成功!\r\n"));
                chailList.Add(new PlainMessage($"目标群：{Enum.GetName(typeof(SubscribeGroupType), groupType)}\r\n"));
                chailList.Add(new PlainMessage($"uid：{dbSubscribe.SubscribeCode}\r\n"));
                chailList.Add(new PlainMessage($"签名：{dbSubscribe.SubscribeDescription}\r\n"));
                FileInfo fileInfo = string.IsNullOrEmpty(userInfoDto.data.user_info.avatar_url) ? null : await HttpHelper.DownImgAsync(userInfoDto.data.user_info.avatar_url);
                if (fileInfo != null) chailList.Add((IChatMessage)await MiraiHelper.Session.UploadPictureAsync(UploadTarget.Group, fileInfo.FullName));
                await session.SendMessageWithAtAsync(args, chailList);
                ConfigHelper.loadSubscribeTask();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "订阅米游社用户异常");
                throw;
            }
        }

        /// <summary>
        /// 退订米游社用户
        /// </summary>
        /// <param name="e"></param>
        /// <param name="message"></param>
        /// <param name="isGroupSubscribe"></param>
        /// <returns></returns>
        public async Task cancleSubscribeMysUserAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string message)
        {
            try
            {
                string userId = null;
                string paramStr = message.splitParam(BotConfig.SubscribeConfig.Mihoyo.RmCommand);
                if (string.IsNullOrWhiteSpace(paramStr))
                {
                    StepInfo stepInfo = await StepCache.CreateStepAsync(session, args);
                    if (stepInfo == null) return;
                    StepDetail uidStep = new StepDetail(60, " 请在60秒内发送要退订用户的id", CheckUserIdAsync);
                    stepInfo.AddStep(uidStep);
                    if (await stepInfo.HandleStep(session, args) == false) return;
                    userId = uidStep.Answer;
                }
                else
                {
                    userId = paramStr.Trim();
                    if (await CheckUserIdAsync(session, args, userId) == false) return;
                }

                List<SubscribePO> subscribeList = mysBusiness.getSubscribeList(userId);
                if (subscribeList == null || subscribeList.Count == 0)
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" 并没有订阅这个用户哦~"));
                    return;
                }

                foreach (var item in subscribeList)
                {
                    subscribeBusiness.delSubscribeGroup(item.Id);
                }

                await session.SendMessageWithAtAsync(args, new PlainMessage($" 已为所有群退订了id为{userId}的米游社用户~"));
                ConfigHelper.loadSubscribeTask();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "退订米游社用户异常");
                throw;
            }
        }

        private async Task<bool> CheckUserIdAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string value)
        {
            long userId = 0;
            if (string.IsNullOrWhiteSpace(value))
            {
                await session.SendMessageWithAtAsync(args, new PlainMessage(" 用户id不可以为空"));
                return false;
            }
            if (long.TryParse(value, out userId) == false)
            {
                await session.SendMessageWithAtAsync(args, new PlainMessage(" 用户id必须为数字"));
                return false;
            }
            if (userId <= 0)
            {
                await session.SendMessageWithAtAsync(args, new PlainMessage(" 用户id无效"));
                return false;
            }
            return true;
        }

        private async Task<bool> CheckSubscribeGroupAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string value)
        {
            int typeId = 0;
            if (string.IsNullOrWhiteSpace(value))
            {
                await session.SendMessageWithAtAsync(args, new PlainMessage(" 未指定目标群"));
                return false;
            }
            if (int.TryParse(value, out typeId) == false)
            {
                await session.SendMessageWithAtAsync(args, new PlainMessage(" 目标必须为数字"));
                return false;
            }
            if (Enum.IsDefined(typeof(SubscribeGroupType), typeId) == false)
            {
                await session.SendMessageWithAtAsync(args, new PlainMessage(" 目标不在范围内"));
                return false;
            }
            return true;
        }




    }
}
