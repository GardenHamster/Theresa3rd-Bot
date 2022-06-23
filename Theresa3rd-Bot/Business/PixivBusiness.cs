using AnimatedGif;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
        /// 获取作品信息的线程数
        /// </summary>
        private const int threadCount = 3;

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

        /// <summary>
        /// 随机获取一个指定标签中的作品
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public async Task<PixivWorkInfoDto> getRandomWorkInTagsAsync(bool includeR18)
        {
            List<string> tagList = BotConfig.SetuConfig.Pixiv.RandomTags;
            if (tagList == null || tagList.Count == 0) return null;
            string tagName = tagList[new Random().Next(0, tagList.Count)];
            return await getRandomWorkAsync(tagName, includeR18);
        }

        /// <summary>
        /// 随机获取一个订阅的画师的作品
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public async Task<PixivWorkInfoDto> getRandomWorkInSubscribeAsync(long groupId)
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
                PixivUserWorkInfoDto pixivWorkInfo = await PixivHelper.GetPixivUserWorkInfoAsync(subscribeTask.SubscribeCode);
                if (pixivWorkInfo == null) continue;
                Dictionary<string, PixivUserWorkInfo> illusts = pixivWorkInfo.body.illusts;
                if (illusts == null || illusts.Count == 0) continue;
                List<PixivUserWorkInfo> workList = illusts.Select(o => o.Value).ToList();
                for (int j = 0; j < loopWorkTimes; j++)
                {
                    PixivUserWorkInfo pixivUserWorkInfo = workList[new Random().Next(0, workList.Count)];
                    if (pixivUserWorkInfo.isR18() && groupId.IsShowR18() == false) continue;
                    PixivWorkInfoDto pixivWorkInfoDto = await PixivHelper.GetPixivWorkInfoAsync(pixivUserWorkInfo.id);
                    await Task.Delay(500);
                    if (pixivWorkInfoDto == null) continue;
                    if (pixivWorkInfoDto.body.bookmarkCount < 100) continue;
                    return pixivWorkInfoDto;
                }
            }
            return null;
        }

        /// <summary>
        /// 随机获取一个关注的画师的作品
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public async Task<PixivWorkInfoDto> getRandomWorkInFollowAsync(long groupId)
        {
            int eachPage = 24;
            int loopUserTimes = 3;
            int loopWorkTimes = 5;
            long userId = BotConfig.WebsiteConfig.Pixiv.UserId;
            PixivFollowDto firstFollowDto = await PixivHelper.GetPixivFollowAsync(userId, 0, eachPage);
            int total = firstFollowDto.body.total;
            int page = (int)Math.Ceiling(Convert.ToDecimal(total) / eachPage);
            await Task.Delay(1000);

            int randomPage = new Random().Next(page);
            PixivFollowDto randomFollow = randomPage == 0 ? firstFollowDto : await PixivHelper.GetPixivFollowAsync(userId, randomPage * eachPage, eachPage);
            if (randomFollow.body.users == null || randomFollow.body.users.Count == 0) return null;
            List<PixivFollowUser> followUserList = randomFollow.body.users;
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
                PixivUserWorkInfoDto pixivWorkInfo = await PixivHelper.GetPixivUserWorkInfoAsync(user.userId);
                if (pixivWorkInfo == null) continue;
                Dictionary<string, PixivUserWorkInfo> illusts = pixivWorkInfo.body.illusts;
                if (illusts == null || illusts.Count == 0) continue;
                List<PixivUserWorkInfo> workList = illusts.Select(o => o.Value).ToList();
                for (int i = 0; i < loopWorkTimes; i++)
                {
                    PixivUserWorkInfo pixivUserWorkInfo = workList[new Random().Next(0, workList.Count)];
                    if (pixivUserWorkInfo.isR18() && groupId.IsShowR18() == false) continue;
                    PixivWorkInfoDto pixivWorkInfoDto = await PixivHelper.GetPixivWorkInfoAsync(pixivUserWorkInfo.id);
                    await Task.Delay(500);
                    if (pixivWorkInfoDto == null) continue;
                    if (pixivWorkInfoDto.body.bookmarkCount < 100) continue;
                    return pixivWorkInfoDto;
                }
            }
            return null;
        }

        /// <summary>
        /// 随机获取一个收藏中的作品
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public async Task<PixivWorkInfoDto> getRandomWorkInBookmarkAsync(long groupId)
        {
            int eachPage = 48;
            int loopPageTimes = 3;
            int loopWorkTimes = 5;
            long userId = BotConfig.WebsiteConfig.Pixiv.UserId;

            PixivBookmarksDto firstBookmarksDto = await PixivHelper.GetPixivBookmarkAsync(userId, 0, eachPage);
            int total = firstBookmarksDto.body.total;
            int page = (int)Math.Ceiling(Convert.ToDecimal(total) / eachPage);
            await Task.Delay(1000);

            for (int i = 0; i < loopPageTimes; i++)
            {
                int randomPage = RandomHelper.getRandomBetween(0, page - 1);
                PixivBookmarksDto randomBookmarks = randomPage == 0 ? firstBookmarksDto : await PixivHelper.GetPixivBookmarkAsync(userId, randomPage * eachPage, eachPage);
                if (randomBookmarks.body.works == null || randomBookmarks.body.works.Count == 0) continue;
                List<PixivBookmarksWork> workList = randomBookmarks.body.works;
                for (int j = 0; j < loopWorkTimes; j++)
                {
                    PixivBookmarksWork randomWork = workList[new Random().Next(0, workList.Count)];
                    if (randomWork.isR18() && groupId.IsShowR18() == false) continue;
                    PixivWorkInfoDto pixivWorkInfoDto = await PixivHelper.GetPixivWorkInfoAsync(randomWork.id);
                    await Task.Delay(500);
                    if (pixivWorkInfoDto == null) continue;
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
        public async Task<PixivWorkInfoDto> getRandomWorkAsync(string tagName, bool includeR18)
        {
            int pageCount = (int)Math.Ceiling(Convert.ToDouble(BotConfig.SetuConfig.Pixiv.MaxScreen) / pixivPageSize);
            if (pageCount < 3) pageCount = 3;

            PixivSearchDto pageOne = await PixivHelper.GetPixivSearchAsync(tagName, 1, false, includeR18);
            int total = pageOne.body.getIllust().total;
            int maxPage = (int)Math.Ceiling(Convert.ToDecimal(total) / pixivPageSize);
            maxPage = maxPage > 1000 ? 1000 : maxPage;
            Thread.Sleep(1000);

            //获取随机页中的所有作品
            int[] pageArr = getRandomPageNo(maxPage, pageCount);
            List<PixivIllust> tempIllustList = new List<PixivIllust>();
            foreach (int page in pageArr)
            {
                PixivSearchDto pixivSearchDto = await PixivHelper.GetPixivSearchAsync(tagName, page, false, includeR18);
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
        public async Task getPixivWorkInfoMethodAsync(List<PixivIllust> pixivIllustList, bool includeR18)
        {
            for (int i = 0; i < pixivIllustList.Count; i++)
            {
                try
                {
                    if (bookUpList.Count > 0) return;
                    PixivWorkInfoDto pixivWorkInfo = await PixivHelper.GetPixivWorkInfoAsync(pixivIllustList[i].id);
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
        public bool checkRandomWorkIsOk(PixivWorkInfoDto pixivWorkInfo)
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
        public bool checkTagWorkIsOk(PixivWorkInfoDto pixivWorkInfo)
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
            PixivUserWorkInfoDto pixivWorkInfo = await PixivHelper.GetPixivUserWorkInfoAsync(userId);
            if (pixivWorkInfo == null) return pixivSubscribeList;
            Dictionary<string, PixivUserWorkInfo> illusts = pixivWorkInfo.body.illusts;
            if (illusts == null || illusts.Count == 0) return pixivSubscribeList;
            foreach (KeyValuePair<string, PixivUserWorkInfo> workInfo in illusts)
            {
                if (++index > getCount) break;
                if (workInfo.Value == null) continue;
                PixivWorkInfoDto pixivWorkInfoDto = await PixivHelper.GetPixivWorkInfoAsync(workInfo.Value.id);
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
            PixivUserWorkInfoDto pixivWorkInfo = await PixivHelper.GetPixivUserWorkInfoAsync(userId);
            if (pixivWorkInfo == null) return pixivSubscribeList;
            Dictionary<string, PixivUserWorkInfo> illusts = pixivWorkInfo.body.illusts;
            if (illusts == null || illusts.Count == 0) return pixivSubscribeList;
            foreach (KeyValuePair<string, PixivUserWorkInfo> workInfo in illusts)
            {
                if (++index > getCount) break;
                if (workInfo.Value == null) continue;
                SubscribeRecordPO dbSubscribe = subscribeRecordDao.checkExists(subscribeId, workInfo.Value.id);
                if (dbSubscribe != null) continue;
                PixivWorkInfoDto pixivWorkInfoDto = await PixivHelper.GetPixivWorkInfoAsync(workInfo.Value.id);
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
        public async Task<List<PixivSubscribe>> getPixivTagSubscribeAsync(string tagName, int subscribeId, bool includeR18)
        {
            PixivSearchDto pageOne = await PixivHelper.GetPixivSearchAsync(tagName, 1, false, includeR18);
            List<PixivSubscribe> pixivSubscribeList = new List<PixivSubscribe>();
            if (pageOne == null) return pixivSubscribeList;
            foreach (PixivIllust item in pageOne.body.getIllust().data)
            {
                int shelfLife = BotConfig.SubscribeConfig.PixivTag.ShelfLife;
                if (shelfLife > 0 && item.createDate < DateTime.Now.AddSeconds(-1 * shelfLife)) continue;

                PixivWorkInfoDto pixivWorkInfoDto = await PixivHelper.GetPixivWorkInfoAsync(item.id);
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
            PixivFollowDto firstFollowDto = await PixivHelper.GetPixivFollowAsync(userId, 0, eachPage);
            int total = firstFollowDto.body.total;
            int page = (int)Math.Ceiling(Convert.ToDecimal(total) / eachPage);
            await Task.Delay(1000);
            for (int i = 0; i < page; i++)
            {
                PixivFollowDto pixivFollowDto = await PixivHelper.GetPixivFollowAsync(userId, offset, eachPage);
                foreach (var item in pixivFollowDto.body.users)
                {
                    if (item == null) continue;
                    followUserList.Add(item);
                }
                offset += eachPage;
                await Task.Delay(1000);
            }
            return followUserList;
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

            PixivUgoiraMetaDto pixivUgoiraMetaDto = await PixivHelper.GetPixivUgoiraMetaAsync(pixivWorkInfo.body.illustId);
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

    }
}
