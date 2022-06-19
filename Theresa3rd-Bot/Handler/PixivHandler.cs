using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Models;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Theresa3rd_Bot.Business;
using Theresa3rd_Bot.Cache;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Model.Cache;
using Theresa3rd_Bot.Model.Pixiv;
using Theresa3rd_Bot.Model.PO;
using Theresa3rd_Bot.Model.Subscribe;
using Theresa3rd_Bot.Type;
using Theresa3rd_Bot.Type.StepOption;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Handler
{
    public class PixivHandler
    {
        private PixivBusiness pixivBusiness;
        private SubscribeBusiness subscribeBusiness;

        public PixivHandler()
        {
            pixivBusiness = new PixivBusiness();
            subscribeBusiness = new SubscribeBusiness();
        }


        public async Task sendGeneralPixivImageAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string message)
        {
            try
            {
                long memberId = args.Sender.Id;
                long groupId = args.Sender.Group.Id;
                DateTime startDateTime = DateTime.Now;
                CoolingCache.SetHanding(groupId, memberId);//请求处理中

                if (string.IsNullOrWhiteSpace(BotConfig.SetuConfig.Pixiv.ProcessingMsg) == false)
                {
                    await session.SendTemplateWithAtAsync(args, BotConfig.SetuConfig.Pixiv.ProcessingMsg, null);
                    await Task.Delay(1000);
                }

                bool includeR18 = groupId.IsShowR18();
                PixivWorkInfoDto pixivWorkInfoDto = null;
                string tagName = message.splitKeyWord(BotConfig.SetuConfig.Pixiv.Command) ?? "";
                tagName = tagName.Replace("（", ")").Replace("）", ")");


                if (StringHelper.isPureNumber(tagName))
                {
                    if (await BusinessHelper.CheckSTCustomEnableAsync(session, args) == false) return;
                    pixivWorkInfoDto = await pixivBusiness.getPixivWorkInfoDtoAsync(tagName);//根据作品id获取作品
                }
                else if (string.IsNullOrEmpty(tagName) && BotConfig.SetuConfig.Pixiv.RandomMode == PixivRandomMode.随机订阅)
                {
                    pixivWorkInfoDto = await pixivBusiness.getRandomWorkInSubscribeAsync(groupId);//获取随机一个订阅中的画师的作品
                }
                else if (string.IsNullOrEmpty(tagName) && BotConfig.SetuConfig.Pixiv.RandomMode == PixivRandomMode.随机关注)
                {
                    pixivWorkInfoDto = await pixivBusiness.getRandomWorkInFollowAsync(groupId);//获取随机一个关注中的画师的作品
                }
                else if (string.IsNullOrEmpty(tagName) && BotConfig.SetuConfig.Pixiv.RandomMode == PixivRandomMode.随机收藏)
                {
                    pixivWorkInfoDto = await pixivBusiness.getRandomWorkInBookmarkAsync(groupId);//获取随机一个收藏中的作品
                }
                else if (string.IsNullOrEmpty(tagName))
                {
                    pixivWorkInfoDto = await pixivBusiness.getRandomWorkInTagsAsync(includeR18);//获取随机一个标签中的作品
                }
                else
                {
                    if (await BusinessHelper.CheckSTCustomEnableAsync(session, args) == false) return;
                    pixivWorkInfoDto = await pixivBusiness.getRandomWorkAsync(tagName, includeR18);//获取随机一个作品
                }

                if (pixivWorkInfoDto == null)
                {
                    await session.SendTemplateWithAtAsync(args, BotConfig.SetuConfig.Pixiv.NotFoundMsg, " 找不到这类型的图片或者收藏比过低，换个标签试试吧~");
                    return;
                }

                int todayLeftCount = BusinessHelper.GetSTLeftToday(session, args);
                FileInfo fileInfo = await pixivBusiness.downImgAsync(pixivWorkInfoDto);
                PixivWorkInfo pixivWorkInfo = pixivWorkInfoDto.body;

                int groupMsgId = 0;
                string template = BotConfig.SetuConfig.Pixiv.Template;
                List<IChatMessage> chatList = new List<IChatMessage>();
                if (string.IsNullOrWhiteSpace(template))
                {
                    StringBuilder warnBuilder = new StringBuilder();
                    if (BotConfig.PermissionsConfig.SetuNoneCDGroups.Contains(groupId) == false)
                    {
                        if (warnBuilder.Length > 0) warnBuilder.Append("，");
                        warnBuilder.Append($"{BotConfig.SetuConfig.MemberCD}秒后再来哦");
                    }
                    if (BotConfig.PermissionsConfig.SetuLimitlessGroups.Contains(groupId) == false || BotConfig.SetuConfig.MaxDaily > 0)
                    {
                        if (warnBuilder.Length > 0) warnBuilder.Append("，");
                        warnBuilder.Append($"今天剩余使用次数{todayLeftCount}次");
                    }
                    if (BotConfig.SetuConfig.RevokeInterval > 0)
                    {
                        if (warnBuilder.Length > 0) warnBuilder.Append("，");
                        warnBuilder.Append($"本消息将在{BotConfig.SetuConfig.RevokeInterval}秒后撤回，尽快保存哦");
                    }

                    chatList.Add(new PlainMessage(warnBuilder.ToString()));
                    chatList.Add(new PlainMessage(pixivBusiness.getDefaultWorkInfo(pixivWorkInfo, fileInfo, startDateTime)));
                }
                else
                {
                    chatList.Add(new PlainMessage(pixivBusiness.getWorkInfoWithLeft(pixivWorkInfo, fileInfo, startDateTime, todayLeftCount, template)));
                }

                try
                {
                    //发送群消息
                    List<IChatMessage> groupList = new List<IChatMessage>(chatList);
                    if (fileInfo == null)
                    {
                        groupList.AddRange(await session.SplitToChainAsync(BotConfig.GeneralConfig.DownErrorImg));
                    }
                    else if (pixivWorkInfoDto.body.isR18() == false)
                    {
                        groupList.Add((IChatMessage)await session.UploadPictureAsync(UploadTarget.Group, fileInfo.FullName));
                    }
                    else if (pixivWorkInfoDto.body.isR18() && groupId.IsShowR18Img())
                    {
                        groupList.Add((IChatMessage)await session.UploadPictureAsync(UploadTarget.Group, fileInfo.FullName));
                    }
                    groupMsgId = await session.SendMessageWithAtAsync(args, groupList);
                    await Task.Delay(1000);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex, "sendGeneralPixivImageAsync群消息发送失败");
                    throw;
                }


                if (BotConfig.SetuConfig.SendPrivate)
                {
                    try
                    {
                        //发送临时会话
                        List<IChatMessage> memberList = new List<IChatMessage>(chatList);
                        if (fileInfo == null)
                        {
                            memberList.AddRange(await session.SplitToChainAsync(BotConfig.GeneralConfig.DownErrorImg, UploadTarget.Temp));
                        }
                        else if (pixivWorkInfoDto.body.isR18() == false)
                        {
                            memberList.Add((IChatMessage)await session.UploadPictureAsync(UploadTarget.Temp, fileInfo.FullName));
                        }
                        else if (pixivWorkInfoDto.body.isR18() && groupId.IsShowR18Img())
                        {
                            memberList.Add((IChatMessage)await session.UploadPictureAsync(UploadTarget.Temp, fileInfo.FullName));
                        }
                        await session.SendTempMessageAsync(memberId, args.Sender.Group.Id, memberList.ToArray());
                        await Task.Delay(1000);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(ex, "临时消息发送失败");
                    }
                }

                //进入CD状态
                CoolingCache.SetMemberSTCooling(args.Sender.Group.Id, memberId);
                if (groupMsgId == 0 || BotConfig.SetuConfig.RevokeInterval == 0) return;

                try
                {
                    //等待撤回
                    await Task.Delay(BotConfig.SetuConfig.RevokeInterval * 1000);
                    await session.RevokeMessageAsync(groupMsgId);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex, "sendGeneralPixivImageAsync消息撤回失败");
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "sendGeneralPixivImageAsync异常");
                await session.SendTemplateWithAtAsync(args, BotConfig.SetuConfig.Pixiv.ErrorMsg, " 获取图片出错了，再试一次吧~");
            }
            finally
            {
                CoolingCache.SetHandFinish(args.Sender.Group.Id, args.Sender.Id);//请求处理完成
            }
        }


        /*-------------------------------------------------------------订阅相关--------------------------------------------------------------------------*/

        /// <summary>
        /// 订阅pixiv画师
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task subscribeUserAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string message)
        {
            try
            {
                string pixivUserIds = null;
                SubscribeGroupType? groupType = null;
                long memberId = args.Sender.Id;
                long groupId = args.Sender.Group.Id;

                string[] paramArr = message.splitParam(BotConfig.SubscribeConfig.PixivUser.AddCommand);
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
                    if (stepInfo == null) return;
                    StepDetail uidStep = new StepDetail(60, " 请在60秒内发送要订阅用户的id，多个id之间可以用逗号或者换行隔开", CheckPixivUserIdsAsync);
                    StepDetail groupStep = new StepDetail(60, $" 请在60秒内发送数字选择目标群：\r\n{EnumHelper.PixivSyncGroupOption()}", CheckSubscribeGroupAsync);
                    stepInfo.AddStep(uidStep);
                    stepInfo.AddStep(groupStep);
                    if (await stepInfo.StartStep(session, args) == false) return;
                    pixivUserIds = uidStep.Answer;
                    groupType = (SubscribeGroupType)Convert.ToInt32(groupStep.Answer);
                }

                string[] pixivUserIdArr = pixivUserIds.splitParams();
                if (pixivUserIdArr.Length > 1)
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" 检测到多个id，开始批量订阅~"));
                    await Task.Delay(1000);
                }

                foreach (var item in pixivUserIdArr)
                {
                    string pixivUserId = item.Trim();
                    try
                    {
                        SubscribePO dbSubscribe = subscribeBusiness.getSubscribe(pixivUserId, SubscribeType.P站画师);
                        if (dbSubscribe == null)
                        {
                            //添加订阅
                            PixivUserInfoDto pixivUserInfoDto = await pixivBusiness.getPixivUserInfoDtoAsync(pixivUserId);
                            dbSubscribe = subscribeBusiness.insertSurscribe(pixivUserInfoDto, pixivUserId);
                        }

                        long subscribeGroupId = groupType == SubscribeGroupType.All ? 0 : args.Sender.Group.Id;
                        if (subscribeBusiness.isExistsSubscribeGroup(subscribeGroupId, dbSubscribe.Id))
                        {
                            //关联订阅
                            await session.SendMessageWithAtAsync(args, new PlainMessage($" 画师id[{pixivUserId}]已经被订阅了~"));
                            continue;
                        }

                        SubscribeGroupPO subscribeGroup = subscribeBusiness.insertSubscribeGroup(subscribeGroupId, dbSubscribe.Id);
                        await session.SendMessageWithAtAsync(args, new PlainMessage($"画师id[{dbSubscribe.SubscribeCode}]订阅成功，正在读取最新作品~"));

                        await Task.Delay(1000);
                        await sendPixivUserNewestWorkAsync(session, args, dbSubscribe, groupId.IsShowR18(), groupId.IsShowR18Img());
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(ex, $"pixiv画师[{pixivUserId}]订阅异常");
                        await session.SendMessageWithAtAsync(args, new PlainMessage($" 画师id[{pixivUserId}]订阅失败~"));
                    }
                    finally
                    {
                        Thread.Sleep(2000);
                    }
                }
                if (pixivUserIdArr.Length > 1)
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" 所有画师订阅完毕"));
                }
                ConfigHelper.loadSubscribeTask();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "订阅功能异常");
                throw;
            }
        }


        /// <summary>
        /// 同步pixiv画师
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task subscribeFollowUserAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string message)
        {
            try
            {
                long memberId = args.Sender.Id;
                long groupId = args.Sender.Group.Id;

                StepInfo stepInfo = await StepCache.CreateStepAsync(session, args);
                if (stepInfo == null) return;

                StepDetail modeStep = new StepDetail(60, $" 请在60秒内发送数字选择模式：\r\n{EnumHelper.PixivSyncModeOption()}", CheckSyncModeAsync);
                StepDetail groupStep = new StepDetail(60, $" 请在60秒内发送数字选择目标群：\r\n{EnumHelper.PixivSyncGroupOption()}", CheckSubscribeGroupAsync);
                stepInfo.AddStep(modeStep);
                stepInfo.AddStep(groupStep);
                if (await stepInfo.StartStep(session, args) == false) return;

                PixivSyncModeType syncMode = (PixivSyncModeType)Convert.ToInt32(modeStep.Answer);
                SubscribeGroupType syncGroup = (SubscribeGroupType)Convert.ToInt32(groupStep.Answer);

                await session.SendMessageWithAtAsync(args, new PlainMessage(" 正在获取pixiv账号中已关注的画师列表..."));
                await Task.Delay(1000);

                List<PixivFollowUser> followUserList = await pixivBusiness.getFollowUserList();
                if (followUserList == null || followUserList.Count == 0)
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" pixiv账号还没有关注任何画师"));
                    return;
                }

                await session.SendMessageWithAtAsync(args, new PlainMessage($" 已获取{followUserList.Count}个画师，正在录入数据..."));
                await Task.Delay(1000);

                //插入Subscribe数据
                DateTime syncDate = DateTime.Now;
                DbScoped.SugarScope.BeginTran();//开始事务
                List<SubscribePO> dbSubscribeList = new List<SubscribePO>();

                foreach (var item in followUserList)
                {
                    SubscribePO dbSubscribe = subscribeBusiness.getSubscribe(item.userId, SubscribeType.P站画师);
                    if (dbSubscribe == null) dbSubscribe = subscribeBusiness.insertSurscribe(item, syncDate);
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
                        if (subscribeGroup == null) subscribeBusiness.insertSubscribeGroup(subscribeGroupId, item.Id);
                    }
                }
                DbScoped.SugarScope.CommitTran();//提交事务

                await session.SendMessageWithAtAsync(args, new PlainMessage(" 关注画师订阅完毕"));
                ConfigHelper.loadSubscribeTask();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "关注画师订阅功能异常");
                DbScoped.SugarScope.RollbackTran();//事务回滚
                throw;
            }
        }


        /// <summary>
        /// 取消订阅pixiv画师
        /// </summary>
        /// <param name="e"></param>
        /// <param name="message"></param>
        /// <param name="isGroupSubscribe"></param>
        /// <returns></returns>
        public async Task cancleSubscribeUserAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string message)
        {
            try
            {
                string pixivUserIds = null;
                long memberId = args.Sender.Id;
                long groupId = args.Sender.Group.Id;

                string[] paramArr = message.splitParam(BotConfig.SubscribeConfig.PixivUser.RmCommand);
                if (paramArr != null && paramArr.Length >= 1)
                {
                    pixivUserIds = paramArr.Length > 0 ? paramArr[0] : null;
                    if (await CheckPixivUserIdsAsync(session, args, pixivUserIds) == false) return;
                }
                else
                {
                    StepInfo stepInfo = await StepCache.CreateStepAsync(session, args);
                    if (stepInfo == null) return;
                    StepDetail uidStep = new StepDetail(60, " 请在60秒内发送要退订用户的id，多个id之间可以用逗号或者换行隔开", CheckPixivUserIdsAsync);
                    stepInfo.AddStep(uidStep);
                    if (await stepInfo.StartStep(session, args) == false) return;
                    pixivUserIds = uidStep.Answer;
                }

                string[] pixivUserIdArr = pixivUserIds.splitParams();
                foreach (string pixivUserId in pixivUserIdArr)
                {
                    SubscribePO dbSubscribe = subscribeBusiness.getSubscribe(pixivUserId, SubscribeType.P站画师);
                    if (dbSubscribe == null)
                    {
                        await session.SendMessageWithAtAsync(args, new PlainMessage(" 退订失败，这个订阅不存在"));
                        return;
                    }
                    subscribeBusiness.delSubscribeGroup(dbSubscribe.Id);
                }

                await session.SendMessageWithAtAsync(args, new PlainMessage($" 已为所有群退订了相关用户~"));
                ConfigHelper.loadSubscribeTask();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "取消订阅异常");
                throw;
            }
        }

        /// <summary>
        /// 订阅pixiv标签
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task subscribeTagAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string message)
        {
            try
            {
                string pixivTag = null;
                SubscribeGroupType? groupType = null;
                long memberId = args.Sender.Id;
                long groupId = args.Sender.Group.Id;

                string[] paramArr = message.splitParam(BotConfig.SubscribeConfig.PixivTag.AddCommand);
                if (paramArr != null && paramArr.Length >= 2)
                {
                    pixivTag = paramArr.Length > 0 ? paramArr[0] : null;
                    string groupTypeStr = paramArr.Length > 1 ? paramArr[1] : null;
                    if (await CheckPixivTagAsync(session, args, pixivTag) == false) return;
                    if (await CheckSubscribeGroupAsync(session, args, groupTypeStr) == false) return;
                    groupType = (SubscribeGroupType)Convert.ToInt32(groupTypeStr);
                }
                else
                {
                    StepInfo stepInfo = await StepCache.CreateStepAsync(session, args);
                    if (stepInfo == null) return;
                    StepDetail tagStep = new StepDetail(60, " 请在60秒内发送要订阅的标签名", CheckPixivTagAsync);
                    StepDetail groupStep = new StepDetail(60, $" 请在60秒内发送数字选择目标群：\r\n{EnumHelper.PixivSyncGroupOption()}", CheckSubscribeGroupAsync);
                    stepInfo.AddStep(tagStep);
                    stepInfo.AddStep(groupStep);
                    if (await stepInfo.StartStep(session, args) == false) return;
                    pixivTag = tagStep.Answer;
                    groupType = (SubscribeGroupType)Convert.ToInt32(groupStep.Answer);
                }

                PixivSearchDto pageOne = await pixivBusiness.getPixivSearchDtoAsync(pixivTag, 1, false);
                if (pageOne == null || pageOne.body.getIllust().data.Count == 0)
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" 该标签中没有任何作品，订阅失败"));
                    return;
                }

                SubscribePO dbSubscribe = subscribeBusiness.getSubscribe(pixivTag, SubscribeType.P站标签);
                if (dbSubscribe == null) dbSubscribe = subscribeBusiness.insertSurscribe(pixivTag);

                long subscribeGroupId = groupType == SubscribeGroupType.All ? 0 : groupId;
                if (subscribeBusiness.isExistsSubscribeGroup(subscribeGroupId, dbSubscribe.Id))
                {
                    //关联订阅
                    await session.SendMessageWithAtAsync(args, new PlainMessage($" 这个标签已经被订阅了~"));
                    return;
                }

                SubscribeGroupPO subscribeGroup = subscribeBusiness.insertSubscribeGroup(subscribeGroupId, dbSubscribe.Id);
                await session.SendMessageWithAtAsync(args, new PlainMessage($" 标签[{pixivTag}]订阅成功,该标签总作品数为:{pageOne.body.illust.total}"));
                ConfigHelper.loadSubscribeTask();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "订阅功能异常");
                throw;
            }
        }

        /// <summary>
        /// 取消订阅pixiv标签
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task cancleSubscribeTagAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string message)
        {
            try
            {
                string pixivTag = null;
                long memberId = args.Sender.Id;
                long groupId = args.Sender.Group.Id;

                string[] paramArr = message.splitParam(BotConfig.SubscribeConfig.PixivTag.RmCommand);
                if (paramArr != null && paramArr.Length >= 1)
                {
                    pixivTag = paramArr.Length > 0 ? paramArr[0] : null;
                    if (await CheckPixivTagAsync(session, args, pixivTag) == false) return;
                }
                else
                {
                    StepInfo stepInfo = await StepCache.CreateStepAsync(session, args);
                    if (stepInfo == null) return;
                    StepDetail tagStep = new StepDetail(60, " 请在60秒内发送要退订的标签名", CheckPixivTagAsync);
                    stepInfo.AddStep(tagStep);
                    if (await stepInfo.StartStep(session, args) == false) return;
                    pixivTag = tagStep.Answer;
                }

                SubscribePO dbSubscribe = subscribeBusiness.getSubscribe(pixivTag, SubscribeType.P站标签);
                if (dbSubscribe == null)
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" 退订失败，这个订阅不存在"));
                    return;
                }

                subscribeBusiness.delSubscribeGroup(dbSubscribe.Id);
                await session.SendMessageWithAtAsync(args, new PlainMessage($" 已为所有群退订了相关标签~"));
                ConfigHelper.loadSubscribeTask();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "取消订阅异常");
                throw;
            }
        }

        /// <summary>
        /// 发送画师的最新作品
        /// </summary>
        /// <param name="session"></param>
        /// <param name="userId"></param>
        /// <param name="subscribeId"></param>
        /// <returns></returns>
        public async Task sendPixivUserNewestWorkAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, SubscribePO dbSubscribe, bool includeR18, bool includeR18Img)
        {
            try
            {
                DateTime startTime = DateTime.Now;
                List<IChatMessage> chatList = new List<IChatMessage>();
                List<PixivSubscribe> pixivSubscribeList = await pixivBusiness.getPixivUserNewestAsync(dbSubscribe.SubscribeCode, dbSubscribe.Id);
                if (pixivSubscribeList == null || pixivSubscribeList.Count == 0)
                {
                    await session.SendGroupMessageAsync(args.Sender.Group.Id, new PlainMessage($"画师[{dbSubscribe.SubscribeName}]还没有发布任何作品~"));
                    return;
                }

                PixivSubscribe pixivSubscribe = pixivSubscribeList.First();
                if (pixivSubscribe.PixivWorkInfoDto.body.isR18() && includeR18 == false)
                {
                    await session.SendGroupMessageAsync(args.Sender.Group.Id, new PlainMessage(" 该作品为R-18作品，根据设置不显示相关内容"));
                    return;
                }

                FileInfo fileInfo = await pixivBusiness.downImgAsync(pixivSubscribe.PixivWorkInfoDto);
                chatList.Add(new PlainMessage($"pixiv画师[{pixivSubscribe.PixivWorkInfoDto.body.userName}]的最新作品："));
                chatList.Add(new PlainMessage(pixivBusiness.getDefaultWorkInfo(pixivSubscribe.PixivWorkInfoDto.body, fileInfo, startTime)));

                if (fileInfo == null)
                {
                    chatList.AddRange(await session.SplitToChainAsync(BotConfig.GeneralConfig.DownErrorImg));
                }
                else if (pixivSubscribe.PixivWorkInfoDto.body.isR18() == false)
                {
                    chatList.Add((IChatMessage)await session.UploadPictureAsync(UploadTarget.Group, fileInfo.FullName));
                }
                else if (pixivSubscribe.PixivWorkInfoDto.body.isR18() && includeR18Img)
                {
                    chatList.Add((IChatMessage)await session.UploadPictureAsync(UploadTarget.Group, fileInfo.FullName));
                }

                await session.SendMessageWithAtAsync(args, chatList);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "读取画师最新作品时出现异常");
                await session.SendGroupMessageAsync(args.Sender.Group.Id, new PlainMessage($"读取画师[{dbSubscribe.SubscribeName}]的最新作品失败~"));
            }
        }

        private async Task<bool> CheckPixivTagAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                await session.SendMessageWithAtAsync(args, new PlainMessage(" 标签不可以为空"));
                return false;
            }
            return true;
        }

        private async Task<bool> CheckPixivUserIdsAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                await session.SendMessageWithAtAsync(args, new PlainMessage(" 用户id不可以为空"));
                return false;
            }

            string[] pixivUserIdArr = value.splitParams();
            if (pixivUserIdArr.Length == 0)
            {
                await session.SendMessageWithAtAsync(args, new PlainMessage(" 没有检测到用户id"));
                return false;
            }

            foreach (var userIdStr in pixivUserIdArr)
            {
                long userId = 0;
                if (long.TryParse(userIdStr, out userId) == false)
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage($" 用户id{userIdStr}必须为数字"));
                    return false;
                }
                if (userId <= 0)
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage($" 用户id{userIdStr}无效"));
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
                await session.SendMessageWithAtAsync(args, new PlainMessage(" 模式必须为数字"));
                return false;
            }
            if (Enum.IsDefined(typeof(PixivSyncModeType), modeId) == false)
            {
                await session.SendMessageWithAtAsync(args, new PlainMessage(" 模式不在范围内"));
                return false;
            }
            return true;
        }

        private async Task<bool> CheckSubscribeGroupAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string value)
        {
            int typeId = 0;
            if (int.TryParse(value, out typeId) == false)
            {
                await session.SendMessageWithAtAsync(args, new PlainMessage(" 必须为数字"));
                return false;
            }
            if (Enum.IsDefined(typeof(SubscribeGroupType), typeId) == false)
            {
                await session.SendMessageWithAtAsync(args, new PlainMessage(" 不在范围内"));
                return false;
            }
            return true;
        }


    }
}
