using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Theresa3rd_Bot.Cache;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Dao;
using Theresa3rd_Bot.Model.Cache;
using Theresa3rd_Bot.Model.Mys;
using Theresa3rd_Bot.Model.PO;
using Theresa3rd_Bot.Model.Subscribe;
using Theresa3rd_Bot.Type;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Business
{
    public class MYSBusiness
    {
        private SubscribeDao subscribeDao;
        private SubscribeGroupDao subscribeGroupDao;
        private SubscribeRecordDao subscribeRecordDao;

        public MYSBusiness()
        {
            subscribeDao = new SubscribeDao();
            subscribeGroupDao = new SubscribeGroupDao();
            subscribeRecordDao = new SubscribeRecordDao();
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
            MysSectionType? mysSection = null;
            string userId = null;

            string[] paramArr = message.splitParam(BotConfig.SubscribeConfig.Mihoyo.AddCommand);
            if (paramArr == null || paramArr.Length < 2)
            {
                StepInfo stepInfo = await StepCache.CreateStepAsync(session, args);
                if (stepInfo == null) return;
                StepDetail sectionStep = new StepDetail(60, $" 请在60秒内发送数字选择你要订阅的版块：\r\n{EnumHelper.MysSectionOption()}", CheckSectionAsync);
                StepDetail uidStep = new StepDetail(60, " 请在60秒内发送要订阅用户的id", CheckUserIdAsync);
                stepInfo.AddStep(sectionStep);
                stepInfo.AddStep(uidStep);
                bool isSuccess = await stepInfo.StartStep(session, args);
                if (isSuccess == false) return;
                mysSection = (MysSectionType)Convert.ToInt32(sectionStep.Answer);
                userId = uidStep.Answer;
            }
            else
            {
                if (await CheckSectionAsync(session, args, paramArr[0]) == false) return;
                if (await CheckUserIdAsync(session, args, paramArr[1]) == false) return;
                mysSection = (MysSectionType)Convert.ToInt32(paramArr[0]);
                userId = paramArr[1];
            }

            MysResult<MysUserFullInfoDto> userInfoDto = await geMysUserFullInfoDtoAsync(userId, (int)mysSection.Value);
            if (userInfoDto == null || userInfoDto.retcode != 0)
            {
                await session.SendMessageWithAtAsync(args, new PlainMessage(" 订阅失败，目标用户不存在"));
                return;
            }

            SubscribePO dbSubscribe = subscribeDao.getSubscribe(userId, SubscribeType.米游社用户);
            if (dbSubscribe == null)
            {
                //添加订阅
                dbSubscribe = new SubscribePO();
                dbSubscribe.SubscribeCode = userId;
                dbSubscribe.SubscribeName = StringHelper.filterEmoji(userInfoDto.data.user_info.nickname);
                dbSubscribe.SubscribeDescription = userInfoDto.data.user_info.introduce;
                dbSubscribe.SubscribeType = SubscribeType.米游社用户;
                dbSubscribe.SubscribeSubType = (int)mysSection.Value;
                dbSubscribe.Isliving = false;
                dbSubscribe.CreateDate = DateTime.Now;
                dbSubscribe = subscribeDao.Insert(dbSubscribe);
            }

            if (subscribeGroupDao.getCountBySubscribe(args.Sender.Group.Id, dbSubscribe.Id) > 0)
            {
                //关联订阅
                await session.SendMessageWithAtAsync(args, new PlainMessage($" 米游社用户[{userId}]已经被订阅了~"));
                return;
            }

            SubscribeGroupPO subscribeGroup = new SubscribeGroupPO();
            subscribeGroup.GroupId = args.Sender.Group.Id;
            subscribeGroup.SubscribeId = dbSubscribe.Id;
            subscribeGroup = subscribeGroupDao.Insert(subscribeGroup);

            StringBuilder msgBuilder = new StringBuilder();
            msgBuilder.AppendLine($"米游社用户[{dbSubscribe.SubscribeName}]订阅成功!");
            msgBuilder.AppendLine($"uid：{dbSubscribe.SubscribeCode}");
            msgBuilder.AppendLine($"签名：{dbSubscribe.SubscribeDescription}");
            await session.SendMessageWithAtAsync(args, new PlainMessage(msgBuilder.ToString()));
            ConfigHelper.loadSubscribeTask();
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
                string keyWord = message.splitKeyWord(BotConfig.SubscribeConfig.Mihoyo.RmCommand);
                if (string.IsNullOrEmpty(keyWord))
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" 没有检测到用户id，请确保指令格式正确"));
                    return;
                }
                if (StringHelper.isPureNumber(keyWord) == false)
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" 用户id必须为纯数字"));
                    return;
                }
                SubscribePO dbSubscribe = subscribeDao.getSubscribe(keyWord, SubscribeType.米游社用户);
                if (dbSubscribe == null)
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" 并没有订阅这个用户哦~"));
                    return;
                }
                bool isGroupSubscribed = subscribeGroupDao.getCountBySubscribe(args.Sender.Group.Id, dbSubscribe.Id) > 0;
                if (isGroupSubscribed == false)
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" 并没有订阅这个用户哦~"));
                    return;
                }
                int successCount = subscribeGroupDao.delSubscribe(args.Sender.Group.Id, dbSubscribe.Id);
                if (successCount == 0)
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" 退订失败"));
                    return;
                }
                await session.SendMessageWithAtAsync(args, new PlainMessage(" 退订成功~"));
                ConfigHelper.loadSubscribeTask();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "取消订阅异常");
                throw;
            }
        }


        private async Task<bool> CheckSectionAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string value)
        {
            int sectionType = 0;
            if (int.TryParse(value, out sectionType) == false)
            {
                await session.SendMessageWithAtAsync(args, new PlainMessage(" 版块必须为数字"));
                return false;
            }
            if (Enum.IsDefined(typeof(MysSectionType), sectionType) == false)
            {
                await session.SendMessageWithAtAsync(args, new PlainMessage(" 版块不在范围内"));
                return false;
            }
            return true;
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
            return true;
        }


        public async Task<List<MysSubscribe>> getMysUserSubscribeAsync(SubscribeInfo subscribeInfo, int getCount = 2)
        {
            int index = 0;
            List<MysSubscribe> mysSubscribeList = new List<MysSubscribe>();
            MysResult<MysPostDataDto> mysPostInfo = await getMysUserPostDtoAsync(subscribeInfo.SubscribeCode, subscribeInfo.SubscribeSubType);
            List<MysPostListDto> postList = mysPostInfo.data.list;
            if (postList.Count == 0) return mysSubscribeList;
            foreach (var item in postList)
            {
                if (++index > getCount) break;
                int shelfLife = BotConfig.SubscribeConfig.Mihoyo.ShelfLife;
                DateTime createTime = DateTimeHelper.UnixTimeStampToDateTime(item.post.created_at);
                if (shelfLife > 0 && createTime < DateTime.Now.AddSeconds(-1 * shelfLife)) continue;

                SubscribeRecordPO subscribeRecord = new SubscribeRecordPO(subscribeInfo.SubscribeId);
                subscribeRecord.Title = item.post.subject.cutString(200);
                subscribeRecord.Content = item.post.content.cutString(500);
                subscribeRecord.CoverUrl = item.post.images.Count > 0 ? item.post.images[0] : "";
                subscribeRecord.LinkUrl = HttpUrl.getMysArticleUrl(item.post.post_id);
                subscribeRecord.DynamicCode = item.post.post_id;
                subscribeRecord.DynamicType = SubscribeDynamicType.帖子;

                SubscribeRecordPO dbSubscribe = subscribeRecordDao.checkExists(subscribeInfo.SubscribeId, item.post.post_id);
                if (dbSubscribe != null) continue;

                MysSubscribe mysSubscribe = new MysSubscribe();
                mysSubscribe.SubscribeRecord = subscribeRecordDao.Insert(subscribeRecord);
                mysSubscribe.MysUserPostDto = item;
                mysSubscribe.CreateTime = createTime;
                mysSubscribeList.Add(mysSubscribe);
                await Task.Delay(1000);
            }
            return mysSubscribeList;
        }

        public async Task<List<IChatMessage>> getSubscribeInfoAsync(MysSubscribe mysSubscribe, string template = "")
        {
            if (string.IsNullOrWhiteSpace(template)) return await getDefaultSubscribeInfoAsync(mysSubscribe);
            template = template.Replace("{UserName}", mysSubscribe.MysUserPostDto.user.nickname);
            template = template.Replace("{CreateTime}", mysSubscribe.CreateTime.ToSimpleString());
            template = template.Replace("{Title}", mysSubscribe.SubscribeRecord.Title);
            template = template.Replace("{Content}", mysSubscribe.SubscribeRecord.Content);
            template = template.Replace("{Urls}", mysSubscribe.SubscribeRecord.LinkUrl);
            List<IChatMessage> chailList = new List<IChatMessage>();
            chailList.Add(new PlainMessage(template));
            FileInfo fileInfo = string.IsNullOrEmpty(mysSubscribe.SubscribeRecord.CoverUrl) ? null : await HttpHelper.DownImgAsync(mysSubscribe.SubscribeRecord.CoverUrl);
            if (fileInfo != null) chailList.Add((IChatMessage)await MiraiHelper.Session.UploadPictureAsync(UploadTarget.Group, fileInfo.FullName));
            return chailList;
        }

        public async Task<List<IChatMessage>> getDefaultSubscribeInfoAsync(MysSubscribe mysSubscribe)
        {
            List<IChatMessage> chailList = new List<IChatMessage>();
            chailList.AddRange(await getDefaultPostInfoAsync(mysSubscribe));
            return chailList;
        }


        public async Task<List<IChatMessage>> getDefaultPostInfoAsync(MysSubscribe mysSubscribe)
        {
            List<IChatMessage> chailList = new List<IChatMessage>();
            chailList.Add(new PlainMessage($"{mysSubscribe.SubscribeRecord.Title}\r\n"));
            chailList.Add(new PlainMessage($"{mysSubscribe.SubscribeRecord.Content}\r\n"));
            FileInfo fileInfo = string.IsNullOrEmpty(mysSubscribe.SubscribeRecord.CoverUrl) ? null : await HttpHelper.DownImgAsync(mysSubscribe.SubscribeRecord.CoverUrl);
            if (fileInfo != null) chailList.Add((IChatMessage)await MiraiHelper.Session.UploadPictureAsync(UploadTarget.Group, fileInfo.FullName));
            chailList.Add(new PlainMessage($"{mysSubscribe.SubscribeRecord.LinkUrl}"));
            return chailList;
        }

        private async Task<MysResult<MysPostDataDto>> getMysUserPostDtoAsync(string userId, int gids)
        {
            Dictionary<string, string> headerDic = new Dictionary<string, string>();
            string getUrl = HttpUrl.getMysPostListUrl(userId, gids);
            string json = await HttpHelper.HttpGetAsync(getUrl, headerDic);
            return JsonConvert.DeserializeObject<MysResult<MysPostDataDto>>(json);
        }

        private async Task<MysResult<MysUserFullInfoDto>> geMysUserFullInfoDtoAsync(string userId, int gids)
        {
            Dictionary<string, string> headerDic = new Dictionary<string, string>();
            string getUrl = HttpUrl.getMystUserFullInfo(userId, gids);
            string json = await HttpHelper.HttpGetAsync(getUrl, headerDic);
            return JsonConvert.DeserializeObject<MysResult<MysUserFullInfoDto>>(json);
        }


    }
}
