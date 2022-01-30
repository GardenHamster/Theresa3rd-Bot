using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Dao;
using Theresa3rd_Bot.Model.Pixiv;
using Theresa3rd_Bot.Model.PO;
using Theresa3rd_Bot.Model.Subscribe;
using Theresa3rd_Bot.Type;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Business
{
    public class SubscribeBusiness
    {
        private SubscribeDao subscribeDao;
        private SubscribeGroupDao subscribeGroupDao;

        public SubscribeBusiness()
        {
            subscribeDao = new SubscribeDao();
            subscribeGroupDao = new SubscribeGroupDao();
        }

        /// <summary>
        /// 获取订阅任务
        /// </summary>
        /// <returns></returns>
        public Dictionary<SubscribeType, List<SubscribeTask>> getSubscribeTask()
        {
            Dictionary<SubscribeType, List<SubscribeTask>> subscribeTaskMap = new Dictionary<SubscribeType, List<SubscribeTask>>();
            List<SubscribeInfo> subscribeInfoList = subscribeDao.getSubscribeInfo();
            foreach (SubscribeInfo subscribeInfo in subscribeInfoList)
            {
                SubscribeType subscribeType = subscribeInfo.SubscribeType;
                if (!subscribeTaskMap.ContainsKey(subscribeType)) subscribeTaskMap[subscribeType] = new List<SubscribeTask>();
                List<SubscribeTask> subscribeTaskList = subscribeTaskMap[subscribeType];
                SubscribeTask subscribeTask = subscribeTaskList.Where(o => o.SubscribeInfo.SubscribeCode == subscribeInfo.SubscribeCode).FirstOrDefault();
                if (subscribeTask == null)
                {
                    subscribeTask = new SubscribeTask(subscribeInfo);
                    subscribeTaskList.Add(subscribeTask);
                }
                if (subscribeTask.GroupIdList.Contains(subscribeInfo.GroupId) == false)
                {
                    subscribeTask.GroupIdList.Add(subscribeInfo.GroupId);
                }
            }
            return subscribeTaskMap;
        }


        /// <summary>
        /// 订阅pixiv画师
        /// </summary>
        /// <param name="e"></param>
        /// <param name="message"></param>
        /// <param name="isGroupSubscribe"></param>
        /// <returns></returns>
        public async Task subscribePixivUserAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string message)
        {
            try
            {
                string pixivUserIds = message.splitKeyWord(BotConfig.SubscribeConfig.PixivUser.AddCommand);
                if (pixivUserIds == null) pixivUserIds = "";
                string[] pixivUserIdArr = pixivUserIds.Split(new string[] { ",", "，", "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (pixivUserIdArr == null || pixivUserIdArr.Length == 0)
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" 没有检测到要订阅的画师id，请确保指令格式正确"));
                    return;
                }
                if (pixivUserIdArr.Length > 1)
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" 检测到多个id，开始批量订阅~"));
                }

                PixivBusiness pixivBusiness = new PixivBusiness();
                foreach (var item in pixivUserIdArr)
                {
                    string pixivUserId = item.Trim();
                    if (StringHelper.isPureNumber(pixivUserId) == false)
                    {
                        await session.SendMessageWithAtAsync(args, new PlainMessage($" 画师ID[{pixivUserId}]格式不正确"));
                        continue;
                    }
                    try
                    {
                        SubscribePO dbSubscribe = subscribeDao.getSubscribe(pixivUserId, SubscribeType.P站画师);
                        if (dbSubscribe == null)
                        {
                            //添加订阅
                            PixivUserInfoDto pixivUserInfoDto = pixivBusiness.getPixivUserInfoDto(pixivUserId);
                            dbSubscribe = new SubscribePO();
                            dbSubscribe.SubscribeCode = pixivUserId;
                            dbSubscribe.SubscribeName = StringHelper.filterEmoji(pixivUserInfoDto.body.extraData.meta.title.Replace("- pixiv", "").Trim());
                            dbSubscribe.SubscribeDescription = dbSubscribe.SubscribeName;
                            dbSubscribe.SubscribeType = SubscribeType.P站画师;
                            dbSubscribe.Isliving = false;
                            dbSubscribe.CreateDate = DateTime.Now;
                            dbSubscribe = subscribeDao.Insert(dbSubscribe);
                        }

                        if (subscribeGroupDao.getCountBySubscribe(args.Sender.Group.Id, dbSubscribe.Id) > 0)
                        {
                            //关联订阅
                            await session.SendMessageWithAtAsync(args, new PlainMessage($" 画师id[{pixivUserId}]已经被订阅了~"));
                            continue;
                        }


                        SubscribeGroupPO subscribeGroup = new SubscribeGroupPO();
                        subscribeGroup.GroupId = args.Sender.Group.Id;
                        subscribeGroup.SubscribeId = dbSubscribe.Id;
                        subscribeGroup = subscribeGroupDao.Insert(subscribeGroup);


                        List<IChatMessage> chatList = new List<IChatMessage>();
                        List<IChatMessage> workChatList = await pixivBusiness.getPixivNewestWorkAsync(session, dbSubscribe.SubscribeCode, dbSubscribe.Id);
                        if (workChatList == null || workChatList.Count == 0)
                        {
                            chatList.Add(new PlainMessage($"画师id[{dbSubscribe.SubscribeCode}]订阅成功，该画师还没有任何作品~"));
                        }
                        else
                        {
                            chatList.Add(new PlainMessage($"画师id[{dbSubscribe.SubscribeCode}]订阅成功，以下是最新作品\r\n"));
                            chatList.AddRange(workChatList);
                        }

                        await session.SendMessageWithAtAsync(args, chatList);
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
        /// 取消订阅pixiv画师
        /// </summary>
        /// <param name="e"></param>
        /// <param name="message"></param>
        /// <param name="isGroupSubscribe"></param>
        /// <returns></returns>
        public async Task cancleSubscribePixivUserAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string message)
        {
            try
            {
                string keyWord = message.splitKeyWord(BotConfig.SubscribeConfig.PixivUser.RmCommand);
                if (string.IsNullOrEmpty(keyWord))
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" 没有检测到要退订的关键词，请确保指令格式正确"));
                    return;
                }
                if (StringHelper.isPureNumber(keyWord) == false)
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" 没有检测到画师ID，请确保指令格式正确"));
                    return;
                }
                SubscribePO dbSubscribe = subscribeDao.getSubscribe(keyWord, SubscribeType.P站画师);
                if (dbSubscribe == null)
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" 退订失败，这个订阅不存在"));
                    return;
                }
                bool isGroupSubscribed = subscribeGroupDao.getCountBySubscribe(args.Sender.Group.Id, dbSubscribe.Id) > 0;
                if (isGroupSubscribed == false)
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" 并没有订阅这个画师哦~"));
                    return;
                }
                int successCount = subscribeGroupDao.delSubscribe(args.Sender.Group.Id, dbSubscribe.Id);
                if (successCount == 0)
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" 退订失败"));
                    return;
                }
                await session.SendMessageWithAtAsync(args, new PlainMessage(" 退订成功，以后不会再推送这个画师的作品了哦~"));
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
        public async Task subscribePixivTagAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string message)
        {
            try
            {
                string pixivTag = message.splitKeyWord(BotConfig.SubscribeConfig.PixivTag.AddCommand);
                if (string.IsNullOrWhiteSpace(pixivTag))
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" 没有检测到要订阅的标签，请确保指令格式正确"));
                    return;
                }
                PixivBusiness pixivBusiness = new PixivBusiness();
                PixivSearchDto pageOne = pixivBusiness.getPixivSearchDto(pixivTag, 1, false);
                if (pageOne == null || pageOne.body.getIllust().data.Count == 0)
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" 该标签中没有任何作品，订阅失败"));
                    return;
                }

                SubscribePO dbSubscribe = subscribeDao.getSubscribe(pixivTag, SubscribeType.P站标签);
                if (dbSubscribe == null)
                {
                    //添加订阅
                    dbSubscribe = new SubscribePO();
                    dbSubscribe.SubscribeCode = pixivTag;
                    dbSubscribe.SubscribeName = pixivTag;
                    dbSubscribe.SubscribeDescription = pixivTag;
                    dbSubscribe.SubscribeType = SubscribeType.P站标签;
                    dbSubscribe.Isliving = false;
                    dbSubscribe.CreateDate = DateTime.Now;
                    dbSubscribe = subscribeDao.Insert(dbSubscribe);
                }

                if (subscribeGroupDao.getCountBySubscribe(args.Sender.Group.Id, dbSubscribe.Id) > 0)
                {
                    //关联订阅
                    await session.SendMessageWithAtAsync(args, new PlainMessage($" 这个标签已经被订阅了~"));
                    return;
                }

                SubscribeGroupPO subscribeGroup = new SubscribeGroupPO();
                subscribeGroup.GroupId = args.Sender.Group.Id;
                subscribeGroup.SubscribeId = dbSubscribe.Id;
                subscribeGroup = subscribeGroupDao.Insert(subscribeGroup);
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
        public async Task cancleSubscribePixivTagAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string message)
        {
            try
            {
                string keyWord = message.splitKeyWord(BotConfig.SubscribeConfig.PixivTag.RmCommand);
                if (string.IsNullOrEmpty(keyWord))
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" 没有检测到要退订的关键词，请确保指令格式正确"));
                    return;
                }

                SubscribePO dbSubscribe = subscribeDao.getSubscribe(keyWord, SubscribeType.P站标签);
                if (dbSubscribe == null)
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" 退订失败，这个订阅不存在"));
                    return;
                }
                bool isGroupSubscribed = subscribeGroupDao.getCountBySubscribe(args.Sender.Group.Id, dbSubscribe.Id) > 0;
                if (isGroupSubscribed == false)
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" 并没有订阅这个标签哦~"));
                    return;
                }
                int successCount = subscribeGroupDao.delSubscribe(args.Sender.Group.Id, dbSubscribe.Id);
                if (successCount == 0)
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" 退订失败"));
                    return;
                }
                await session.SendMessageWithAtAsync(args, new PlainMessage(" 退订成功，以后不会再推送这个标签的作品了哦~"));
                ConfigHelper.loadSubscribeTask();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "取消订阅异常");
                throw;
            }
        }


    }
}
