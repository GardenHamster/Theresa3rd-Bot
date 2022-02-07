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
        /// 收藏数超过2000的作品集
        /// </summary>
        private List<PixivWorkInfoDto> bookUp2000List;

        /// <summary>
        /// 收藏数超过1500的作品集
        /// </summary>
        private List<PixivWorkInfoDto> bookUp1500List;

        /// <summary>
        /// 收藏数超过1000的作品集
        /// </summary>
        private List<PixivWorkInfoDto> bookUp1000List;

        /// <summary>
        /// 所有作品集
        /// </summary>
        private List<PixivWorkInfoDto> bookUp800List;

        /// <summary>
        /// 收藏数超过0的作品集
        /// </summary>
        private List<PixivWorkInfoDto> bookUpList;

        public PixivBusiness()
        {
            bookUp2000List = new List<PixivWorkInfoDto>();
            bookUp1500List = new List<PixivWorkInfoDto>();
            bookUp1000List = new List<PixivWorkInfoDto>();
            bookUp800List = new List<PixivWorkInfoDto>();
            bookUpList = new List<PixivWorkInfoDto>();
            subscribeRecordDao = new SubscribeRecordDao();
        }


        public async Task sendGeneralPixivImageAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string message)
        {
            try
            {
                DateTime startDateTime = DateTime.Now;
                CoolingCache.SetHanding(args.Sender.Group.Id, args.Sender.Id);//请求处理中

                if (string.IsNullOrWhiteSpace(BotConfig.SetuConfig.Pixiv.ProcessingMsg) == false)
                {
                    await session.SendTemplateWithAtAsync(args, BotConfig.SetuConfig.Pixiv.ProcessingMsg, null);
                    await Task.Delay(1000);
                }

                PixivWorkInfoDto pixivWorkInfoDto = null;

                string tagName = message.splitKeyWord(BotConfig.SetuConfig.Pixiv.Command) ?? "";
                tagName = tagName.Replace("（", ")").Replace("）", ")");


                if (StringHelper.isPureNumber(tagName))
                {
                    if (BusinessHelper.CheckSTCustomEnableAsync(session, args).Result == false) return;
                    pixivWorkInfoDto = getPixivWorkInfoDto(tagName);//根据作品id获取作品
                }
                else if (string.IsNullOrEmpty(tagName) && BotConfig.SetuConfig.Pixiv.RandomMode == PixivRandomMode.随机标签)
                {
                    pixivWorkInfoDto = getRandomWorkInTags();//获取随机一个标签中的作品
                }
                else if (string.IsNullOrEmpty(tagName) && BotConfig.SetuConfig.Pixiv.RandomMode == PixivRandomMode.随机订阅)
                {
                    pixivWorkInfoDto = getRandomWorkInSubscribe(args.Sender.Group.Id);//获取随机一个订阅中的画师的作品
                }
                else if (string.IsNullOrEmpty(tagName))
                {
                    pixivWorkInfoDto = getRandomWorkInTags();//获取随机一个标签中的作品
                }
                else
                {
                    if (BusinessHelper.CheckSTCustomEnableAsync(session, args).Result == false) return;
                    pixivWorkInfoDto = getRandomWorkAsync(tagName).Result;//获取随机一个作品
                }

                if (pixivWorkInfoDto == null)
                {
                    await session.SendTemplateWithAtAsync(args, BotConfig.SetuConfig.Pixiv.NotFoundMsg, " 找不到这类型的图片或者收藏比过低，换个标签试试吧~");
                    return;
                }

                int todayLeftCount = BusinessHelper.GetSTLeftToday(session, args);
                FileInfo fileInfo = downImg(pixivWorkInfoDto);
                PixivWorkInfo pixivWorkInfo = pixivWorkInfoDto.body;

                int groupMsgId = 0;
                string template = BotConfig.SetuConfig.Pixiv.Template;
                List<IChatMessage> chatList = new List<IChatMessage>();
                if (string.IsNullOrWhiteSpace(template))
                {
                    StringBuilder warnBuilder = new StringBuilder();
                    if (BotConfig.PermissionsConfig.SetuNoneCDGroups.Contains(args.Sender.Group.Id) == false)
                    {
                        if (warnBuilder.Length > 0) warnBuilder.Append("，");
                        warnBuilder.Append($"{BotConfig.SetuConfig.MemberCD}秒后再来哦");
                    }
                    if (BotConfig.PermissionsConfig.SetuLimitlessGroups.Contains(args.Sender.Group.Id) == false)
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
                        groupList.AddRange(session.SplitToChainAsync(BotConfig.GeneralConfig.DownErrorImg).Result);
                    }
                    else if (pixivWorkInfoDto.body.isR18() == false)
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
                            memberList.AddRange(session.SplitToChainAsync(BotConfig.GeneralConfig.DownErrorImg, UploadTarget.Friend).Result);
                        }
                        if (pixivWorkInfoDto.body.isR18() == false && fileInfo != null)
                        {
                            memberList.Add((IChatMessage)await session.UploadPictureAsync(UploadTarget.Friend, fileInfo.FullName));
                        }
                        await session.SendFriendMessageAsync(args.Sender.Id, memberList.ToArray());
                        await Task.Delay(1000);
                    }
                    catch (Exception)
                    {
                    }
                }

                //进入CD状态
                CoolingCache.SetMemberSTCooling(args.Sender.Group.Id, args.Sender.Id);
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


        public async Task<List<IChatMessage>> getPixivNewestWorkAsync(IMiraiHttpSession session, string userId, int subscribeId)
        {
            try
            {
                DateTime startTime = DateTime.Now;
                List<IChatMessage> chatList = new List<IChatMessage>();
                List<PixivSubscribe> pixivSubscribeList = getPixivUserNewestWork(userId, subscribeId, PixivNewestRead);
                if (pixivSubscribeList == null || pixivSubscribeList.Count == 0) return new List<IChatMessage>();
                PixivSubscribe pixivSubscribe = pixivSubscribeList.First();
                if (pixivSubscribe.PixivWorkInfoDto.body.isR18()) return new List<IChatMessage> { new PlainMessage(" 该作品为R-18作品，不显示相关内容") };
                FileInfo fileInfo = downImg(pixivSubscribe.PixivWorkInfoDto);
                chatList.Add(new PlainMessage($"pixiv画师[{pixivSubscribe.PixivWorkInfoDto.body.userName}]的最新作品："));
                chatList.Add(new PlainMessage(getDefaultWorkInfo(pixivSubscribe.PixivWorkInfoDto.body, fileInfo, startTime)));
                if (fileInfo == null)
                {
                    chatList.AddRange(session.SplitToChainAsync(BotConfig.GeneralConfig.DownErrorImg).Result);
                }
                else
                {
                    if (!pixivSubscribe.PixivWorkInfoDto.body.isR18()) chatList.Add((IChatMessage)await session.UploadPictureAsync(UploadTarget.Group, fileInfo.FullName));
                }
                return chatList;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "获取画师最新作品时出现异常");
                throw;
            }
        }

        /// <summary>
        /// 随机获取一个指定标签中的作品
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        protected PixivWorkInfoDto getRandomWorkInTags()
        {
            List<string> tagList = BotConfig.SetuConfig.Pixiv.RandomTags;
            if (tagList == null || tagList.Count == 0) return null;
            string tagName = tagList[new Random().Next(0, tagList.Count)];
            return getRandomWorkAsync(tagName).Result;
        }

        /// <summary>
        /// 随机获取一个关注的画师的作品
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        protected PixivWorkInfoDto getRandomWorkInSubscribe(long groupId)
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
                PixivUserWorkInfoDto pixivWorkInfo = getPixivUserWorkInfoDto(subscribeTask.SubscribeInfo.SubscribeCode);
                if (pixivWorkInfo == null) continue;
                Dictionary<string, PixivUserWorkInfo> illusts = pixivWorkInfo.body.illusts;
                if (illusts == null || illusts.Count == 0) continue;
                List<string> illustKeyList = Enumerable.ToList(illusts.Keys);
                for (int j = 0; j < loopWorkTimes; j++)
                {
                    int randomWorkIndex = RandomHelper.getRandomBetween(0, illustKeyList.Count - 1);
                    string randomWorkKey = illustKeyList[randomWorkIndex];
                    PixivUserWorkInfo pixivUserWorkInfo = illusts[randomWorkKey];
                    if (pixivUserWorkInfo.isR18()) continue;
                    PixivWorkInfoDto pixivWorkInfoDto = getPixivWorkInfoDto(pixivUserWorkInfo.id);
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
        protected async Task<PixivWorkInfoDto> getRandomWorkAsync(string tagName)
        {
            int pageCount = 3;
            PixivSearchDto pageOne = getPixivSearchDto(tagName, 1, false);
            int total = pageOne.body.getIllust().total;
            int maxPage = (int)Math.Ceiling(Convert.ToDecimal(total) / pixivPageSize);
            maxPage = maxPage > 1000 ? 1000 : maxPage;
            Thread.Sleep(1000);

            //获取随机页中的所有作品
            int[] pageArr = getRandomPageNo(maxPage, pageCount);
            List<PixivIllust> tempIllustList = new List<PixivIllust>();
            foreach (int page in pageArr)
            {
                PixivSearchDto pixivSearchDto = getPixivSearchDto(tagName, page, false);
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

            //是否优先筛选较高收藏
            bool isScreen = total > workScreenMoreThen;


            //开启所有线程
            Task[] tasks = new Task[threadCount];
            for (int i = 0; i < taskList.Length; i++)
            {
                tasks[i] = Task.Factory.StartNew(() => getPixivWorkInfoMethod(taskList[i], isScreen));
                await Task.Delay(RandomHelper.getRandomBetween(500, 1000));//将每条线程的间隔错开
            }
            Task.WaitAll(tasks);

            //获取收藏度最高的作品
            PixivWorkInfoDto randomWork = bookUp2000List.FirstOrDefault();
            if (randomWork == null) randomWork = bookUp1500List.FirstOrDefault();
            if (randomWork == null) randomWork = bookUp1000List.FirstOrDefault();
            if (randomWork == null) randomWork = bookUp800List.FirstOrDefault();
            if (randomWork == null) return null;
            randomWork.body.RelevantCount = total;
            return randomWork;
        }

        /// <summary>
        /// 线程函数
        /// </summary>
        /// <param name="pixivIllustList"></param>
        /// <param name="isScreen"></param>
        protected void getPixivWorkInfoMethod(List<PixivIllust> pixivIllustList, bool isScreen)
        {
            for (int i = 0; i < pixivIllustList.Count; i++)
            {
                try
                {
                    if (isScreen == true && bookUp2000List.Count > 0) return;//如果启用筛选,获取到一个2000+就结束全部线程
                    if (isScreen == false && bookUp800List.Count > 0) return;//如果不启用筛选,获取到一个800+就结束全部线程
                    PixivWorkInfoDto pixivWorkInfo = getPixivWorkInfoDto(pixivIllustList[i].id);
                    if (checkRandomWorkIsOk(pixivWorkInfo) && pixivWorkInfo.error == false)
                    {
                        if (pixivWorkInfo.body.likeCount >= 2000)
                        {
                            lock (bookUp2000List) bookUp2000List.Add(pixivWorkInfo);
                        }
                        else if (pixivWorkInfo.body.likeCount >= 1500)
                        {
                            lock (bookUp1500List) bookUp1500List.Add(pixivWorkInfo);
                        }
                        else if (pixivWorkInfo.body.likeCount >= 1000)
                        {
                            lock (bookUp1000List) bookUp1000List.Add(pixivWorkInfo);
                        }
                        else if (pixivWorkInfo.body.likeCount >= 800)
                        {
                            lock (bookUp800List) bookUp800List.Add(pixivWorkInfo);
                        }
                        lock (bookUpList) bookUpList.Add(pixivWorkInfo);
                    }
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

        /// <summary>
        /// 获取画师的最新作品
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="subscribeId"></param>
        /// <param name="getCount"></param>
        /// <returns></returns>
        public List<PixivSubscribe> getPixivUserNewestWork(string userId, int subscribeId, int getCount = 2)
        {
            int index = 0;
            List<PixivSubscribe> pixivSubscribeList = new List<PixivSubscribe>();
            PixivUserWorkInfoDto pixivWorkInfo = getPixivUserWorkInfoDto(userId);
            if (pixivWorkInfo == null) return pixivSubscribeList;
            Dictionary<string, PixivUserWorkInfo> illusts = pixivWorkInfo.body.illusts;
            if (illusts == null || illusts.Count == 0) return pixivSubscribeList;
            foreach (KeyValuePair<string, PixivUserWorkInfo> workInfo in illusts)
            {
                if (++index > getCount) break;
                if (workInfo.Value == null) continue;
                PixivWorkInfoDto pixivWorkInfoDto = getPixivWorkInfoDto(workInfo.Value.id);
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
        public List<PixivSubscribe> getPixivUserSubscribeWork(string userId, int subscribeId, int getCount = 2)
        {
            int index = 0;
            List<PixivSubscribe> pixivSubscribeList = new List<PixivSubscribe>();
            PixivUserWorkInfoDto pixivWorkInfo = getPixivUserWorkInfoDto(userId);
            if (pixivWorkInfo == null) return pixivSubscribeList;
            Dictionary<string, PixivUserWorkInfo> illusts = pixivWorkInfo.body.illusts;
            if (illusts == null || illusts.Count == 0) return pixivSubscribeList;
            foreach (KeyValuePair<string, PixivUserWorkInfo> workInfo in illusts)
            {
                if (++index > getCount) break;
                if (workInfo.Value == null) continue;
                SubscribeRecordPO dbSubscribe = subscribeRecordDao.checkExists(subscribeId, workInfo.Value.id);
                if (dbSubscribe != null) continue;
                PixivWorkInfoDto pixivWorkInfoDto = getPixivWorkInfoDto(workInfo.Value.id);
                if (pixivWorkInfoDto == null) continue;
                if (pixivWorkInfoDto.body.isR18()) continue;
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
            PixivSearchDto pageOne = getPixivSearchDto(tagName, 1, false);
            List<PixivSubscribe> pixivSubscribeList = new List<PixivSubscribe>();
            if (pageOne == null) return pixivSubscribeList;
            foreach (PixivIllust item in pageOne.body.getIllust().data)
            {
                int shelfLife = BotConfig.SubscribeConfig.PixivTag.ShelfLife;
                if (shelfLife > 0 && item.createDate < DateTime.Now.AddSeconds(-1 * shelfLife)) continue;
                
                PixivWorkInfoDto pixivWorkInfoDto = getPixivWorkInfoDto(item.id);
                if (pixivWorkInfoDto == null) continue;
                if (pixivWorkInfoDto.body.isR18()) continue;
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


        /// <summary>
        /// 判断插画质量是否符合
        /// </summary>
        /// <param name="pixivWorkInfo"></param>
        /// <param name="isR18"></param>
        /// <returns></returns>
        protected bool checkRandomWorkIsOk(PixivWorkInfoDto pixivWorkInfo)
        {
            if (pixivWorkInfo == null) return false;
            if (pixivWorkInfo.body == null) return false;
            bool isNotR18 = pixivWorkInfo.body.isR18() == false;
            bool isNotBantTag = pixivWorkInfo.body.hasBanTag() == false;
            bool isPopularity = pixivWorkInfo.body.bookmarkCount >= BotConfig.SetuConfig.Pixiv.MinBookmark;
            bool isBookProportional = Convert.ToDouble(pixivWorkInfo.body.bookmarkCount) / pixivWorkInfo.body.viewCount >= BotConfig.SetuConfig.Pixiv.MinBookRate;
            return isNotR18 && isPopularity && isBookProportional && isNotBantTag;
        }

        /// <summary>
        /// 判断插画质量是否符合
        /// </summary>
        /// <param name="pixivWorkInfo"></param>
        /// <param name="isR18"></param>
        /// <returns></returns>
        protected bool checkTagWorkIsOk(PixivWorkInfoDto pixivWorkInfo)
        {
            if (pixivWorkInfo == null) return false;
            if (pixivWorkInfo.body == null) return false;
            bool isNotR18 = pixivWorkInfo.body.isR18() == false;
            bool isPopularity = pixivWorkInfo.body.bookmarkCount >= BotConfig.SubscribeConfig.PixivTag.MinBookmark;
            TimeSpan timeSpan = DateTime.Now.Subtract(pixivWorkInfo.body.createDate);
            int totalHours = (int)(timeSpan.TotalHours + 1 > 0 ? timeSpan.TotalHours + 1 : 0);
            bool isBookProportional = pixivWorkInfo.body.bookmarkCount > totalHours * BotConfig.SubscribeConfig.PixivTag.MinBookPerHour;
            return isPopularity && isNotR18 && isBookProportional && totalHours > 0;
        }

        public FileInfo downImg(PixivWorkInfoDto pixivWorkInfo)
        {
            try
            {
                if (pixivWorkInfo.body.isGif()) return downAndComposeGif(pixivWorkInfo);
                string fullFileName = pixivWorkInfo.body.illustId + ".jpg";
                string imgReferer = HttpUrl.getPixivArtworksReferer(pixivWorkInfo.body.illustId);
                string imgUrl = pixivWorkInfo.body.urls.original;
                if (BotConfig.GeneralConfig.DownWithProxy) imgUrl = imgUrl.Replace("https://i.pximg.net", BotConfig.GeneralConfig.PixivProxy);
                string fullImageSavePath = Path.Combine(FilePath.getDownImgSavePath(), fullFileName);
                return HttpHelper.downImg(imgUrl, fullImageSavePath, imgReferer, BotConfig.WebsiteConfig.Pixiv.Cookie);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "PixivBusiness.downImg下载图片失败");
                return null;
            }
        }

        protected FileInfo downAndComposeGif(PixivWorkInfoDto pixivWorkInfo)
        {
            Dictionary<string, string> headerDic = new Dictionary<string, string>();
            headerDic.Add("Referer", HttpUrl.getPixivUgoiraMetaReferer(pixivWorkInfo.body.illustId));
            PixivUgoiraMetaDto pixivUgoiraMetaDto = getPixivUgoiraMetaDto(pixivWorkInfo.body.illustId);
            string fullZipSavePath = Path.Combine(FilePath.getDownImgSavePath(), $"{StringHelper.get16UUID()}.zip");
            HttpHelper.HttpDownload(pixivUgoiraMetaDto.body.src, fullZipSavePath, headerDic);
            string unZipDirPath = Path.Combine(FilePath.getDownImgSavePath(), pixivWorkInfo.body.illustId);
            ZipHelper.ZipToFile(fullZipSavePath, unZipDirPath);
            DirectoryInfo directoryInfo = new DirectoryInfo(unZipDirPath);
            FileInfo[] files = directoryInfo.GetFiles();
            List<PixivUgoiraMetaFrames> frames = pixivUgoiraMetaDto.body.frames;
            string fullGifSavePath = Path.Combine(FilePath.getDownImgSavePath(), $"{pixivWorkInfo.body.illustId}.gif");
            using (var gif = AnimatedGif.AnimatedGif.Create(fullGifSavePath, 0))
            {
                foreach (var file in files)
                {
                    PixivUgoiraMetaFrames frame = frames.Where(o => o.file.Trim() == file.Name).FirstOrDefault();
                    int delay = frame == null ? 60 : frame.delay;
                    var img = Image.FromFile(file.FullName);
                    gif.AddFrame(img, delay, GifQuality.Bit8);
                    Thread.Sleep(100);
                }
            }
            //string tomcatGifSavePath = FilePath.getGifImgPath();
            //if (Directory.Exists(tomcatGifSavePath) == false) Directory.CreateDirectory(tomcatGifSavePath);
            //string fullTomcatGifSavePath = Path.Combine(tomcatGifSavePath, $"{pixivWorkInfo.body.illustId}.gif");
            //if (File.Exists(fullTomcatGifSavePath)) File.Delete(fullTomcatGifSavePath);
            //File.Copy(fullGifSavePath, fullTomcatGifSavePath);
            return new FileInfo(fullGifSavePath);
        }


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
            workInfoStr.AppendLine($"标题：{pixivWorkInfo.illustTitle}，画师：{pixivWorkInfo.userName}，画师id：{pixivWorkInfo.userId}，大小：{sizeMB}MB，");
            workInfoStr.AppendLine($"收藏：{pixivWorkInfo.bookmarkCount}，赞：{pixivWorkInfo.likeCount}，浏览：{pixivWorkInfo.viewCount}，");
            workInfoStr.AppendLine($"耗时：{costSecond}s，标签图片：{pixivWorkInfo.RelevantCount}张，作品图片:{pixivWorkInfo.pageCount}张");
            workInfoStr.AppendLine($"标签：{getTagsStr(pixivWorkInfo.tags)}");
            workInfoStr.Append(getWorkUrlStr(pixivWorkInfo));
            return workInfoStr.ToString();
        }

        protected string getWorkUrlStr(PixivWorkInfo pixivWorkInfo)
        {
            //if (pixivWorkInfo.isGif())
            //{
            //    return HttpUrl.getTomcatGifUrl(pixivWorkInfo.illustId);
            //}
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

        public PixivSearchDto getPixivSearchDto(string keyword, int pageNo, bool isMatchAll)
        {
            string referer = HttpUrl.getPixivSearchReferer(keyword);
            Dictionary<string, string> headerDic = getPixivHeader(referer);
            string postUrl = HttpUrl.getPixivSearchUrl(keyword, pageNo, isMatchAll);
            string json = HttpHelper.HttpGet(postUrl, headerDic, 10 * 1000);
            return JsonConvert.DeserializeObject<PixivSearchDto>(json);
        }

        public PixivWorkInfoDto getPixivWorkInfoDto(string wordId)
        {
            string referer = HttpUrl.getPixivWorkInfoReferer(wordId);
            Dictionary<string, string> headerDic = getPixivHeader(referer);
            string postUrl = HttpUrl.getPixivWorkInfoUrl(wordId);
            string json = HttpHelper.HttpGet(postUrl, headerDic, 10 * 1000);
            return JsonConvert.DeserializeObject<PixivWorkInfoDto>(json);
        }

        public PixivUserWorkInfoDto getPixivUserWorkInfoDto(string userId)
        {
            string referer = HttpUrl.getPixivUserWorkInfoReferer(userId);
            Dictionary<string, string> headerDic = getPixivHeader(referer);
            string postUrl = HttpUrl.getPixivUserWorkInfoUrl(userId);
            string json = HttpHelper.HttpGet(postUrl, headerDic, 10 * 1000);
            if (string.IsNullOrEmpty(json) == false && json.Contains("\"illusts\":[]"))
            {
                //throw new Exception($"pixiv用户{userId}作品列表illusts为空,cookie可能已经过期");
                return null;
            }
            return JsonConvert.DeserializeObject<PixivUserWorkInfoDto>(json);
        }

        public PixivUserInfoDto getPixivUserInfoDto(string userId)
        {
            string referer = HttpUrl.getPixivUserWorkInfoReferer(userId);
            Dictionary<string, string> headerDic = getPixivHeader(referer);
            string postUrl = HttpUrl.getPixivUserWorkInfoUrl(userId);
            string json = HttpHelper.HttpGet(postUrl, headerDic, 10 * 1000);
            return JsonConvert.DeserializeObject<PixivUserInfoDto>(json);
        }

        public PixivUgoiraMetaDto getPixivUgoiraMetaDto(string wordId)
        {
            string referer = HttpUrl.getPixivUgoiraMetaReferer(wordId);
            Dictionary<string, string> headerDic = getPixivHeader(referer);
            string postUrl = HttpUrl.getPixivUgoiraMetaUrl(wordId);
            string json = HttpHelper.HttpGet(postUrl, headerDic, 10 * 1000);
            return JsonConvert.DeserializeObject<PixivUgoiraMetaDto>(json);
        }


        public Dictionary<string, string> getPixivHeader(string referer)
        {
            Dictionary<string, string> headerDic = new Dictionary<string, string>();
            headerDic.Add("cookie", BotConfig.WebsiteConfig.Pixiv.Cookie);
            headerDic.Add("referer", referer);
            headerDic.Add("accept", "application/json");
            headerDic.Add("sec-fetch-mode", "cors");
            headerDic.Add("sec-fetch-site", "same-origin");
            //headerDic.Add("x-user-id", Setting.Pixiv.XUserId);
            return headerDic;
        }



    }
}
