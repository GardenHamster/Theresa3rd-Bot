using AnimatedGif;
using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Theresa3rd_Bot.Cache;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Dao;
using Theresa3rd_Bot.Model.Pixiv;
using Theresa3rd_Bot.Model.PO;
using Theresa3rd_Bot.Model.Subscribe;
using Theresa3rd_Bot.Type;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Business
{
    public class PixivBusiness
    {
        private SubscribeDao subscribeDao;
        private SubscribeGroupDao subscribeGroupDao;
        private SubscribeRecordDao subscribeRecordDao;

        /// <summary>
        /// p站每页作品数
        /// </summary>
        private const int pixivPageSize = 60;

        /// <summary>
        /// 每次读取多少条画师的作品
        /// </summary>
        private const int PixivNewestRead = 2;

        /// <summary>
        /// 获取作品信息的线程数
        /// </summary>
        private const int threadCount = 3;

        /// <summary>
        /// 每条线程读取作品数
        /// </summary>
        private const int workEachThread = 30;

        /// <summary>
        /// 每条线程读取作品数
        /// </summary>
        private const int workScreenMoreThen = 300;

        /// <summary>
        /// 收藏数超过0的作品集
        /// </summary>
        private List<PixivWorkInfoDto> bookUpList;

        public PixivBusiness()
        {
            bookUpList = new List<PixivWorkInfoDto>();
            subscribeDao = new SubscribeDao();
            subscribeGroupDao = new SubscribeGroupDao();
            subscribeRecordDao = new SubscribeRecordDao();
        }

        /*----------------------------------------------------涩图指令----------------------------------------------------------------------------*/

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
                    pixivWorkInfoDto = await getPixivWorkInfoDtoAsync(tagName);//根据作品id获取作品
                }
                else if (string.IsNullOrEmpty(tagName) && BotConfig.SetuConfig.Pixiv.RandomMode == PixivRandomMode.随机标签)
                {
                    pixivWorkInfoDto = await getRandomWorkInTagsAsync(includeR18);//获取随机一个标签中的作品
                }
                else if (string.IsNullOrEmpty(tagName) && BotConfig.SetuConfig.Pixiv.RandomMode == PixivRandomMode.随机订阅)
                {
                    pixivWorkInfoDto = await getRandomWorkInSubscribeAsync(groupId);//获取随机一个订阅中的画师的作品
                }
                else if (string.IsNullOrEmpty(tagName))
                {
                    pixivWorkInfoDto = await getRandomWorkInTagsAsync(includeR18);//获取随机一个标签中的作品
                }
                else
                {
                    if (await BusinessHelper.CheckSTCustomEnableAsync(session, args) == false) return;
                    pixivWorkInfoDto = await getRandomWorkAsync(tagName, includeR18);//获取随机一个作品
                }

                if (pixivWorkInfoDto == null)
                {
                    await session.SendTemplateWithAtAsync(args, BotConfig.SetuConfig.Pixiv.NotFoundMsg, " 找不到这类型的图片或者收藏比过低，换个标签试试吧~");
                    return;
                }

                int todayLeftCount = BusinessHelper.GetSTLeftToday(session, args);
                FileInfo fileInfo = await downImgAsync(pixivWorkInfoDto);
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
                    if (BotConfig.PermissionsConfig.SetuLimitlessGroups.Contains(groupId) == false)
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
                    chatList.Add(new PlainMessage(getDefaultWorkInfo(pixivWorkInfo, fileInfo, startDateTime)));
                }
                else
                {
                    chatList.Add(new PlainMessage(getWorkInfoWithLeft(pixivWorkInfo, fileInfo, startDateTime, todayLeftCount, template)));
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

        /// <summary>
        /// 随机获取一个指定标签中的作品
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        protected async Task<PixivWorkInfoDto> getRandomWorkInTagsAsync(bool includeR18)
        {
            List<string> tagList = BotConfig.SetuConfig.Pixiv.RandomTags;
            if (tagList == null || tagList.Count == 0) return null;
            string tagName = tagList[new Random().Next(0, tagList.Count)];
            return await getRandomWorkAsync(tagName, includeR18);
        }

        /// <summary>
        /// 随机获取一个关注的画师的作品
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        protected async Task<PixivWorkInfoDto> getRandomWorkInSubscribeAsync(long groupId)
        {
            int loopUserTimes = 3;
            int loopWorkTimes = 5;
            SubscribeType subscribeType = SubscribeType.P站画师;
            if (BotConfig.SubscribeTaskMap.ContainsKey(subscribeType) == false) return null;
            List<SubscribeTask> subscribeTaskList = BotConfig.SubscribeTaskMap[subscribeType].Where(m => m.GroupIdList.Contains(groupId)).ToList();
            if (subscribeTaskList == null || subscribeTaskList.Count == 0) return null;
            for (int i = 0; i < loopUserTimes; i++)
            {
                int randomUserIndex = RandomHelper.getRandomBetween(0, subscribeTaskList.Count - 1);
                SubscribeTask subscribeTask = subscribeTaskList[randomUserIndex];
                PixivUserWorkInfoDto pixivWorkInfo = await getPixivUserWorkInfoDtoAsync(subscribeTask.SubscribeInfo.SubscribeCode);
                if (pixivWorkInfo == null) continue;
                Dictionary<string, PixivUserWorkInfo> illusts = pixivWorkInfo.body.illusts;
                if (illusts == null || illusts.Count == 0) continue;
                List<string> illustKeyList = Enumerable.ToList(illusts.Keys);
                for (int j = 0; j < loopWorkTimes; j++)
                {
                    int randomWorkIndex = RandomHelper.getRandomBetween(0, illustKeyList.Count - 1);
                    string randomWorkKey = illustKeyList[randomWorkIndex];
                    PixivUserWorkInfo pixivUserWorkInfo = illusts[randomWorkKey];
                    if (pixivUserWorkInfo.isR18() && groupId.IsShowR18() == false) continue;
                    PixivWorkInfoDto pixivWorkInfoDto = await getPixivWorkInfoDtoAsync(pixivUserWorkInfo.id);
                    if (pixivWorkInfoDto == null) continue;
                    bool isPopularity = pixivWorkInfoDto.body.bookmarkCount >= 100;
                    if (isPopularity == false) continue;
                    return pixivWorkInfoDto;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取作品集中的随机一个作品
        /// </summary>
        /// <param name="pixivicSearchDto"></param>
        /// <returns></returns>
        protected async Task<PixivWorkInfoDto> getRandomWorkAsync(string tagName, bool includeR18)
        {
            int pageCount = 3;
            PixivSearchDto pageOne = await getPixivSearchDtoAsync(tagName, 1, false);
            int total = pageOne.body.getIllust().total;
            int maxPage = (int)Math.Ceiling(Convert.ToDecimal(total) / pixivPageSize);
            maxPage = maxPage > 1000 ? 1000 : maxPage;
            Thread.Sleep(1000);

            //获取随机页中的所有作品
            int[] pageArr = getRandomPageNo(maxPage, pageCount);
            List<PixivIllust> tempIllustList = new List<PixivIllust>();
            foreach (int page in pageArr)
            {
                PixivSearchDto pixivSearchDto = await getPixivSearchDtoAsync(tagName, page, false);
                tempIllustList.AddRange(pixivSearchDto.body.getIllust().data);
                Thread.Sleep(1000);
            }

            //乱序
            List<PixivIllust> pixivIllustList = new List<PixivIllust>();
            Random random = RandomHelper.getRandom();
            while (tempIllustList.Count > 0)
            {
                int randomIndex = random.Next(0, tempIllustList.Count);
                pixivIllustList.Add(tempIllustList[randomIndex]);
                tempIllustList.RemoveAt(randomIndex);
            }

            //提取前N个作品
            pixivIllustList = pixivIllustList.Take(BotConfig.SetuConfig.Pixiv.MaxScreen).ToList();

            //创建线程池
            List<PixivIllust>[] taskList = new List<PixivIllust>[threadCount];
            for (int i = 0; i < taskList.Length; i++)
            {
                taskList[i] = new List<PixivIllust>();
            }

            //将作品分配给线程
            for (int i = 0; i < pixivIllustList.Count; i++)
            {
                if (i >= threadCount * workEachThread) break;
                taskList[i % threadCount].Add(pixivIllustList[i]);
            }

            //开启所有线程
            Task[] tasks = new Task[threadCount];
            for (int i = 0; i < taskList.Length; i++)
            {
                tasks[i] = Task.Factory.StartNew(() => getPixivWorkInfoMethodAsync(taskList[i], includeR18));
                await Task.Delay(RandomHelper.getRandomBetween(500, 1000));//将每条线程的间隔错开
            }
            Task.WaitAll(tasks);

            PixivWorkInfoDto randomWork = bookUpList.OrderByDescending(o => o.body.bookmarkCount).FirstOrDefault();
            if (randomWork == null) return null;
            randomWork.body.RelevantCount = total;
            return randomWork;
        }

        /// <summary>
        /// 线程函数
        /// </summary>
        /// <param name="pixivIllustList"></param>
        /// <param name="isScreen"></param>
        protected async Task getPixivWorkInfoMethodAsync(List<PixivIllust> pixivIllustList, bool includeR18)
        {
            for (int i = 0; i < pixivIllustList.Count; i++)
            {
                try
                {
                    if (bookUpList.Count > 0) return;
                    PixivWorkInfoDto pixivWorkInfo = await getPixivWorkInfoDtoAsync(pixivIllustList[i].id);
                    if (pixivWorkInfo.error) continue;
                    if (pixivWorkInfo.body.isR18() && includeR18 == false) continue;
                    if (checkRandomWorkIsOk(pixivWorkInfo) == false) continue;
                    lock (bookUpList) bookUpList.Add(pixivWorkInfo);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex, "获取作品信息时出现异常");
                }
                finally
                {
                    Thread.Sleep(500);//防止请求过快被检测
                }
            }
        }

        /// <summary>
        /// 判断插画质量是否符合
        /// </summary>
        /// <param name="pixivWorkInfo"></param>
        /// <returns></returns>
        protected bool checkRandomWorkIsOk(PixivWorkInfoDto pixivWorkInfo)
        {
            if (pixivWorkInfo == null) return false;
            if (pixivWorkInfo.body == null) return false;
            bool isNotBantTag = pixivWorkInfo.body.hasBanTag() == false;
            bool isPopularity = pixivWorkInfo.body.bookmarkCount >= BotConfig.SetuConfig.Pixiv.MinBookmark;
            bool isBookProportional = Convert.ToDouble(pixivWorkInfo.body.bookmarkCount) / pixivWorkInfo.body.viewCount >= BotConfig.SetuConfig.Pixiv.MinBookRate;
            return isPopularity && isBookProportional && isNotBantTag;
        }

        /// <summary>
        /// 判断插画质量是否符合
        /// </summary>
        /// <param name="pixivWorkInfo"></param>
        /// <returns></returns>
        protected bool checkTagWorkIsOk(PixivWorkInfoDto pixivWorkInfo)
        {
            if (pixivWorkInfo == null) return false;
            if (pixivWorkInfo.body == null) return false;
            bool isPopularity = pixivWorkInfo.body.bookmarkCount >= BotConfig.SubscribeConfig.PixivTag.MinBookmark;
            TimeSpan timeSpan = DateTime.Now.Subtract(pixivWorkInfo.body.createDate);
            int totalHours = (int)(timeSpan.TotalHours + 1 > 0 ? timeSpan.TotalHours + 1 : 0);
            bool isBookProportional = pixivWorkInfo.body.bookmarkCount > totalHours * BotConfig.SubscribeConfig.PixivTag.MinBookPerHour;
            return isPopularity && isBookProportional && totalHours > 0;
        }

        /// <summary>
        /// 从最大页数maxPage获取pageCount个随机页码
        /// </summary>
        /// <param name="maxPage"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        protected int[] getRandomPageNo(int maxPage, int pageCount)
        {
            if (maxPage <= pageCount)
            {
                int[] pageArr = new int[maxPage];
                for (int i = 0; i < maxPage; i++) pageArr[i] = i + 1;
                return pageArr;
            }
            else if (maxPage <= pageCount + 2)
            {
                int[] pageArr = new int[pageCount];
                int startPage = RandomHelper.getRandomBetween(1, 2);
                for (int i = 0; i < pageCount; i++) pageArr[i] = startPage + i;
                return pageArr;
            }
            else
            {
                int j = 0;
                int[] pageArr = new int[pageCount];
                while (j < pageArr.Length)
                {
                    int startPage = maxPage >= 10 ? 3 : 1;
                    int randomPage = RandomHelper.getRandomBetween(startPage, maxPage);
                    if (pageArr.Contains(randomPage)) continue;
                    pageArr[j] = randomPage;
                    j++;
                }
                return pageArr;
            }
        }


        /*-------------------------------------------------------------订阅相关--------------------------------------------------------------------------*/

        /// <summary>
        /// 订阅pixiv画师
        /// </summary>
        /// <param name="e"></param>
        /// <param name="message"></param>
        /// <param name="isGroupSubscribe"></param>
        /// <returns></returns>
        public async Task subscribeUserAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string message)
        {
            try
            {
                long memberId = args.Sender.Id;
                long groupId = args.Sender.Group.Id;

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
                            PixivUserInfoDto pixivUserInfoDto = await getPixivUserInfoDtoAsync(pixivUserId);
                            dbSubscribe = new SubscribePO();
                            dbSubscribe.SubscribeCode = pixivUserId;
                            dbSubscribe.SubscribeName = StringHelper.filterEmoji(pixivUserInfoDto.body.extraData.meta.title.Replace("- pixiv", "").Trim());
                            dbSubscribe.SubscribeDescription = dbSubscribe.SubscribeName;
                            dbSubscribe.SubscribeType = SubscribeType.P站画师;
                            dbSubscribe.Isliving = false;
                            dbSubscribe.CreateDate = DateTime.Now;
                            dbSubscribe = subscribeDao.Insert(dbSubscribe);
                        }

                        if (subscribeGroupDao.getCountBySubscribe(groupId, dbSubscribe.Id) > 0)
                        {
                            //关联订阅
                            await session.SendMessageWithAtAsync(args, new PlainMessage($" 画师id[{pixivUserId}]已经被订阅了~"));
                            continue;
                        }


                        SubscribeGroupPO subscribeGroup = new SubscribeGroupPO();
                        subscribeGroup.GroupId = groupId;
                        subscribeGroup.SubscribeId = dbSubscribe.Id;
                        subscribeGroup = subscribeGroupDao.Insert(subscribeGroup);
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
        public async Task subscribeTagAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string message)
        {
            try
            {
                string pixivTag = message.splitKeyWord(BotConfig.SubscribeConfig.PixivTag.AddCommand);
                if (string.IsNullOrWhiteSpace(pixivTag))
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" 没有检测到要订阅的标签，请确保指令格式正确"));
                    return;
                }
                PixivSearchDto pageOne = await getPixivSearchDtoAsync(pixivTag, 1, false);
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
        public async Task cancleSubscribeTagAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string message)
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
                List<PixivSubscribe> pixivSubscribeList = await getPixivUserNewestAsync(dbSubscribe.SubscribeCode, dbSubscribe.Id, PixivNewestRead);
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

                FileInfo fileInfo = await downImgAsync(pixivSubscribe.PixivWorkInfoDto);
                chatList.Add(new PlainMessage($"pixiv画师[{pixivSubscribe.PixivWorkInfoDto.body.userName}]的最新作品："));
                chatList.Add(new PlainMessage(getDefaultWorkInfo(pixivSubscribe.PixivWorkInfoDto.body, fileInfo, startTime)));

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


        /*-------------------------------------------------------------获取最新订阅--------------------------------------------------------------------------*/

        /// <summary>
        /// 获取画师的最新作品
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="subscribeId"></param>
        /// <param name="getCount"></param>
        /// <returns></returns>
        public async Task<List<PixivSubscribe>> getPixivUserNewestAsync(string userId, int subscribeId, int getCount = 2)
        {
            int index = 0;
            List<PixivSubscribe> pixivSubscribeList = new List<PixivSubscribe>();
            PixivUserWorkInfoDto pixivWorkInfo = await getPixivUserWorkInfoDtoAsync(userId);
            if (pixivWorkInfo == null) return pixivSubscribeList;
            Dictionary<string, PixivUserWorkInfo> illusts = pixivWorkInfo.body.illusts;
            if (illusts == null || illusts.Count == 0) return pixivSubscribeList;
            foreach (KeyValuePair<string, PixivUserWorkInfo> workInfo in illusts)
            {
                if (++index > getCount) break;
                if (workInfo.Value == null) continue;
                PixivWorkInfoDto pixivWorkInfoDto = await getPixivWorkInfoDtoAsync(workInfo.Value.id);
                if (pixivWorkInfoDto == null) continue;
                SubscribeRecordPO subscribeRecord = new SubscribeRecordPO(subscribeId);
                subscribeRecord.Title = StringHelper.filterEmoji(pixivWorkInfoDto.body.illustTitle);
                subscribeRecord.Content = subscribeRecord.Title;
                subscribeRecord.CoverUrl = HttpUrl.getPixivWorkInfoUrl(workInfo.Value.id);
                subscribeRecord.LinkUrl = HttpUrl.getPixivWorkInfoUrl(workInfo.Value.id);
                subscribeRecord.DynamicCode = pixivWorkInfoDto.body.illustId;
                subscribeRecord.DynamicType = SubscribeDynamicType.插画;
                PixivSubscribe pixivSubscribe = new PixivSubscribe();
                pixivSubscribe.SubscribeRecord = subscribeRecord;
                pixivSubscribe.PixivWorkInfoDto = pixivWorkInfoDto;
                pixivSubscribeList.Add(pixivSubscribe);
            }
            return pixivSubscribeList;
        }

        /// <summary>
        /// 获取订阅画师的最新作品
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="subscribeId"></param>
        /// <param name="getCount"></param>
        /// <returns></returns>
        public async Task<List<PixivSubscribe>> getPixivUserSubscribeAsync(string userId, int subscribeId, int getCount = 2)
        {
            int index = 0;
            List<PixivSubscribe> pixivSubscribeList = new List<PixivSubscribe>();
            PixivUserWorkInfoDto pixivWorkInfo = await getPixivUserWorkInfoDtoAsync(userId);
            if (pixivWorkInfo == null) return pixivSubscribeList;
            Dictionary<string, PixivUserWorkInfo> illusts = pixivWorkInfo.body.illusts;
            if (illusts == null || illusts.Count == 0) return pixivSubscribeList;
            foreach (KeyValuePair<string, PixivUserWorkInfo> workInfo in illusts)
            {
                if (++index > getCount) break;
                if (workInfo.Value == null) continue;
                SubscribeRecordPO dbSubscribe = subscribeRecordDao.checkExists(subscribeId, workInfo.Value.id);
                if (dbSubscribe != null) continue;
                PixivWorkInfoDto pixivWorkInfoDto = await getPixivWorkInfoDtoAsync(workInfo.Value.id);
                if (pixivWorkInfoDto == null) continue;
                int shelfLife = BotConfig.SubscribeConfig.PixivUser.ShelfLife;
                if (shelfLife > 0 && pixivWorkInfoDto.body.createDate < DateTime.Now.AddSeconds(-1 * shelfLife)) continue;
                SubscribeRecordPO subscribeRecord = new SubscribeRecordPO(subscribeId);
                subscribeRecord.Title = StringHelper.filterEmoji(pixivWorkInfoDto.body.illustTitle);
                subscribeRecord.Content = subscribeRecord.Title;
                subscribeRecord.CoverUrl = HttpUrl.getPixivWorkInfoUrl(workInfo.Value.id);
                subscribeRecord.LinkUrl = HttpUrl.getPixivWorkInfoUrl(workInfo.Value.id);
                subscribeRecord.DynamicCode = pixivWorkInfoDto.body.illustId;
                subscribeRecord.DynamicType = SubscribeDynamicType.插画;
                subscribeRecord = subscribeRecordDao.Insert(subscribeRecord);
                PixivSubscribe pixivSubscribe = new PixivSubscribe();
                pixivSubscribe.SubscribeRecord = subscribeRecord;
                pixivSubscribe.PixivWorkInfoDto = pixivWorkInfoDto;
                pixivSubscribeList.Add(pixivSubscribe);
            }
            return pixivSubscribeList;
        }

        /// <summary>
        /// 获取订阅标签的最新作品
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="subscribeId"></param>
        /// <returns></returns>
        public async Task<List<PixivSubscribe>> getPixivTagSubscribeAsync(string tagName, int subscribeId)
        {
            PixivSearchDto pageOne = await getPixivSearchDtoAsync(tagName, 1, false);
            List<PixivSubscribe> pixivSubscribeList = new List<PixivSubscribe>();
            if (pageOne == null) return pixivSubscribeList;
            foreach (PixivIllust item in pageOne.body.getIllust().data)
            {
                int shelfLife = BotConfig.SubscribeConfig.PixivTag.ShelfLife;
                if (shelfLife > 0 && item.createDate < DateTime.Now.AddSeconds(-1 * shelfLife)) continue;

                PixivWorkInfoDto pixivWorkInfoDto = await getPixivWorkInfoDtoAsync(item.id);
                if (pixivWorkInfoDto == null) continue;
                if (checkTagWorkIsOk(pixivWorkInfoDto) == false) continue;
                SubscribeRecordPO dbSubscribe = subscribeRecordDao.checkExists(subscribeId, pixivWorkInfoDto.body.illustId);
                if (dbSubscribe != null) continue;
                SubscribeRecordPO subscribeRecord = new SubscribeRecordPO(subscribeId);
                subscribeRecord.Title = StringHelper.filterEmoji(pixivWorkInfoDto.body.illustTitle);
                subscribeRecord.Content = subscribeRecord.Title;
                subscribeRecord.CoverUrl = HttpUrl.getPixivWorkInfoUrl(pixivWorkInfoDto.body.illustId);
                subscribeRecord.LinkUrl = HttpUrl.getPixivWorkInfoUrl(pixivWorkInfoDto.body.illustId);
                subscribeRecord.DynamicCode = pixivWorkInfoDto.body.illustId;
                subscribeRecord.DynamicType = SubscribeDynamicType.插画;
                subscribeRecord = subscribeRecordDao.Insert(subscribeRecord);
                PixivSubscribe pixivSubscribe = new PixivSubscribe();
                pixivSubscribe.SubscribeRecord = subscribeRecord;
                pixivSubscribe.PixivWorkInfoDto = pixivWorkInfoDto;
                pixivSubscribeList.Add(pixivSubscribe);
                await Task.Delay(1000);
            }
            return pixivSubscribeList;
        }


        /*-------------------------------------------------------------图片下载--------------------------------------------------------------------------*/

        public async Task<FileInfo> downImgAsync(PixivWorkInfoDto pixivWorkInfo)
        {
            try
            {
                if (pixivWorkInfo.body.isGif()) return await downAndComposeGifAsync(pixivWorkInfo);
                string imgUrl = pixivWorkInfo.body.urls.original;
                string fullFileName = $"{pixivWorkInfo.body.illustId}.jpg";
                string fullImageSavePath = Path.Combine(FilePath.getDownImgSavePath(), fullFileName);
                if (BotConfig.GeneralConfig.DownWithProxy || BotConfig.GeneralConfig.PixivFreeProxy)
                {
                    imgUrl = imgUrl.Replace("https://i.pximg.net", BotConfig.GeneralConfig.PixivProxy);
                    return await HttpHelper.DownFileAsync(imgUrl, fullImageSavePath);
                }
                else
                {
                    Dictionary<string, string> headerDic = new Dictionary<string, string>();
                    headerDic.Add("Referer", HttpUrl.getPixivArtworksReferer(pixivWorkInfo.body.illustId));
                    headerDic.Add("Cookie", BotConfig.WebsiteConfig.Pixiv.Cookie);
                    return await HttpHelper.DownFileAsync(imgUrl, fullImageSavePath, headerDic);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "PixivBusiness.downImg下载图片失败");
                return null;
            }
        }


        /// <summary>
        /// 下载动图zip包并合成gif图片
        /// </summary>
        /// <param name="pixivWorkInfo"></param>
        /// <returns></returns>
        protected async Task<FileInfo> downAndComposeGifAsync(PixivWorkInfoDto pixivWorkInfo)
        {
            string fullGifSavePath = Path.Combine(FilePath.getDownImgSavePath(), $"{pixivWorkInfo.body.illustId}.gif");
            if (File.Exists(fullGifSavePath)) return new FileInfo(fullGifSavePath);

            PixivUgoiraMetaDto pixivUgoiraMetaDto = await getPixivUgoiraMetaDtoAsync(pixivWorkInfo.body.illustId);
            string fullZipSavePath = Path.Combine(FilePath.getDownImgSavePath(), $"{StringHelper.get16UUID()}.zip");
            string zipHttpUrl = pixivUgoiraMetaDto.body.src;

            if (BotConfig.GeneralConfig.DownWithProxy || BotConfig.GeneralConfig.PixivFreeProxy)
            {
                zipHttpUrl = zipHttpUrl.Replace("https://i.pximg.net", BotConfig.GeneralConfig.PixivProxy);
                await HttpHelper.DownFileAsync(zipHttpUrl, fullZipSavePath);
            }
            else
            {
                Dictionary<string, string> headerDic = new Dictionary<string, string>();
                headerDic.Add("Referer", HttpUrl.getPixivArtworksReferer(pixivWorkInfo.body.illustId));
                headerDic.Add("Cookie", BotConfig.WebsiteConfig.Pixiv.Cookie);
                await HttpHelper.DownFileAsync(zipHttpUrl, fullZipSavePath, headerDic);
            }

            string unZipDirPath = Path.Combine(FilePath.getDownImgSavePath(), pixivWorkInfo.body.illustId);
            ZipHelper.ZipToFile(fullZipSavePath, unZipDirPath);
            DirectoryInfo directoryInfo = new DirectoryInfo(unZipDirPath);
            FileInfo[] files = directoryInfo.GetFiles();
            List<PixivUgoiraMetaFrames> frames = pixivUgoiraMetaDto.body.frames;
            using AnimatedGifCreator gif = AnimatedGif.AnimatedGif.Create(fullGifSavePath, 0);
            foreach (FileInfo file in files)
            {
                PixivUgoiraMetaFrames frame = frames.Where(o => o.file.Trim() == file.Name).FirstOrDefault();
                int delay = frame == null ? 60 : frame.delay;
                using Image img = Image.FromFile(file.FullName);
                gif.AddFrame(img, delay, GifQuality.Bit8);
                Thread.Sleep(100);
            }

            File.Delete(fullZipSavePath);
            Directory.Delete(unZipDirPath, true);
            return new FileInfo(fullGifSavePath);
        }

        /*-------------------------------------------------------------作品信息--------------------------------------------------------------------------*/

        public string getWorkInfoWithLeft(PixivWorkInfo pixivWorkInfo, FileInfo fileInfo, DateTime startTime, int todayLeft, string template = "")
        {
            template = getWorkInfo(pixivWorkInfo, fileInfo, startTime, template);
            template = template.Replace("{TodayLeft}", todayLeft.ToString());
            return template;
        }

        public string getWorkInfoWithTag(PixivWorkInfo pixivWorkInfo, FileInfo fileInfo, DateTime startTime, string tagName, string template = "")
        {
            template = getWorkInfo(pixivWorkInfo, fileInfo, startTime, template);
            template = template.Replace("{TagName}", tagName);
            return template;
        }

        public string getWorkInfo(PixivWorkInfo pixivWorkInfo, FileInfo fileInfo, DateTime startTime, string template = "")
        {
            if (string.IsNullOrWhiteSpace(template)) return getDefaultWorkInfo(pixivWorkInfo, fileInfo, startTime);
            int costSecond = DateTimeHelper.GetSecondDiff(startTime, DateTime.Now);
            double sizeMB = fileInfo == null ? 0 : MathHelper.getMbWithByte(fileInfo.Length);
            template = template.Replace("{MemberCD}", BotConfig.SetuConfig.MemberCD.ToString());
            template = template.Replace("{RevokeInterval}", BotConfig.SetuConfig.RevokeInterval.ToString());
            template = template.Replace("{IllustTitle}", pixivWorkInfo.illustTitle);
            template = template.Replace("{UserName}", pixivWorkInfo.userName);
            template = template.Replace("{UserId}", pixivWorkInfo.userId.ToString());
            template = template.Replace("{SizeMB}", sizeMB.ToString());
            template = template.Replace("{CreateTime}", pixivWorkInfo.createDate.ToSimpleString());
            template = template.Replace("{BookmarkCount}", pixivWorkInfo.bookmarkCount.ToString());
            template = template.Replace("{LikeCount}", pixivWorkInfo.likeCount.ToString());
            template = template.Replace("{ViewCount}", pixivWorkInfo.viewCount.ToString());
            template = template.Replace("{CostSecond}", costSecond.ToString());
            template = template.Replace("{RelevantCount}", pixivWorkInfo.RelevantCount.ToString());
            template = template.Replace("{PageCount}", pixivWorkInfo.pageCount.ToString());
            template = template.Replace("{Tags}", getTagsStr(pixivWorkInfo.tags));
            template = template.Replace("{Urls}", getWorkUrlStr(pixivWorkInfo));
            return template;
        }

        public string getDefaultWorkInfo(PixivWorkInfo pixivWorkInfo, FileInfo fileInfo, DateTime startTime)
        {
            StringBuilder workInfoStr = new StringBuilder();
            int costSecond = DateTimeHelper.GetSecondDiff(startTime, DateTime.Now);
            double sizeMB = fileInfo == null ? 0 : MathHelper.getMbWithByte(fileInfo.Length);
            workInfoStr.AppendLine($"标题：{pixivWorkInfo.illustTitle}，画师：{pixivWorkInfo.userName}，画师id：{pixivWorkInfo.userId}，大小：{sizeMB}MB，发布时间：{pixivWorkInfo.createDate.ToSimpleString()}，");
            workInfoStr.AppendLine($"收藏：{pixivWorkInfo.bookmarkCount}，赞：{pixivWorkInfo.likeCount}，浏览：{pixivWorkInfo.viewCount}，");
            workInfoStr.AppendLine($"耗时：{costSecond}s，标签图片：{pixivWorkInfo.RelevantCount}张，作品图片:{pixivWorkInfo.pageCount}张");
            workInfoStr.AppendLine($"标签：{getTagsStr(pixivWorkInfo.tags)}");
            workInfoStr.Append(getWorkUrlStr(pixivWorkInfo));
            return workInfoStr.ToString();
        }

        protected string getWorkUrlStr(PixivWorkInfo pixivWorkInfo)
        {
            int maxShowCount = 3;
            string proxy = BotConfig.GeneralConfig.PixivProxy;
            if (string.IsNullOrWhiteSpace(proxy)) proxy = "https://i.pixiv.re";
            StringBuilder LinkStr = new StringBuilder();
            int endCount = pixivWorkInfo.pageCount > maxShowCount ? maxShowCount : pixivWorkInfo.pageCount;
            for (int i = 0; i < endCount; i++)
            {
                string imgUrl = pixivWorkInfo.urls.original.Replace("https://i.pximg.net", proxy);
                if (i > 0) imgUrl = imgUrl.Replace("_p0.", $"_p{i}.");
                if (i < endCount - 1)
                {
                    LinkStr.AppendLine(imgUrl);
                }
                else
                {
                    LinkStr.Append(imgUrl);
                }
            }
            return LinkStr.ToString();
        }

        protected static string getTagsStr(PixivTagDto tags)
        {
            string tagstr = "";
            foreach (PixivTagModel pixivTagModel in tags.tags)
            {
                if (tagstr.Length > 0) tagstr += "，";
                tagstr += pixivTagModel.translation != null && string.IsNullOrEmpty(pixivTagModel.translation.en) == false ? pixivTagModel.translation.en : pixivTagModel.tag;
            }
            return tagstr;
        }

        /*-------------------------------------------------------------接口相关--------------------------------------------------------------------------*/

        public async Task<PixivSearchDto> getPixivSearchDtoAsync(string keyword, int pageNo, bool isMatchAll)
        {
            string referer = HttpUrl.getPixivSearchReferer(keyword);
            Dictionary<string, string> headerDic = getPixivHeader(referer);
            string postUrl = HttpUrl.getPixivSearchUrl(keyword, pageNo, isMatchAll);
            string json = await HttpHelper.PixivGetAsync(postUrl, headerDic);
            return JsonConvert.DeserializeObject<PixivSearchDto>(json);
        }

        public async Task<PixivWorkInfoDto> getPixivWorkInfoDtoAsync(string wordId)
        {
            string referer = HttpUrl.getPixivArtworksReferer(wordId);
            Dictionary<string, string> headerDic = getPixivHeader(referer);
            string postUrl = HttpUrl.getPixivWorkInfoUrl(wordId);
            string json = await HttpHelper.PixivGetAsync(postUrl, headerDic);
            return JsonConvert.DeserializeObject<PixivWorkInfoDto>(json);
        }

        public async Task<PixivUserWorkInfoDto> getPixivUserWorkInfoDtoAsync(string userId)
        {
            string referer = HttpUrl.getPixivUserWorkInfoReferer(userId);
            Dictionary<string, string> headerDic = getPixivHeader(referer);
            string postUrl = HttpUrl.getPixivUserWorkInfoUrl(userId);
            string json = await HttpHelper.PixivGetAsync(postUrl, headerDic);
            if (string.IsNullOrEmpty(json) == false && json.Contains("\"illusts\":[]"))
            {
                //throw new Exception($"pixiv用户{userId}作品列表illusts为空,cookie可能已经过期");
                return null;
            }
            return JsonConvert.DeserializeObject<PixivUserWorkInfoDto>(json);
        }

        public async Task<PixivUserInfoDto> getPixivUserInfoDtoAsync(string userId)
        {
            string referer = HttpUrl.getPixivUserWorkInfoReferer(userId);
            Dictionary<string, string> headerDic = getPixivHeader(referer);
            string postUrl = HttpUrl.getPixivUserWorkInfoUrl(userId);
            string json = await HttpHelper.PixivGetAsync(postUrl, headerDic);
            return JsonConvert.DeserializeObject<PixivUserInfoDto>(json);
        }

        public async Task<PixivUgoiraMetaDto> getPixivUgoiraMetaDtoAsync(string wordId)
        {
            string referer = HttpUrl.getPixivArtworksReferer(wordId);
            Dictionary<string, string> headerDic = getPixivHeader(referer);
            string postUrl = HttpUrl.getPixivUgoiraMetaUrl(wordId);
            string json = await HttpHelper.PixivGetAsync(postUrl, headerDic);
            return JsonConvert.DeserializeObject<PixivUgoiraMetaDto>(json);
        }


        public Dictionary<string, string> getPixivHeader(string referer)
        {
            Dictionary<string, string> headerDic = new Dictionary<string, string>();
            headerDic.Add("cookie", BotConfig.WebsiteConfig.Pixiv.Cookie);
            headerDic.Add("referer", referer);
            //headerDic.Add("accept", "application/json");
            //headerDic.Add("sec-fetch-mode", "cors");
            //headerDic.Add("sec-fetch-site", "same-origin");
            //headerDic.Add("x-user-id", Setting.Pixiv.XUserId);
            return headerDic;
        }



    }
}
