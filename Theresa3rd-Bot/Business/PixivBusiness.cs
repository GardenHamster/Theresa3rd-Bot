using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using System.Collections.Generic;
using System.Threading.Tasks;
using Theresa3rd_Bot.Dao;
using Theresa3rd_Bot.Model.Pixiv;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Business
{
    public class PixivBusiness
    {
        /*
        private SubscribeRecordDao subscribeRecordDao;

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

        public async Task setPixivCookieAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string message)
        {
            string cookie = message.splitKeyWord("pixivcookie");
            if (string.IsNullOrWhiteSpace(cookie))
            {
                await session.SendGroupMessageAsync(args.Sender.Group.Id, new AtMessage(args.Sender.Id, ""), new PlainMessage("Hello World!"));
                e.FromQQ.SendPrivateMessage("未检测到cookie,请使用pixivcookie + cookie形式发送");
                return;
            }
            string baseCookie = splitBaseCookie(cookie);//分离出固定不变的cookie部分
            Website website = new WebsiteBusiness().updateWebsite(WebsiteType.Pixiv, baseCookie, 2 * 7 * 24 * 60);
            SettingHelper.loadWebsiteAndCookie();
            e.SendMessageWithAt(string.Format("cookie更新完毕,过期时间为{0}", website.CookieExpireDate.ToString("yyyy-MM-dd HH:mm:ss")));
            return;
        }

        public void setBilibiliCookie(CQGroupMessageEventArgs e)
        {
            string cookie = StringHelper.splitKeyWord(e.Message.Text.Trim(), "bilicookie");
            if (string.IsNullOrEmpty(cookie))
            {
                e.FromQQ.SendPrivateMessage("未检测到cookie,请使用pixivcookie + cookie形式发送");
                return;
            }
            string baseCookie = splitBaseCookie(cookie);//分离出固定不变的cookie部分
            Website website = new WebsiteBusiness().updateWebsite(WebsiteType.Bilibili, baseCookie, 30 * 24 * 60);
            SettingHelper.loadWebsiteAndCookie();
            e.SendMessageWithAt(string.Format("cookie更新完毕,过期时间为{0}", website.CookieExpireDate.ToString("yyyy-MM-dd HH:mm:ss")));
            return;
        }

        public string splitBaseCookie(string cookie)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            Dictionary<string, string> cookieDic = StringHelper.splitCookie(cookie);
            foreach (KeyValuePair<string, string> item in cookieDic)
            {
                if (item.Key == "ki_t") continue;
                if (item.Key == "__utmb") continue;
                if (sqlBuilder.Length > 0) sqlBuilder.Append("; ");
                sqlBuilder.Append(item.Key + "=" + item.Value);
            }
            return sqlBuilder.ToString();
        }

        public string getForgedCookie()
        {
            DateTime nowDateTime = DateTime.Now;
            Random random = new Random();
            long nowTimeStamp = DateTimeHelper.dateTimeToTimeStamp(nowDateTime);
            long nowLongTimeStamp = DateTimeHelper.dateTimeToLongTimeStamp(nowDateTime);
            if (Setting.Pixiv.ExpireLongTimeStamp < nowLongTimeStamp) Setting.Pixiv.ExpireLongTimeStamp = nowLongTimeStamp + 20 * 60 * 1000;
            long changeTimeStamp = (Setting.Pixiv.ExpireLongTimeStamp / 1000) + 12 * 60 * 60;
            int randomIndex = random.Next(1, 3);
            int sendCount1 = random.Next(1, 30);
            int sendCount2 = random.Next(1, 30);
            string ki_t = string.Format("ki_t={0}%3B{0}%3B{1}%3B{2}%3B{3}", Setting.Pixiv.ExpireLongTimeStamp, nowLongTimeStamp, randomIndex, sendCount1);
            string __utmb = string.Format("__utmb=235335808.{0}.10.{1}", sendCount2, changeTimeStamp);
            string cookie = Setting.Pixiv.Cookie + "; " + ki_t + "; " + __utmb;
            ////CQHelper.CQLog.Debug(cookie);
            return cookie;
        }

        public void sendGeneralPixivImage(CQGroupMessageEventArgs e)
        {
            try
            {
                string message = e.Message.Text.Trim();
                DateTime startDateTime = DateTime.Now;
                CoolingCache.setHanding(e.FromGroup.Id, e.FromQQ.Id);
                if (BusinessHelper.isPixivCookieExpire(e)) return;
                string[] splitArr = message.Split(new string[] { "涩图" }, StringSplitOptions.RemoveEmptyEntries);
                if (splitArr.Length > 1 && BusinessHelper.checkSTBanWord(e))
                {
                    e.SendMessageWithAt(" 禁止查找这个类型的涩图哦，换个标签试试吧~");
                    return;
                }

                PixivWorkInfoDto pixivWorkInfoDto = null;
                string tagName = splitArr.Length > 1 ? splitArr[1].Trim() : "";
                tagName = tagName.Replace("（", ")").Replace("）", ")");

                if (StringHelper.isPureNumber(tagName))
                {
                    pixivWorkInfoDto = getPixivWorkInfoDto(tagName);//根据作品id获取作品
                }
                else if (string.IsNullOrEmpty(tagName))
                {
                    pixivWorkInfoDto = getRandomWorkInFollow(e);//获取随机一个关注的画师的作品
                }
                else
                {
                    if (BusinessHelper.isGroupAllowCustomST(e) == false) return;
                    pixivWorkInfoDto = getRandomWork(e, tagName, false);//获取随机一个作品
                }

                if (pixivWorkInfoDto == null)
                {
                    e.SendMessageWithAt(CQApi.CQCode_Image("face/face06.gif").ToSendString() + "找不到这类型的图片或者图片收藏比过低，换个标签试试吧~");
                    return;
                }

                int todayLeftCount = BusinessHelper.getSTLeftUseToday(e, 1);
                FileInfo fileInfo = downImg(pixivWorkInfoDto);
                string atStr = CQApi.CQCode_At(e.FromQQ.Id).ToSendString();
                PixivWorkInfo pixivWorkInfo = pixivWorkInfoDto.body;
                string warnMsg = string.Format("{0} {1}秒后再来哦，今天剩余使用次数{2}次，本消息将在{3}秒后撤回，尽快保存哦\r\n", atStr, Setting.Robot.GetSTInterval, todayLeftCount, Setting.Robot.RemovePixivSTInterval);
                string workInfoStr = getWorkInfoStr(pixivWorkInfo, fileInfo, DateTimeHelper.GetSecondDiff(startDateTime, DateTime.Now));
                string imgStr = fileInfo == null ? FaceHelper.faceImgFail() : CQApi.CQCode_Image(FilePath.getRelativeDownImgPath(fileInfo.Name)).ToSendString();
                if (pixivWorkInfoDto.body.isR18()) imgStr = "";
                string sendMessage = warnMsg + workInfoStr + imgStr;
                QQMessage qqMessage = e.CQApi.SendGroupMessage(e.FromGroup.Id, new Object[] { sendMessage.ToString() });
                Thread.Sleep(2000);//防止请求过快被检测
                e.FromQQ.SendPrivateMessage(workInfoStr + imgStr);
                CoolingCache.setMemberSTCooling(e.FromGroup.Id, e.FromQQ.Id);
                new FunctionRecordBusiness().addRecord(e.FromGroup.Id, e.FromQQ.Id, FunctionType.PixivGeneralST.TypeId, message);
                if (Setting.Permissions.NoRemoveSTGroups.Contains(e.FromGroup.Id)) return;
                //Thread.Sleep(Setting.Robot.RemovePixivSTInterval * 1000);
                //e.FromGroup.CQApi.RemoveMessage(qqMessage);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
                e.CQLog.Error("涩图功能错误", ex.Message, ex.StackTrace);
                BusinessHelper.sendErrorMessage(e.CQApi, ex, "涩图功能错误", false);
                e.SendMessageWithAt(CQApi.CQCode_Image("face/face06.gif").ToSendString() + "获取图片出错了，再试一次吧~");
            }
            finally
            {
                CoolingCache.setHandFinish(e.FromGroup.Id, e.FromQQ.Id);
            }
        }

        public void sendR18PixivImage(CQGroupMessageEventArgs e)
        {
            try
            {
                string message = e.Message.Text.Trim();
                DateTime startDateTime = DateTime.Now;
                CoolingCache.setHanding(e.FromGroup.Id, e.FromQQ.Id);
                PixivicTagModel tagModel = RandomTag.getRandomTag();//获取随机标签随机页
                string[] splitArr = message.Split(new string[] { "色图" }, StringSplitOptions.RemoveEmptyEntries);

                PixivWorkInfoDto pixivWorkInfoDto = null;
                string tagName = splitArr.Length > 1 ? splitArr[1].Trim() : "";
                tagName = tagName.Replace("（", ")").Replace("）", ")");

                if (StringHelper.isPureNumber(tagName))//作品id
                {
                    pixivWorkInfoDto = getPixivWorkInfoDto(tagName);
                }
                else
                {
                    if (string.IsNullOrEmpty(tagName) == false) tagModel = new PixivicTagModel(tagName, 0, true);
                    pixivWorkInfoDto = getRandomWork(e, tagName, true);//获取随机一个作品
                }

                if (pixivWorkInfoDto == null)
                {
                    e.SendMessageWithAt(CQApi.CQCode_Image("face/face06.gif").ToSendString() + "找不到这类型的图片或者图片收藏比过低，换个标签试试吧~");
                    return;
                }

                int todayLeftCount = BusinessHelper.getSTLeftUseToday(e, 1);
                FileInfo fileInfo = downImg(pixivWorkInfoDto);
                string atStr = CQApi.CQCode_At(e.FromQQ.Id).ToSendString();
                PixivWorkInfo pixivWorkInfo = pixivWorkInfoDto.body;
                string warnMsg = string.Format("{0} {1}秒后再来哦，今天剩余使用次数{2}次，图片在下面的链接中，本消息将在{3}秒后撤回，尽快保存哦\r\n", atStr, Setting.Robot.GetSTInterval, todayLeftCount, Setting.Robot.RemovePixivSTInterval);
                string workInfoStr = getWorkInfoStr(pixivWorkInfo, fileInfo, DateTimeHelper.GetSecondDiff(startDateTime, DateTime.Now));
                string imgStr = fileInfo == null ? FaceHelper.faceImgFail() : CQApi.CQCode_Image(FilePath.getRelativeDownImgPath(fileInfo.Name)).ToSendString();
                QQMessage qqMessage = e.CQApi.SendGroupMessage(e.FromGroup.Id, new Object[] { warnMsg + workInfoStr });
                if (qqMessage.IsSuccess == false) e.SendMessageWithAt(CQApi.CQCode_Image("face/face06.gif").ToSendString() + "图片发送失败了~");
                Thread.Sleep(2000);//防止请求过快被检测
                e.FromQQ.SendPrivateMessage(new Object[] { warnMsg + workInfoStr + imgStr });
                CoolingCache.setMemberSTCooling(e.FromGroup.Id, e.FromQQ.Id);
                new FunctionRecordBusiness().addRecord(e.FromGroup.Id, e.FromQQ.Id, FunctionType.PixivR18ST.TypeId, message);
                if (Setting.Permissions.NoRemoveSTGroups.Contains(e.FromGroup.Id)) return;
                //Thread.Sleep(Setting.Robot.RemoveR18STInterval * 1000);
                //e.FromGroup.CQApi.RemoveMessage(qqMessage);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
                e.CQLog.Error("r18涩图功能错误", ex.Message, ex.StackTrace);
                e.SendMessageWithAt(CQApi.CQCode_Image("face/face06.gif").ToSendString() + "获取图片出错了，再试一次吧~");
            }
            finally
            {
                CoolingCache.setHandFinish(e.FromGroup.Id, e.FromQQ.Id);
            }
        }



        public string getPixivNewestWork(CQGroupMessageEventArgs e, string userId, int subscribeId)
        {
            try
            {
                List<PixivSubscribe> pixivSubscribeList = getPixivUserNewestWork(userId, subscribeId, Setting.Subscribe.PixivEachRead);
                if (pixivSubscribeList == null || pixivSubscribeList.Count == 0) return "该画师还没有任何作品~";
                PixivSubscribe pixivSubscribe = pixivSubscribeList.First();
                if (pixivSubscribe.PixivWorkInfoDto.body.isR18() && Setting.Permissions.R18Groups.Contains(e.FromGroup.Id) == false) return "该作品为R-18作品，不显示相关内容";
                string imgStr = pixivSubscribe.WorkFileInfo == null ? FaceHelper.faceImgFail() : CQApi.CQCode_Image(FilePath.getRelativeDownImgPath(pixivSubscribe.WorkFileInfo.Name)).ToSendString();
                if (pixivSubscribe.PixivWorkInfoDto.body.isR18()) imgStr = "";
                return string.Format("pixiv画师[{0}]的最新作品：\r\n{1}{2}", pixivSubscribe.PixivWorkInfoDto.body.userName, pixivSubscribe.WorkInfo, imgStr);
            }
            catch (Exception ex)
            {
                e.CQLog.Error("发送画师最新作品时出现异常", ex.Message, ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// 随机获取一个关注的画师的作品
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        protected PixivWorkInfoDto getRandomWorkInFollow(CQGroupMessageEventArgs e)
        {
            int loopUserTimes = 3;
            int loopWorkTimes = 5;
            TypeModel subscribeType = SubscribeSourceType.PixivUser;
            if (Setting.Subscribe.SubscribeTaskMap.ContainsKey(subscribeType.TypeId) == false) return null;
            List<SubscribeTask> subscribeTaskList = Setting.Subscribe.SubscribeTaskMap[subscribeType.TypeId];
            if (subscribeTaskList == null || subscribeTaskList.Count == 0) return null;
            for (int i = 0; i < loopUserTimes; i++)
            {
                int randomUserIndex = RandomHelper.getRandomBetween(0, subscribeTaskList.Count - 1);
                SubscribeTask subscribeTask = subscribeTaskList[randomUserIndex];
                PixivUserWorkInfoDto pixivWorkInfo = getPixivUserWorkInfoDto(subscribeTask.SubscribeCode);
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
                    bool isPopularity = pixivWorkInfoDto.body.likeCount >= 400 && pixivWorkInfoDto.body.bookmarkCount >= 600;
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
        protected PixivWorkInfoDto getRandomWork(CQGroupMessageEventArgs e, string tagName, bool isR18)
        {
            int pageCount = 3;
            PixivSearchDto pageOne = getPixivSearchDto(tagName, 1, false, isR18);
            int total = pageOne.body.getIllust().total;
            int maxPage = (int)Math.Ceiling(Convert.ToDecimal(total) / Setting.Pixiv.PageSize);
            maxPage = maxPage > 1000 ? 1000 : maxPage;
            Thread.Sleep(1000);//防止请求过快被检测
            int[] pageArr = getRandomPageNo(maxPage, pageCount);
            List<PixivIllust> tempIllustList = new List<PixivIllust>();
            foreach (int page in pageArr)
            {
                PixivSearchDto pixivSearchDto = getPixivSearchDto(tagName, page, false, isR18);
                foreach (PixivIllust pixivIllust in pixivSearchDto.body.getIllust().data)
                {
                    if (pixivIllust != null) tempIllustList.Add(pixivIllust);
                }
                Thread.Sleep(1000);//防止请求过快被检测
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

            bool isScreen = total > workScreenMoreThen;//是否优先筛选较高收藏
            Task[] tasks = new Task[threadCount];
            for (int i = 0; i < taskList.Length; i++)
            {
                List<PixivIllust> illustList = taskList[i];
                tasks[i] = Task.Factory.StartNew(() => getPixivWorkInfoMethod(e, illustList, isScreen, isR18));
                Thread.Sleep(RandomHelper.getRandomBetween(500, 1000));//将每条线程的间隔错开
            }
            Task.WaitAll(tasks);

            PixivWorkInfoDto randomWork = bookUp2000List.FirstOrDefault();
            if (randomWork == null) randomWork = bookUp1500List.FirstOrDefault();
            if (randomWork == null) randomWork = bookUp1000List.FirstOrDefault();
            if (randomWork == null) randomWork = bookUp800List.FirstOrDefault();
            if (randomWork == null) return null;
            randomWork.body.RelevantCount = total;
            return randomWork;
        }

        protected void getPixivWorkInfoMethod(CQGroupMessageEventArgs e, List<PixivIllust> pixivIllustList, bool isScreen, bool isR18)
        {
            for (int i = 0; i < pixivIllustList.Count; i++)
            {
                try
                {
                    if (isScreen == true && bookUp2000List.Count > 0) return;//如果启用筛选,获取到一个2000+就结束全部线程
                    if (isScreen == true && bookUp800List.Count > 0 && bookUpList.Count >= 30) return;
                    if (isScreen == false && bookUp800List.Count > 0) return;//如果不启用筛选,获取到一个800+就结束全部线程
                    PixivWorkInfoDto pixivWorkInfo = getPixivWorkInfoDto(pixivIllustList[i].id);
                    if (checkRandomWorkIsOk(pixivWorkInfo, isR18) && pixivWorkInfo.error == false)
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
                    e.CQLog.Error("-1", "获取作品信息时出现异常:" + ex.Message, ex.StackTrace);
                }
                finally
                {
                    Thread.Sleep(500);//防止请求过快被检测
                }
            }
        }

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

        public List<PixivSubscribe> getPixivUserNewestWork(string userId, int subscribeId, int getCount = 2)
        {
            int index = 0;
            DateTime startDateTime = DateTime.Now;
            List<PixivSubscribe> pixivSubscribeList = new List<PixivSubscribe>();
            PixivUserWorkInfoDto pixivWorkInfo = getPixivUserWorkInfoDto(userId);
            if (pixivWorkInfo == null) return pixivSubscribeList;
            Dictionary<string, PixivUserWorkInfo> illusts = pixivWorkInfo.body.illusts;
            if (illusts == null || illusts.Count == 0) return pixivSubscribeList;
            foreach (KeyValuePair<string, PixivUserWorkInfo> workInfo in illusts)
            {
                if (++index > getCount) break;
                if (workInfo.Value == null) continue;
                SubscribeRecord dbSubscribe = subscribeRecordDao.checkExists(subscribeId, workInfo.Value.id);
                if (dbSubscribe != null) continue;
                PixivWorkInfoDto pixivWorkInfoDto = getPixivWorkInfoDto(workInfo.Value.id);
                if (pixivWorkInfoDto == null) continue;
                FileInfo fileInfo = downImg(pixivWorkInfoDto);
                SubscribeRecord subscribeRecord = new SubscribeRecord(subscribeId);
                subscribeRecord.Title = StringHelper.filterEmoji(pixivWorkInfoDto.body.illustTitle);
                subscribeRecord.Content = subscribeRecord.Title;
                subscribeRecord.CoverUrl = HttpUrl.getPixivWorkInfoUrl(workInfo.Value.id);
                subscribeRecord.LinkUrl = HttpUrl.getPixivWorkInfoUrl(workInfo.Value.id);
                subscribeRecord.ArticleId = pixivWorkInfoDto.body.illustId;
                subscribeRecord.ArticleType = SubscribeArticleType.Illustration.TypeId;
                subscribeRecord = subscribeRecordDao.Insert(subscribeRecord);
                PixivSubscribe pixivSubscribe = new PixivSubscribe();
                pixivSubscribe.SubscribeRecord = subscribeRecord;
                pixivSubscribe.PixivWorkInfoDto = pixivWorkInfoDto;
                pixivSubscribe.WorkFileInfo = fileInfo;
                pixivSubscribe.WorkInfo = getWorkInfoStr(pixivWorkInfoDto.body, fileInfo, DateTimeHelper.GetSecondDiff(startDateTime, DateTime.Now));
                pixivSubscribeList.Add(pixivSubscribe);
            }
            return pixivSubscribeList;
        }

        public List<PixivSubscribe> getPixivTagNewestWork(string tagName, int subscribeId)
        {
            DateTime startDateTime = DateTime.Now;
            PixivSearchDto pageOne = getPixivSearchDto(tagName, 1, true, false);
            List<PixivSubscribe> pixivSubscribeList = new List<PixivSubscribe>();
            if (pageOne == null) return pixivSubscribeList;
            foreach (PixivIllust item in pageOne.body.getIllust().data)
            {
                if (item.createDate < DateTime.Now.AddDays(-1)) break;
                PixivWorkInfoDto pixivWorkInfoDto = getPixivWorkInfoDto(item.id);
                if (pixivWorkInfoDto == null) continue;
                PixivWorkInfo pixivWorkInfo = pixivWorkInfoDto.body;
                if (pixivWorkInfo.isR18()) continue;
                if (checkNewWorkIsOk(pixivWorkInfoDto) == false) continue;
                SubscribeRecord dbSubscribe = subscribeRecordDao.checkExists(subscribeId, pixivWorkInfo.illustId);
                if (dbSubscribe != null) continue;
                FileInfo fileInfo = downImg(pixivWorkInfoDto);
                SubscribeRecord subscribeRecord = new SubscribeRecord(subscribeId);
                subscribeRecord.Title = StringHelper.filterEmoji(pixivWorkInfoDto.body.illustTitle);
                subscribeRecord.Content = subscribeRecord.Title;
                subscribeRecord.CoverUrl = HttpUrl.getPixivWorkInfoUrl(pixivWorkInfo.illustId);
                subscribeRecord.LinkUrl = HttpUrl.getPixivWorkInfoUrl(pixivWorkInfo.illustId);
                subscribeRecord.ArticleId = pixivWorkInfoDto.body.illustId;
                subscribeRecord.ArticleType = SubscribeArticleType.Illustration.TypeId;
                subscribeRecord = subscribeRecordDao.Insert(subscribeRecord);
                PixivSubscribe pixivSubscribe = new PixivSubscribe();
                pixivSubscribe.SubscribeRecord = subscribeRecord;
                pixivSubscribe.PixivWorkInfoDto = pixivWorkInfoDto;
                pixivSubscribe.WorkFileInfo = fileInfo;
                pixivSubscribe.WorkInfo = getWorkInfoStr(pixivWorkInfoDto.body, fileInfo, DateTimeHelper.GetSecondDiff(startDateTime, DateTime.Now));
                pixivSubscribeList.Add(pixivSubscribe);
            }
            return pixivSubscribeList;
        }


        /// <summary>
        /// 判断插画质量是否符合
        /// </summary>
        /// <param name="pixivWorkInfo"></param>
        /// <param name="isR18"></param>
        /// <returns></returns>
        protected bool checkRandomWorkIsOk(PixivWorkInfoDto pixivWorkInfo, bool isR18)
        {
            if (pixivWorkInfo == null) return false;
            if (pixivWorkInfo.body == null) return false;
            bool isNotBantag = pixivWorkInfo.body.hasBanTag() == false;
            bool isAllowR18 = isR18 || pixivWorkInfo.body.isR18() == false;
            bool isPopularity = pixivWorkInfo.body.likeCount >= 500 && pixivWorkInfo.body.bookmarkCount >= 800;
            bool isLikeProportional = Convert.ToDouble(pixivWorkInfo.body.likeCount) / pixivWorkInfo.body.viewCount >= 0.04;
            bool isBookProportional = Convert.ToDouble(pixivWorkInfo.body.bookmarkCount) / pixivWorkInfo.body.viewCount >= 0.05;
            return isPopularity && isAllowR18 && isBookProportional && isLikeProportional && isNotBantag;
        }

        /// <summary>
        /// 判断插画质量是否符合
        /// </summary>
        /// <param name="pixivWorkInfo"></param>
        /// <param name="isR18"></param>
        /// <returns></returns>
        protected bool checkNewWorkIsOk(PixivWorkInfoDto pixivWorkInfo)
        {
            if (pixivWorkInfo == null) return false;
            if (pixivWorkInfo.body == null) return false;
            bool isNotR18 = pixivWorkInfo.body.isR18() == false;
            bool isPopularity = pixivWorkInfo.body.likeCount >= 100 && pixivWorkInfo.body.bookmarkCount >= 150;
            TimeSpan timeSpan = DateTime.Now.Subtract(pixivWorkInfo.body.createDate);
            bool isLikeProportional = pixivWorkInfo.body.likeCount > (timeSpan.Hours + 1) * 40;
            bool isBookProportional = pixivWorkInfo.body.bookmarkCount > (timeSpan.Hours + 1) * 50;
            return isPopularity && isNotR18 && isBookProportional && isLikeProportional;
        }

        protected FileInfo downImg(PixivWorkInfoDto pixivWorkInfo)
        {
            try
            {
                CQHelper.CQLog.Info("开始下载图片" + pixivWorkInfo.body.urls.original);
                if (pixivWorkInfo.body.isGif()) return downAndComposeGif(pixivWorkInfo);
                string fullFileName = pixivWorkInfo.body.illustId + ".jpg";
                string imgReferer = HttpUrl.getPixivArtworksReferer(pixivWorkInfo.body.illustId);
                string imgUrl = pixivWorkInfo.body.urls.original;
                //string imgUrl = pixivWorkInfo.body.urls.original.Replace("i.pximg.net", "i.pixiv.cat");
                string fullImageSavePath = FilePath.getDownImgSavePath() + fullFileName;
                return HttpHelper.downImg(imgUrl, fullImageSavePath, imgReferer, getForgedCookie());
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, "PixivBusiness.downImg下载图片失败");
                CQHelper.CQLog.Error("PixivBusiness.downImg下载图片失败", ex.Message, ex.StackTrace);
                return null;
            }
        }

        protected FileInfo downAndComposeGif(PixivWorkInfoDto pixivWorkInfo)
        {
            Dictionary<string, string> headerDic = new Dictionary<string, string>();
            headerDic.Add("Referer", HttpUrl.getPixivUgoiraMetaReferer(pixivWorkInfo.body.illustId));
            //CQHelper.CQLog.Debug("illustId=" + pixivWorkInfo.body.illustId);
            PixivUgoiraMetaDto pixivUgoiraMetaDto = getPixivUgoiraMetaDto(pixivWorkInfo.body.illustId);
            string fullZipSavePath = FilePath.getDownImgSavePath() + StringHelper.get16UUID() + ".zip";
            //CQHelper.CQLog.Debug("fullZipSavePath=" + fullZipSavePath);
            //CQHelper.CQLog.Debug("src=" + pixivUgoiraMetaDto.body.src);
            HttpHelper.HttpDownload(pixivUgoiraMetaDto.body.src, fullZipSavePath, headerDic);
            string unZipDirPath = FilePath.getDownImgSavePath() + pixivWorkInfo.body.illustId;
            ZipHelper.ZipToFile(fullZipSavePath, unZipDirPath);
            DirectoryInfo directoryInfo = new DirectoryInfo(unZipDirPath);
            FileInfo[] files = directoryInfo.GetFiles();
            List<PixivUgoiraMetaFrames> frames = pixivUgoiraMetaDto.body.frames;
            string fullGifSavePath = FilePath.getDownImgSavePath() + pixivWorkInfo.body.illustId + ".gif";
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
            string tomcatGifSavePath = FilePath.getGifImgPath();
            if (Directory.Exists(tomcatGifSavePath) == false) Directory.CreateDirectory(tomcatGifSavePath);
            string fullTomcatGifSavePath = tomcatGifSavePath + pixivWorkInfo.body.illustId + ".gif";
            if (File.Exists(fullTomcatGifSavePath)) File.Delete(fullTomcatGifSavePath);
            File.Copy(fullGifSavePath, fullTomcatGifSavePath);
            return new FileInfo(fullGifSavePath);
        }

        public string getWorkInfoStr(PixivWorkInfo pixivWorkInfo, FileInfo fileInfo, int costSecond)
        {
            StringBuilder workInfoStr = new StringBuilder();
            double mb = fileInfo == null ? 0 : MathHelper.getMbWithByte(fileInfo.Length);
            workInfoStr.Append(string.Format("标题：{0}，画师：{1}，画师id：{2}，大小：{3}MB，",
                pixivWorkInfo.illustTitle, pixivWorkInfo.userName, pixivWorkInfo.userId, mb));
            workInfoStr.Append(string.Format("收藏：{0}，赞：{1}，浏览：{2}，",
               pixivWorkInfo.bookmarkCount, pixivWorkInfo.likeCount, pixivWorkInfo.viewCount));
            //workInfoStr.Append(string.Format("收藏比：{0}，赞比：{1}，",
            //   MathHelper.getRateStr(pixivWorkInfo.bookmarkCount, pixivWorkInfo.viewCount, 2),
            //   MathHelper.getRateStr(pixivWorkInfo.likeCount, pixivWorkInfo.viewCount, 2)));
            workInfoStr.Append(string.Format("耗时：{0}s", costSecond, pixivWorkInfo.pageCount));
            if (pixivWorkInfo.RelevantCount > 0) workInfoStr.Append(string.Format("，相关图片：{0}张", pixivWorkInfo.RelevantCount));
            workInfoStr.AppendLine();
            //workInfoStr.AppendLine(string.Format("标签：{0}", getTagsStr(pixivWorkInfo.tags)));
            workInfoStr.Append(getWorkUrlStr(pixivWorkInfo));
            return workInfoStr.ToString();
        }

        protected string getWorkUrlStr(PixivWorkInfo pixivWorkInfo)
        {
            int maxShowCount = 3;
            StringBuilder LinkStr = new StringBuilder();
            int imgCount = pixivWorkInfo.pageCount;
            int endCount = imgCount > maxShowCount ? maxShowCount : imgCount;
            string LineInfo = string.Format("该作品列表中包含{0}张图片，前{1}张图片链接如下:", imgCount, endCount);
            if (pixivWorkInfo.isGif()) LinkStr.Append("\r\n" + HttpUrl.getTomcatGifUrl(pixivWorkInfo.illustId));
            for (int i = 0; i < endCount; i++)
            {
                string imgUrl = pixivWorkInfo.urls.original.Replace("https:", "http:").Replace("i.pximg.net", "pixiv2.theresa3rd.cn");
                if (i > 0) imgUrl = imgUrl.Replace("_p0.", string.Format("_p{0}.", i));
                LinkStr.Append("\r\n" + imgUrl);
            }
            return LineInfo + LinkStr.ToString();
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

        public PixivSearchDto getPixivSearchDto(string keyword, int pageNo, bool isMatchAll, bool isR18)
        {
            string referer = HttpUrl.getPixivSearchReferer(keyword);
            Dictionary<string, string> headerDic = getPixivHeader(referer);
            string postUrl = HttpUrl.getPixivSearchUrl(keyword, pageNo, isMatchAll, isR18);
            //CQHelper.CQLog.Debug("getPixivSearchDto-postUrl", postUrl);
            string json = HttpHelper.HttpGet(postUrl, headerDic, 10 * 1000);
            //CQHelper.CQLog.Debug("getPixivSearchDto-json", json);
            return JsonConvert.DeserializeObject<PixivSearchDto>(json);
        }

        public PixivWorkInfoDto getPixivWorkInfoDto(string wordId)
        {
            string referer = HttpUrl.getPixivWorkInfoReferer(wordId);
            Dictionary<string, string> headerDic = getPixivHeader(referer);
            string postUrl = HttpUrl.getPixivWorkInfoUrl(wordId);
            //CQHelper.CQLog.Debug("getPixivWorkInfoDto-postUrl", postUrl);
            string json = HttpHelper.HttpGet(postUrl, headerDic, 10 * 1000);
            //CQHelper.CQLog.Debug("getPixivWorkInfoDto-json", json);
            return JsonConvert.DeserializeObject<PixivWorkInfoDto>(json);
        }

        public PixivUserWorkInfoDto getPixivUserWorkInfoDto(string userId)
        {
            string referer = HttpUrl.getPixivUserWorkInfoReferer(userId);
            Dictionary<string, string> headerDic = getPixivHeader(referer);
            string postUrl = HttpUrl.getPixivUserWorkInfoUrl(userId);
            //CQHelper.CQLog.Debug("getPixivUserWorkInfoDto-postUrl", postUrl);
            string json = HttpHelper.HttpGet(postUrl, headerDic, 10 * 1000);
            //CQHelper.CQLog.Debug("getPixivUserWorkInfoDto-json", json);
            if (string.IsNullOrEmpty(json) == false && json.Contains("\"illusts\":[]"))
            {
                throw new Exception($"pixiv用户{userId}作品列表illusts为空,cookie可能已经过期");
            }
            return JsonConvert.DeserializeObject<PixivUserWorkInfoDto>(json);
        }

        public PixivUserInfoDto getPixivUserInfoDto(string userId)
        {
            string referer = HttpUrl.getPixivUserWorkInfoReferer(userId);
            Dictionary<string, string> headerDic = getPixivHeader(referer);
            string postUrl = HttpUrl.getPixivUserWorkInfoUrl(userId);
            //CQHelper.CQLog.Debug("getPixivUserInfoDto-postUrl", postUrl);
            string json = HttpHelper.HttpGet(postUrl, headerDic, 10 * 1000);
            //CQHelper.CQLog.Debug("getPixivUserInfoDto-json", json);
            return JsonConvert.DeserializeObject<PixivUserInfoDto>(json);
        }

        public PixivUgoiraMetaDto getPixivUgoiraMetaDto(string wordId)
        {
            string referer = HttpUrl.getPixivUgoiraMetaReferer(wordId);
            Dictionary<string, string> headerDic = getPixivHeader(referer);
            string postUrl = HttpUrl.getPixivUgoiraMetaUrl(wordId);
            //CQHelper.CQLog.Debug("getPixivUgoiraMetaDto-postUrl", postUrl);
            string json = HttpHelper.HttpGet(postUrl, headerDic, 10 * 1000);
            //CQHelper.CQLog.Debug("getPixivUgoiraMetaDto-json", json);
            return JsonConvert.DeserializeObject<PixivUgoiraMetaDto>(json);
        }


        public Dictionary<string, string> getPixivHeader(string referer)
        {
            Dictionary<string, string> headerDic = new Dictionary<string, string>();
            headerDic.Add("cookie", getForgedCookie());
            headerDic.Add("referer", referer);
            //headerDic.Add("accept-encoding", "gzip, deflate, br");
            //headerDic.Add("accept-language", "zh-CN,zh;q=0.9");
            headerDic.Add("accept", "application/json");
            headerDic.Add("sec-fetch-mode", "cors");
            headerDic.Add("sec-fetch-site", "same-origin");
            headerDic.Add("x-user-id", Setting.Pixiv.XUserId);
            return headerDic;
        }
        */

    }
}
