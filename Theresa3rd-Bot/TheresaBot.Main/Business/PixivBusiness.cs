using System.Text;
using TheresaBot.Main.Common;
using TheresaBot.Main.Dao;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Cache;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Pixiv;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Model.Subscribe;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Business
{
    internal class PixivBusiness : SetuBusiness
    {
        private SubscribeDao subscribeDao;
        private SubscribeRecordDao subscribeRecordDao;

        /// <summary>
        /// p站每页作品数
        /// </summary>
        private const int PageSize = 60;

        /// <summary>
        /// 获取作品信息的线程数
        /// </summary>
        private const int threadCount = 3;

        /// <summary>
        /// 收藏数超过0的作品集
        /// </summary>
        private List<PixivWorkInfo> bookUpList;

        public PixivBusiness()
        {
            bookUpList = new List<PixivWorkInfo>();
            subscribeDao = new SubscribeDao();
            subscribeRecordDao = new SubscribeRecordDao();
        }

        /// <summary>
        /// 根据一个pid获取作品信息
        /// </summary>
        /// <param name="workId"></param>
        /// <returns></returns>
        public async Task<PixivWorkInfo> getPixivWorkInfoAsync(string workId, int? retryTimes = null)
        {
            return await PixivHelper.GetPixivWorkInfoAsync(workId, retryTimes);
        }

        /// <summary>
        /// 随机获取一个指定标签中的作品
        /// </summary>
        /// <param name="includeR18"></param>
        /// <param name="includeAI"></param>
        /// <returns></returns>
        public async Task<PixivWorkInfo> getRandomWorkInTagsAsync(bool includeR18, bool includeAI)
        {
            List<string> tagList = BotConfig.SetuConfig.Pixiv.RandomTags;
            if (tagList is null || tagList.Count == 0) return null;
            string tagName = tagList[new Random().Next(0, tagList.Count)];
            return await getRandomWorkAsync(tagName, includeR18, includeAI);
        }

        /// <summary>
        /// 随机获取一个订阅的画师的作品
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public async Task<PixivWorkInfo> getRandomWorkInSubscribeAsync(long groupId, bool includeR18, bool includeAI)
        {
            int loopUserTimes = 3;
            int loopWorkTimes = 5;
            SubscribeType subscribeType = SubscribeType.P站画师;
            if (BotConfig.SubscribeTaskMap.ContainsKey(subscribeType) == false) return null;
            List<SubscribeTask> subscribeTaskList = BotConfig.SubscribeTaskMap[subscribeType].Where(m => m.GroupIdList.Contains(groupId)).ToList();
            if (subscribeTaskList is null || subscribeTaskList.Count == 0) return null;
            for (int i = 0; i < loopUserTimes; i++)
            {
                int randomUserIndex = RandomHelper.getRandomBetween(0, subscribeTaskList.Count - 1);
                SubscribeTask subscribeTask = subscribeTaskList[randomUserIndex];
                PixivUserProfileTop pixivUserInfo = await PixivHelper.GetPixivUserProfileTopAsync(subscribeTask.SubscribeCode);
                if (pixivUserInfo is null) continue;
                Dictionary<string, PixivUserWorkInfo> illusts = pixivUserInfo.illusts;
                if (illusts is null || illusts.Count == 0) continue;
                List<PixivUserWorkInfo> workList = illusts.Select(o => o.Value).ToList();
                for (int j = 0; j < loopWorkTimes; j++)
                {
                    PixivUserWorkInfo pixivUserWorkInfo = workList[new Random().Next(0, workList.Count)];
                    if (pixivUserWorkInfo.IsImproper) continue;
                    if (pixivUserWorkInfo.hasBanTag() != null) continue;
                    if (pixivUserWorkInfo.IsR18 && includeR18 == false) continue;
                    if (pixivUserWorkInfo.IsAI && includeAI == false) continue;
                    PixivWorkInfo pixivWorkInfo = await PixivHelper.GetPixivWorkInfoAsync(pixivUserWorkInfo.id);
                    if (pixivWorkInfo is null) continue;
                    if (pixivWorkInfo.bookmarkCount < 100) continue;
                    return pixivWorkInfo;
                }
            }
            return null;
        }

        /// <summary>
        /// 随机获取一个关注的画师的作品
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public async Task<PixivWorkInfo> getRandomWorkInFollowAsync(bool includeR18, bool includeAI)
        {
            int eachPage = 24;
            int loopUserTimes = 3;
            int loopWorkTimes = 5;
            long userId = BotConfig.WebsiteConfig.Pixiv.UserId;
            PixivFollow firstFollowDto = await PixivHelper.GetPixivFollowAsync(userId, 0, eachPage);
            int total = firstFollowDto.total;
            int page = (int)Math.Ceiling(Convert.ToDecimal(total) / eachPage);

            int randomPage = new Random().Next(page);
            PixivFollow randomFollow = randomPage == 0 ? firstFollowDto : await PixivHelper.GetPixivFollowAsync(userId, randomPage * eachPage, eachPage);
            if (randomFollow.users is null || randomFollow.users.Count == 0) return null;
            List<PixivFollowUser> followUserList = randomFollow.users;
            List<PixivFollowUser> randomUserList = new List<PixivFollowUser>();
            for (int i = 0; i < loopUserTimes; i++)
            {
                if (followUserList.Count == 0) break;
                PixivFollowUser randomUser = followUserList[new Random().Next(followUserList.Count)];
                randomUserList.Add(randomUser);
                followUserList.Remove(randomUser);
            }

            foreach (PixivFollowUser user in randomUserList)
            {
                PixivUserProfileTop pixivUserInfo = await PixivHelper.GetPixivUserProfileTopAsync(user.userId);
                if (pixivUserInfo is null) continue;
                Dictionary<string, PixivUserWorkInfo> illusts = pixivUserInfo.illusts;
                if (illusts is null || illusts.Count == 0) continue;
                List<PixivUserWorkInfo> workList = illusts.Select(o => o.Value).ToList();
                for (int i = 0; i < loopWorkTimes; i++)
                {
                    PixivUserWorkInfo pixivUserWorkInfo = workList[new Random().Next(0, workList.Count)];
                    if (pixivUserWorkInfo.IsImproper) continue;
                    if (pixivUserWorkInfo.hasBanTag() != null) continue;
                    if (pixivUserWorkInfo.IsR18 && includeR18 == false) continue;
                    if (pixivUserWorkInfo.IsAI && includeAI == false) continue;
                    PixivWorkInfo pixivWorkInfo = await PixivHelper.GetPixivWorkInfoAsync(pixivUserWorkInfo.id);
                    if (pixivWorkInfo is null) continue;
                    if (pixivWorkInfo.bookmarkCount < 100) continue;
                    return pixivWorkInfo;
                }
            }
            return null;
        }

        /// <summary>
        /// 随机获取一个收藏中的作品
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public async Task<PixivWorkInfo> getRandomWorkInBookmarkAsync(bool includeR18, bool includeAI)
        {
            int eachPage = 48;
            int loopPageTimes = 3;
            int loopWorkTimes = 5;
            long userId = BotConfig.WebsiteConfig.Pixiv.UserId;

            PixivBookmarks firstBookmarksDto = await PixivHelper.GetPixivBookmarkAsync(userId, 0, eachPage);
            int total = firstBookmarksDto.total;
            int page = (int)Math.Ceiling(Convert.ToDecimal(total) / eachPage);

            for (int i = 0; i < loopPageTimes; i++)
            {
                int randomPage = RandomHelper.getRandomBetween(0, page - 1);
                PixivBookmarks randomBookmarks = randomPage == 0 ? firstBookmarksDto : await PixivHelper.GetPixivBookmarkAsync(userId, randomPage * eachPage, eachPage);
                if (randomBookmarks is null || randomBookmarks.works is null || randomBookmarks.works.Count == 0) continue;
                List<PixivBookmarksWork> workList = randomBookmarks.works;
                for (int j = 0; j < loopWorkTimes; j++)
                {
                    PixivBookmarksWork randomWork = workList[new Random().Next(0, workList.Count)];
                    if (randomWork.IsImproper()) continue;
                    if (randomWork.hasBanTag() != null) continue;
                    if (randomWork.isR18() && includeR18 == false) continue;
                    if (randomWork.isAI() && includeAI == false) continue;
                    PixivWorkInfo pixivWorkInfo = await PixivHelper.GetPixivWorkInfoAsync(randomWork.id);
                    if (pixivWorkInfo is null) continue;
                    return pixivWorkInfo;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取作品集中的随机一个作品
        /// </summary>
        /// <param name="pixivicSearchDto"></param>
        /// <returns></returns>
        public async Task<PixivWorkInfo> getRandomWorkAsync(string tagNames, bool includeR18, bool includeAI)
        {
            int pageCount = (int)Math.Ceiling(Convert.ToDouble(BotConfig.SetuConfig.Pixiv.MaxScreen) / PageSize);
            if (pageCount < 3) pageCount = 3;

            string searchWord = toPixivSearchWord(tagNames);
            PixivSearch pageOne = await PixivHelper.GetPixivSearchAsync(searchWord, 1, false, includeR18);
            int total = pageOne.getIllust().total;
            int maxPage = MathHelper.getMaxPage(total, PageSize);
            maxPage = maxPage > 1000 ? 1000 : maxPage;
            await Task.Delay(1000);

            //获取随机页中的所有作品
            int[] pageArr = getRandomPageNo(maxPage, pageCount);
            List<PixivIllust> tempIllustList = new List<PixivIllust>();
            foreach (int page in pageArr)
            {
                PixivSearch pixivSearchDto = await PixivHelper.GetPixivSearchAsync(searchWord, page, false, includeR18);
                if (pixivSearchDto?.getIllust()?.data is null) continue;
                tempIllustList.AddRange(pixivSearchDto.getIllust().data);
                await Task.Delay(1000);
            }

            //乱序
            List<PixivIllust> pixivIllustList = new List<PixivIllust>();
            Random random = RandomHelper.getRandom();
            while (tempIllustList.Count > 0)
            {
                PixivIllust randomIllust = tempIllustList[random.Next(0, tempIllustList.Count)];
                pixivIllustList.Add(randomIllust);
                tempIllustList.Remove(randomIllust);
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
                taskList[i % threadCount].Add(pixivIllustList[i]);
            }

            //开启所有线程
            Task[] tasks = new Task[threadCount];
            for (int i = 0; i < taskList.Length; i++)
            {
                tasks[i] = getPixivWorkInfoMethodAsync(taskList[i], includeR18, includeAI);
                await Task.Delay(1000);//将每条线程的间隔错开
            }
            Task.WaitAll(tasks);

            PixivWorkInfo randomWork = bookUpList.OrderByDescending(o => o.bookmarkCount).FirstOrDefault();
            if (randomWork is null) return null;
            randomWork.RelevantCount = total;
            return randomWork;
        }

        /// <summary>
        /// 线程函数
        /// </summary>
        /// <param name="pixivIllustList"></param>
        /// <param name="isScreen"></param>
        private async Task getPixivWorkInfoMethodAsync(List<PixivIllust> pixivIllustList, bool includeR18, bool includeAI)
        {
            for (int i = 0; i < pixivIllustList.Count; i++)
            {
                try
                {
                    if (bookUpList.Count > 0) return;
                    PixivWorkInfo pixivWorkInfo = await PixivHelper.GetPixivWorkInfoAsync(pixivIllustList[i].id, 0);
                    if (pixivWorkInfo.IsImproper) continue;
                    if (pixivWorkInfo.hasBanTag() is not null) continue;
                    if (pixivWorkInfo.IsR18 && includeR18 == false) continue;
                    if (pixivWorkInfo.IsAI && includeAI == false) continue;
                    if (checkRandomWorkIsOk(pixivWorkInfo) == false) continue;
                    lock (bookUpList) bookUpList.Add(pixivWorkInfo);
                }
                catch (Exception)
                {
                }
                finally
                {
                    await Task.Delay(1000);
                }
            }
        }

        /// <summary>
        /// 判断插画质量是否符合
        /// </summary>
        /// <param name="pixivWorkInfo"></param>
        /// <returns></returns>
        public bool checkRandomWorkIsOk(PixivWorkInfo pixivWorkInfo)
        {
            if (pixivWorkInfo is null) return false;
            double target = getTargetByWorkType(pixivWorkInfo);
            bool isNotBanTag = pixivWorkInfo.hasBanTag() is null;
            bool isBookmarkOk = pixivWorkInfo.bookmarkCount >= BotConfig.SetuConfig.Pixiv.MinBookmark * target;
            bool isBookRateOk = pixivWorkInfo.bookmarkRate >= BotConfig.SetuConfig.Pixiv.MinBookRate * target;
            return isNotBanTag && isBookmarkOk && isBookRateOk;
        }

        /// <summary>
        /// 判断插画质量是否符合
        /// </summary>
        /// <param name="pixivWorkInfo"></param>
        /// <returns></returns>
        public bool checkTagWorkIsOk(PixivWorkInfo pixivWorkInfo)
        {
            if (pixivWorkInfo is null) return false;
            double target = getTargetByWorkType(pixivWorkInfo);
            TimeSpan timeSpan = DateTime.Now.Subtract(pixivWorkInfo.createDate);
            int totalHours = (int)Math.Ceiling(timeSpan.TotalHours);
            bool isBookmarkOk = pixivWorkInfo.bookmarkCount >= BotConfig.SubscribeConfig.PixivTag.MinBookmark * target;
            bool isBookPerHourOk = pixivWorkInfo.bookmarkCount >= totalHours * BotConfig.SubscribeConfig.PixivTag.MinBookPerHour * target;
            bool isBookRateOk = pixivWorkInfo.bookmarkRate >= BotConfig.SubscribeConfig.PixivTag.MinBookRate * target;
            return isBookmarkOk && isBookPerHourOk && isBookRateOk && totalHours > 0;
        }

        /// <summary>
        /// 通过作品类型获取不同的指标
        /// </summary>
        /// <param name="pixivWorkInfo"></param>
        /// <returns></returns>
        private double getTargetByWorkType(PixivWorkInfo pixivWorkInfo)
        {
            if (pixivWorkInfo.IsAI) return BotConfig.PixivConfig.AITarget;
            if (pixivWorkInfo.IsR18) return BotConfig.PixivConfig.R18Target;
            return BotConfig.PixivConfig.GeneralTarget;
        }

        /// <summary>
        /// 从最大页数maxPage获取pageCount个随机页码
        /// </summary>
        /// <param name="maxPage"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        public int[] getRandomPageNo(int maxPage, int pageCount)
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


        /*-------------------------------------------------------------获取最新作品--------------------------------------------------------------------------*/

        /// <summary>
        /// 获取画师的最新作品
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="subscribeId"></param>
        /// <param name="getCount"></param>
        /// <returns></returns>
        public async Task<List<PixivSubscribe>> getUserNewestAsync(string userId, int subscribeId, int getCount = 1)
        {
            List<PixivSubscribe> pixivSubscribeList = new List<PixivSubscribe>();
            PixivUserProfileTop pixivUserInfo = await PixivHelper.GetPixivUserProfileTopAsync(userId);
            if (pixivUserInfo is null) return pixivSubscribeList;
            Dictionary<string, PixivUserWorkInfo> illusts = pixivUserInfo.illusts;
            if (illusts is null || illusts.Count == 0) return pixivSubscribeList;
            List<PixivUserWorkInfo> workInfoList = illusts.Select(o => o.Value).OrderByDescending(o => o.createDate).ToList();
            foreach (PixivUserWorkInfo userWork in workInfoList)
            {
                if (pixivSubscribeList.Count >= getCount) break;
                if (userWork is null) continue;
                PixivWorkInfo pixivWorkInfo = await PixivHelper.GetPixivWorkInfoAsync(userWork.id);
                if (pixivWorkInfo is null) continue;
                SubscribeRecordPO subscribeRecord = toSubscribeRecord(pixivWorkInfo, subscribeId);
                PixivSubscribe pixivSubscribe = new PixivSubscribe(subscribeRecord, pixivWorkInfo, null);
                pixivSubscribeList.Add(pixivSubscribe);
            }
            return pixivSubscribeList;
        }

        /// <summary>
        /// 获取画师的作品合集
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="subscribeId"></param>
        /// <param name="getCount"></param>
        /// <returns></returns>
        public async Task<PixivUserProfileInfo> getUserProfileInfoAsync(string userId, long groupId)
        {
            int eachPage = 40;
            int cacheSeconds = BotConfig.SetuConfig.PixivUser.CacheSeconds;
            int maxScan = BotConfig.SetuConfig.PixivUser.MaxScan;
            bool isShowR18 = groupId.IsShowR18Setu();
            List<PixivUserWorkInfo> workList = new List<PixivUserWorkInfo>();
            PixivUserProfileTop profileTop = await PixivHelper.GetPixivUserProfileTopAsync(userId);
            PixivUserProfileAll profileAll = await PixivHelper.GetPixivUserProfileAllAsync(userId);
            Dictionary<int, object> illusts = profileAll?.illusts;
            if (illusts is null || illusts.Count == 0) return new(userId, profileTop.UserName, cacheSeconds);
            List<int> workIds = illusts.Select(o => o.Key).Where(o => o > 0).OrderByDescending(o => o).Take(maxScan).ToList();
            int startIndex = 0;
            while (startIndex < workIds.Count)
            {
                List<int> pageList = workIds.Skip(startIndex).Take(eachPage).ToList();
                var userWorks = await PixivHelper.GetPixivUserProfileIllustsAsync(userId, pageList, startIndex == 0);
                workList.AddRange(userWorks.works.Values);
                startIndex += eachPage;
            }
            workList = workList.Where(o => o.IsImproper == false).ToList();
            workList = workList.Where(o => o.hasBanTag() is null).ToList();
            workList = workList.Where(o => isShowR18 || o.IsR18 == false).ToList();
            workList = workList.OrderByDescending(o => Convert.ToInt32(o.id)).ToList();
            List<PixivProfileDetail> profileDetails = new List<PixivProfileDetail>();
            for (int i = 0; i < workList.Count; i++) profileDetails.Add(new PixivProfileDetail(workList[i], i + 1));
            return new(userId, profileTop.UserName, cacheSeconds, profileDetails);
        }

        /// <summary>
        /// 获取订阅画师的最新作品
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="subscribeId"></param>
        /// <param name="getCount"></param>
        /// <returns></returns>
        public async Task<List<PixivSubscribe>> scanUserWorkAsync(SubscribeTask subscribeTask, PixivUserScanReport scanReport, Func<PixivSubscribe, Task> pushAsync = null)
        {
            int index = 0;
            int getCount = 5;
            string userId = subscribeTask.SubscribeCode;
            int subscribeId = subscribeTask.SubscribeId;
            List<long> groupIds = subscribeTask.GroupIdList;
            bool isShowAIs = groupIds.IsShowAISetu();
            bool isShowR18s = groupIds.IsShowR18Setu();
            PixivUserProfileTop pixivUserInfo = await PixivHelper.GetPixivUserProfileTopAsync(userId);
            if (pixivUserInfo is null) return new();
            Dictionary<string, PixivUserWorkInfo> illusts = pixivUserInfo?.illusts;
            if (illusts is null || illusts.Count == 0) return new();
            int shelfLife = BotConfig.SubscribeConfig.PixivUser.ShelfLife;
            List<PixivSubscribe> pixivSubscribeList = new List<PixivSubscribe>();
            List<PixivUserWorkInfo> userWorks = illusts.Select(o => o.Value).OrderByDescending(o => o.createDate).ToList();
            foreach (PixivUserWorkInfo userWork in userWorks)
            {
                try
                {
                    if (++index > getCount) break;
                    if (userWork is null) continue;
                    if (string.IsNullOrWhiteSpace(userWork.id)) continue;
                    if (userWork.IsImproper) continue;
                    if (userWork.hasBanTag() is not null) continue;
                    if (isShowAIs == false && userWork.IsAI) continue;
                    if (isShowR18s == false && userWork.IsR18) continue;
                    if (shelfLife > 0 && userWork.createDate < DateTime.Now.AddSeconds(-1 * shelfLife)) break;
                    if (subscribeRecordDao.checkExists(subscribeTask.SubscribeType, userWork.id)) continue;
                    scanReport.ScanWork++;
                    PixivWorkInfo pixivWorkInfo = await PixivHelper.GetPixivWorkInfoAsync(userWork.id, 0);
                    if (pixivWorkInfo is null) continue;
                    if (pixivWorkInfo.IsImproper) continue;
                    if (pixivWorkInfo.hasBanTag() is not null) continue;
                    SubscribeRecordPO subscribeRecord = toSubscribeRecord(pixivWorkInfo, subscribeId);
                    PixivSubscribe pixivSubscribe = new PixivSubscribe(subscribeRecord, pixivWorkInfo, subscribeTask);
                    if (pushAsync is not null)
                    {
                        await pushAsync(pixivSubscribe);
                        await insertSubscribeRecord(pixivSubscribe);
                    }
                    else
                    {
                        pixivSubscribeList.Add(pixivSubscribe);
                    }
                }
                catch (Exception ex)
                {
                    scanReport.ErrorWork++;
                    LogHelper.Error(ex);
                }
                finally
                {
                    await Task.Delay(1000);
                }
            }
            return pixivSubscribeList;
        }

        /// <summary>
        /// 获取订阅标签的最新作品
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="subscribeId"></param>
        /// <returns></returns>
        public async Task<List<PixivSubscribe>> scanTagWorkAsync(SubscribeTask subscribeTask, PixivTagScanReport scanReport, Func<PixivSubscribe, Task> pushAsync = null)
        {
            string tagNames = subscribeTask.SubscribeCode;
            int subscribeId = subscribeTask.SubscribeId;
            List<long> groupIds = subscribeTask.GroupIdList;
            bool isShowAIs = groupIds.IsShowAISetu();
            bool isShowR18s = groupIds.IsShowR18Setu();
            string searchWord = toPixivSearchWord(tagNames);
            int maxScan = BotConfig.SubscribeConfig.PixivTag.MaxScan;
            int shelfLife = BotConfig.SubscribeConfig.PixivTag.ShelfLife;
            List<PixivSubscribe> pixivSubscribeList = new List<PixivSubscribe>();
            List<PixivIllust> illutsList = await getTagIllustListAsync(searchWord, maxScan, shelfLife);
            foreach (PixivIllust illuts in illutsList)
            {
                try
                {
                    if (illuts is null) continue;
                    if (string.IsNullOrWhiteSpace(illuts.id)) continue;
                    if (illuts.IsImproper) continue;
                    if (illuts.hasBanTag() is not null) continue;
                    if (isShowAIs == false && illuts.IsAI) continue;
                    if (isShowR18s == false && illuts.IsR18) continue;
                    if (shelfLife > 0 && illuts.createDate < DateTime.Now.AddSeconds(-1 * shelfLife)) break;
                    if (subscribeRecordDao.checkExists(subscribeTask.SubscribeType, illuts.id)) continue;
                    scanReport.ScanWork++;
                    PixivWorkInfo pixivWorkInfo = await PixivHelper.GetPixivWorkInfoAsync(illuts.id, 0);
                    if (pixivWorkInfo is null) continue;
                    if (pixivWorkInfo.IsImproper) continue;
                    if (pixivWorkInfo.hasBanTag() is not null) continue;
                    if (checkTagWorkIsOk(pixivWorkInfo) == false) continue;
                    SubscribeRecordPO subscribeRecord = toSubscribeRecord(pixivWorkInfo, subscribeId);
                    PixivSubscribe pixivSubscribe = new PixivSubscribe(subscribeRecord, pixivWorkInfo, subscribeTask);
                    if (pushAsync is not null)
                    {
                        await pushAsync(pixivSubscribe);
                        await insertSubscribeRecord(pixivSubscribe);
                    }
                    else
                    {
                        pixivSubscribeList.Add(pixivSubscribe);
                    }
                }
                catch (Exception ex)
                {
                    scanReport.ErrorWork++;
                    LogHelper.Error(ex);
                }
                finally
                {
                    await Task.Delay(1000);
                }
            }
            return pixivSubscribeList;
        }

        /// <summary>
        /// 获取pixiv账号中关注用户的最新作品
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="subscribeId"></param>
        /// <param name="getCount"></param>
        /// <returns></returns>
        public async Task<List<PixivSubscribe>> scanFollowWorkAsync(PixivUserScanReport scanReport, Func<PixivSubscribe, Task> pushAsync = null)
        {
            int pageIndex = 1;
            List<long> groupIds = BotConfig.PermissionsConfig.SubscribeGroups;
            bool isShowAIs = groupIds.IsShowAISetu();
            bool isShowR18s = groupIds.IsShowR18Setu();
            PixivFollowLatest pageOne = await PixivHelper.GetPixivFollowLatestAsync(pageIndex);
            if (pageOne?.page?.ids is null) return new();
            int shelfLife = BotConfig.SubscribeConfig.PixivTag.ShelfLife;
            List<PixivSubscribe> pixivSubscribeList = new List<PixivSubscribe>();
            List<int> wordIdList = pageOne.page.ids.OrderByDescending(o => o).ToList();
            foreach (int workId in wordIdList)
            {
                try
                {
                    if (workId <= 0) continue;
                    if (subscribeRecordDao.checkExists(SubscribeType.P站画师, workId.ToString())) continue;
                    scanReport.ScanWork++;
                    PixivWorkInfo pixivWorkInfo = await PixivHelper.GetPixivWorkInfoAsync(workId.ToString(), 0);
                    if (pixivWorkInfo is null) continue;
                    if (pixivWorkInfo.IsImproper) continue;
                    if (pixivWorkInfo.hasBanTag() is not null) continue;
                    if (isShowAIs == false && pixivWorkInfo.IsAI) continue;
                    if (isShowR18s == false && pixivWorkInfo.IsR18) continue;
                    if (shelfLife > 0 && pixivWorkInfo.createDate < DateTime.Now.AddSeconds(-1 * shelfLife)) break;
                    SubscribePO dbSubscribe = getOrInsertUserSubscribe(pixivWorkInfo);
                    SubscribeRecordPO subscribeRecord = toSubscribeRecord(pixivWorkInfo, dbSubscribe.Id);
                    PixivSubscribe pixivSubscribe = new PixivSubscribe(subscribeRecord, pixivWorkInfo, null);
                    if (pushAsync is not null)
                    {
                        await pushAsync(pixivSubscribe);
                        await insertSubscribeRecord(pixivSubscribe);
                    }
                    else
                    {
                        pixivSubscribeList.Add(pixivSubscribe);
                    }
                }
                catch (Exception ex)
                {
                    scanReport.ErrorWork++;
                    LogHelper.Error(ex);
                }
                finally
                {
                    await Task.Delay(1000);
                }
            }
            return pixivSubscribeList;
        }


        /// <summary>
        /// 添加订阅发送记录
        /// </summary>
        /// <param name="pixivSubscribes"></param>
        public async Task insertSubscribeRecord(List<PixivSubscribe> pixivSubscribes)
        {
            foreach (var subscribe in pixivSubscribes)
            {
                await insertSubscribeRecord(subscribe);
            }
        }

        /// <summary>
        /// 添加订阅发送记录
        /// </summary>
        /// <param name="pixivSubscribe"></param>
        public async Task insertSubscribeRecord(PixivSubscribe pixivSubscribe)
        {
            try
            {
                subscribeRecordDao.Insert(pixivSubscribe.SubscribeRecord);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }

        /// <summary>
        /// 根据最大扫描数量,读取该数量的作品列表
        /// </summary>
        /// <param name="searchWord"></param>
        /// <param name="maxScan"></param>
        /// <returns></returns>
        private async Task<List<PixivIllust>> getTagIllustListAsync(string searchWord, int maxScan, int shelfLife)
        {
            int maxPage = MathHelper.getMaxPage(maxScan, PageSize);
            List<PixivIllust> pixivIllustList = new List<PixivIllust>();
            for (int i = 1; i <= maxPage; i++)
            {
                if (pixivIllustList.Count >= maxScan) break;
                PixivSearch pixivSearch = await PixivHelper.GetPixivSearchAsync(searchWord, i, false, true);
                List<PixivIllust> illusts = pixivSearch?.getIllust()?.data;
                if (illusts is null || illusts.Count == 0) break;
                pixivIllustList.AddRange(illusts);
                if (illusts.Count < PageSize) break;
                if (shelfLife > 0 && illusts.Last().createDate < DateTime.Now.AddSeconds(-1 * shelfLife)) break;
            }
            return pixivIllustList.OrderByDescending(o => o.createDate).Take(maxScan).ToList();
        }

        /// <summary>
        /// 创建或返回一个订阅
        /// </summary>
        /// <param name="pixivWorkInfo"></param>
        /// <returns></returns>
        private SubscribePO getOrInsertUserSubscribe(PixivWorkInfo pixivWorkInfo)
        {
            string userId = pixivWorkInfo.userId.ToString();
            string userName = StringHelper.filterEmoji(pixivWorkInfo.userName)?.filterEmoji().cutString(50);
            SubscribePO dbSubscribe = subscribeDao.getSubscribe(userId, SubscribeType.P站画师);
            if (dbSubscribe != null) return dbSubscribe;
            dbSubscribe = new SubscribePO();
            dbSubscribe.SubscribeCode = userId;
            dbSubscribe.SubscribeName = userName;
            dbSubscribe.SubscribeType = SubscribeType.P站画师;
            dbSubscribe.SubscribeSubType = 0;
            dbSubscribe.CreateDate = DateTime.Now;
            return subscribeDao.Insert(dbSubscribe);
        }

        private SubscribeRecordPO toSubscribeRecord(PixivWorkInfo workInfo, int subscribeId)
        {
            SubscribeRecordPO subscribeRecord = new SubscribeRecordPO(subscribeId);
            subscribeRecord.Title = StringHelper.filterEmoji(workInfo.illustTitle);
            subscribeRecord.Content = subscribeRecord.Title;
            subscribeRecord.CoverUrl = HttpUrl.getPixivWorkInfoUrl(workInfo.illustId.ToString());
            subscribeRecord.LinkUrl = HttpUrl.getPixivWorkInfoUrl(workInfo.illustId.ToString());
            subscribeRecord.DynamicCode = workInfo.illustId.ToString();
            subscribeRecord.DynamicType = SubscribeDynamicType.插画;
            return subscribeRecord;
        }

        /*-------------------------------------------------------------获取关注列表--------------------------------------------------------------------------*/
        /// <summary>
        /// 获取订阅账号中已关注的用户列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<PixivFollowUser>> getFollowUserList()
        {
            int offset = 0;
            int eachPage = 24;
            long userId = BotConfig.WebsiteConfig.Pixiv.UserId;
            List<PixivFollowUser> followUserList = new List<PixivFollowUser>();
            PixivFollow firstFollowDto = await PixivHelper.GetPixivFollowAsync(userId, 0, eachPage);
            int total = firstFollowDto.total;
            int page = (int)Math.Ceiling(Convert.ToDecimal(total) / eachPage);
            for (int i = 0; i < page; i++)
            {
                PixivFollow pixivFollowDto = await PixivHelper.GetPixivFollowAsync(userId, offset, eachPage);
                foreach (var item in pixivFollowDto.users)
                {
                    if (item is null) continue;
                    followUserList.Add(item);
                }
                offset += eachPage;
            }
            return followUserList;
        }


        /*-------------------------------------------------------------作品信息--------------------------------------------------------------------------*/

        public string getSetuRemindMsg(string template, long todayLeft)
        {
            template = template?.Trim()?.TrimLine();
            template = template.Replace("{MemberCD}", BotConfig.SetuConfig.MemberCD.ToString());
            template = template.Replace("{RevokeInterval}", BotConfig.SetuConfig.RevokeInterval.ToString());
            template = template.Replace("{TodayLeft}", todayLeft.ToString());
            return template;
        }

        public string getUserPushRemindMsg(PixivSubscribe pixivSubscribe)
        {
            string userName = pixivSubscribe.PixivWorkInfo.userName;
            string template = BotConfig.SubscribeConfig?.PixivUser?.Template?.Trim()?.TrimLine();
            if (string.IsNullOrWhiteSpace(template))
            {
                return $"pixiv画师[{userName}]发布了新作品：";
            }
            else
            {
                return template.Replace("{UserName}", userName);
            }
        }

        public string getTagPushRemindMsg(PixivSubscribe pixivSubscribe)
        {
            string tagName = pixivSubscribe.SubscribeTask.SubscribeName;
            string template = BotConfig.SubscribeConfig?.PixivTag?.Template?.Trim()?.TrimLine();
            if (string.IsNullOrWhiteSpace(template))
            {
                return $"pixiv标签[{tagName}]发布了新作品：";
            }
            else
            {
                return template.Replace("{TagName}", tagName);
            }
        }

        public string getWorkInfo(PixivWorkInfo pixivWorkInfo)
        {
            string template = BotConfig.PixivConfig?.Template?.Trim()?.TrimLine();
            if (string.IsNullOrWhiteSpace(template)) return getDefaultWorkInfo(pixivWorkInfo);
            template = template.Replace("{MemberCD}", BotConfig.SetuConfig.MemberCD.ToString());
            template = template.Replace("{RevokeInterval}", BotConfig.SetuConfig.RevokeInterval.ToString());
            template = template.Replace("{IllustTitle}", pixivWorkInfo.illustTitle);
            template = template.Replace("{PixivId}", pixivWorkInfo.illustId.ToString());
            template = template.Replace("{UserName}", pixivWorkInfo.userName);
            template = template.Replace("{UserId}", pixivWorkInfo.userId.ToString());
            template = template.Replace("{CreateTime}", pixivWorkInfo.createDate.ToSimpleString());
            template = template.Replace("{BookmarkCount}", pixivWorkInfo.bookmarkCount.ToString());
            template = template.Replace("{LikeCount}", pixivWorkInfo.likeCount.ToString());
            template = template.Replace("{ViewCount}", pixivWorkInfo.viewCount.ToString());
            template = template.Replace("{RelevantCount}", pixivWorkInfo.RelevantCount.ToString());
            template = template.Replace("{PageCount}", pixivWorkInfo.pageCount.ToString());
            template = template.Replace("{Tags}", pixivWorkInfo.Tags.JoinPixivTagsStr(BotConfig.PixivConfig.TagShowMaximum));
            template = template.Replace("{Urls}", BusinessHelper.JoinPixivImgOriginUrls(pixivWorkInfo));
            return template;
        }

        public string getDefaultWorkInfo(PixivWorkInfo pixivWorkInfo)
        {
            StringBuilder workInfoStr = new StringBuilder();
            workInfoStr.AppendLine($"本条数据来源于Pixiv~");
            workInfoStr.AppendLine($"标题：{pixivWorkInfo.illustTitle}，PixivId：{pixivWorkInfo.illustId}，画师：{pixivWorkInfo.userName}，画师id：{pixivWorkInfo.userId}，发布时间：{pixivWorkInfo.createDate.ToSimpleString()}，");
            workInfoStr.AppendLine($"收藏：{pixivWorkInfo.bookmarkCount}，赞：{pixivWorkInfo.likeCount}，浏览：{pixivWorkInfo.viewCount}，");
            workInfoStr.AppendLine($"标签图片：{pixivWorkInfo.RelevantCount}张，作品图片:{pixivWorkInfo.pageCount}张");
            workInfoStr.AppendLine($"标签：{pixivWorkInfo.Tags.JoinPixivTagsStr(BotConfig.PixivConfig.TagShowMaximum)}");
            workInfoStr.Append(BusinessHelper.JoinPixivImgOriginUrls(pixivWorkInfo));
            return workInfoStr.ToString();
        }

        public string getUserProfileMsg(string userName, string template)
        {
            template = template?.Trim()?.TrimLine();
            if (string.IsNullOrWhiteSpace(template)) return getDefaultUserProfile(userName);
            template = template.Replace("{UserName}", userName);
            template = template.Replace("{CacheSeconds}", BotConfig.PixivRankingConfig.CacheSeconds.ToString());
            return template;
        }

        public string getDefaultUserProfile(string userName)
        {
            return $"画师{userName}作品合集，数据缓存{BotConfig.SetuConfig.PixivUser.CacheSeconds}秒";
        }


        public List<SetuContent> getNumAndPids(PixivUserProfileInfo profileInfo, int eachPage)
        {
            int startIndex = 0;
            if (eachPage <= 0) return new();
            var details = profileInfo.ProfileDetails.ToList();
            var pidList = details.Select(o => $"No.{o.No.ToString().PadRight(3, ' ')} {o.WorkInfo.id}").ToList();
            List<SetuContent> rankContents = new List<SetuContent>();
            while (startIndex < pidList.Count)
            {
                var pageList = pidList.Skip(startIndex).Take(eachPage).ToList();
                rankContents.Add(new(String.Join("\r\n", pageList)));
                startIndex += eachPage;
            }
            return rankContents;
        }

        /// <summary>
        /// 将用户输入的关键词转换为pixiv可搜索的word
        /// </summary>
        /// <param name="tagNames"></param>
        /// <returns></returns>
        public string toPixivSearchWord(string tagNames)
        {
            tagNames = tagNames.Trim().Replace("(", "（").Replace(")", "）");
            string[] andArr = tagNames.Split(new char[] { ' ', '+' }, StringSplitOptions.RemoveEmptyEntries);
            if (andArr is null || andArr.Length == 0) return tagNames;
            StringBuilder searchBuilder = new StringBuilder();
            foreach (string andStr in andArr)
            {
                if (string.IsNullOrEmpty(andStr)) continue;
                string[] orArr = andStr.Split(new char[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries);
                string appendWord = orArr.Length > 1 ? $"({string.Join(" OR ", orArr)})" : orArr[0];
                if (searchBuilder.Length > 0) searchBuilder.Append(" ");
                searchBuilder.Append(appendWord);
            }
            return searchBuilder.ToString();
        }

    }
}
