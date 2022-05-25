using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            try
            {
                MysSectionType? mysSection = null;
                string userId = null;

                string[] paramArr = message.splitParam(BotConfig.SubscribeConfig.Mihoyo.AddCommand);
                if (paramArr == null || paramArr.Length == 0)
                {
                    StepInfo stepInfo = await StepCache.CreateStepAsync(session, args);
                    if (stepInfo == null) return;
                    StepDetail uidStep = new StepDetail(60, " 请在60秒内发送要订阅用户的id", CheckUserIdAsync);
                    StepDetail sectionStep = new StepDetail(60, $" 请在60秒内发送数字选择你要订阅的频道：\r\n{EnumHelper.MysSectionOption()}", CheckSectionAsync);
                    stepInfo.AddStep(uidStep);
                    stepInfo.AddStep(sectionStep);
                    bool isSuccess = await stepInfo.StartStep(session, args);
                    if (isSuccess == false) return;
                    userId = uidStep.Answer;
                    mysSection = (MysSectionType)Convert.ToInt32(sectionStep.Answer);
                }
                else
                {
                    userId = paramArr.Length > 0 ? paramArr[0] : null;
                    string mysSectionStr = paramArr.Length > 1 ? paramArr[1] : "0";
                    if (await CheckUserIdAsync(session, args, userId) == false) return;
                    if (await CheckSectionAsync(session, args, mysSectionStr) == false) return;
                    mysSection = (MysSectionType)Convert.ToInt32(mysSectionStr);
                }

                MysResult<MysUserFullInfoDto> userInfoDto = await geMysUserFullInfoDtoAsync(userId, (int)mysSection.Value);
                if (userInfoDto == null || userInfoDto.retcode != 0)
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" 订阅失败，目标用户不存在"));
                    return;
                }

                SubscribePO fullSubscribe = subscribeDao.getSubscribe(userId, SubscribeType.米游社用户, (int)MysSectionType.全部);
                if (fullSubscribe != null && subscribeGroupDao.getCountBySubscribe(args.Sender.Group.Id, fullSubscribe.Id) > 0)
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage($" 已订阅了米游社用户[{userId}]的[{Enum.GetName(typeof(MysSectionType), MysSectionType.全部)}]频道~"));
                    return;
                }

                SubscribePO dbSubscribe = subscribeDao.getSubscribe(userId, SubscribeType.米游社用户, (int)mysSection.Value);
                if (dbSubscribe == null)
                {
                    //添加订阅
                    dbSubscribe = new SubscribePO();
                    dbSubscribe.SubscribeCode = userId;
                    dbSubscribe.SubscribeName = StringHelper.filterEmoji(userInfoDto.data.user_info.nickname)?.filterEmoji().cutString(50);
                    dbSubscribe.SubscribeDescription = userInfoDto.data.user_info.introduce?.filterEmoji().cutString(200);
                    dbSubscribe.SubscribeType = SubscribeType.米游社用户;
                    dbSubscribe.SubscribeSubType = (int)mysSection.Value;
                    dbSubscribe.Isliving = false;
                    dbSubscribe.CreateDate = DateTime.Now;
                    dbSubscribe = subscribeDao.Insert(dbSubscribe);
                }

                if (subscribeGroupDao.getCountBySubscribe(args.Sender.Group.Id, dbSubscribe.Id) > 0)
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage($" 已订阅了米游社用户[{userId}]的[{Enum.GetName(typeof(MysSectionType), mysSection.Value)}]频道~"));
                    return;
                }

                if (mysSection.Value == MysSectionType.全部) delAllSubscribe(args.Sender.Group.Id, userId);

                SubscribeGroupPO subscribeGroup = new SubscribeGroupPO();
                subscribeGroup.GroupId = args.Sender.Group.Id;
                subscribeGroup.SubscribeId = dbSubscribe.Id;
                subscribeGroup = subscribeGroupDao.Insert(subscribeGroup);

                List<IChatMessage> chailList = new List<IChatMessage>();
                chailList.Add(new PlainMessage($"米游社用户[{dbSubscribe.SubscribeName}]订阅成功!\r\n"));
                chailList.Add(new PlainMessage($"uid：{dbSubscribe.SubscribeCode}\r\n"));
                chailList.Add(new PlainMessage($"频道：{Enum.GetName(typeof(MysSectionType), mysSection)}\r\n"));
                chailList.Add(new PlainMessage($"签名：{dbSubscribe.SubscribeDescription}\r\n"));
                FileInfo fileInfo = string.IsNullOrEmpty(userInfoDto.data.user_info.avatar_url) ? null : await HttpHelper.DownImgAsync(userInfoDto.data.user_info.avatar_url);
                if (fileInfo != null) chailList.Add((IChatMessage)await MiraiHelper.Session.UploadPictureAsync(UploadTarget.Group, fileInfo.FullName));
                await session.SendMessageWithAtAsync(args, chailList);
                ConfigHelper.loadSubscribeTask();
            }
            catch(Exception ex)
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
                MysSectionType? mysSection = null;
                string userId = null;

                string[] paramArr = message.splitParam(BotConfig.SubscribeConfig.Mihoyo.RmCommand);
                if (paramArr == null || paramArr.Length == 0)
                {
                    StepInfo stepInfo = await StepCache.CreateStepAsync(session, args);
                    if (stepInfo == null) return;
                    StepDetail uidStep = new StepDetail(60, " 请在60秒内发送要退订用户的id", CheckUserIdAsync);
                    StepDetail sectionStep = new StepDetail(60, CancleSectionQuestion, CheckSectionAsync);
                    stepInfo.AddStep(uidStep);
                    stepInfo.AddStep(sectionStep);
                    bool isSuccess = await stepInfo.StartStep(session, args);
                    if (isSuccess == false) return;
                    userId = uidStep.Answer;
                    mysSection = (MysSectionType)Convert.ToInt32(sectionStep.Answer);
                }
                else
                {
                    userId = paramArr.Length > 0 ? paramArr[0] : null;
                    string mysSectionStr = paramArr.Length > 1 ? paramArr[1] : "0";
                    if (await CheckUserIdAsync(session, args, userId) == false) return;
                    if (await CheckSectionAsync(session, args, mysSectionStr) == false) return;
                    mysSection = (MysSectionType)Convert.ToInt32(mysSectionStr);
                }

                List<SubscribePO> subscribeList = getSubscribeList(args.Sender.Group.Id, userId);
                if (subscribeList == null || subscribeList.Count == 0)
                {
                    await session.SendMessageWithAtAsync(args, new PlainMessage(" 并没有订阅这个用户哦~"));
                    return;
                }

                foreach (var item in subscribeList)
                {
                    if (mysSection.Value == MysSectionType.全部 || (int)mysSection.Value == item.SubscribeSubType)
                    {
                        subscribeGroupDao.delSubscribe(args.Sender.Group.Id, item.Id);
                    }
                }

                await session.SendMessageWithAtAsync(args, new PlainMessage(" 退订成功~"));
                ConfigHelper.loadSubscribeTask();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "退订米游社用户异常");
                throw;
            }
        }


        /// <summary>
        /// 获取某个群已订阅的列表
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="subscribeCode"></param>
        /// <returns></returns>
        public List<SubscribePO> getSubscribeList(long groupId, string subscribeCode)
        {
            List<SubscribePO> subscribeList = new List<SubscribePO>();
            List<SubscribePO> dbSubscribes = subscribeDao.getSubscribes(subscribeCode, SubscribeType.米游社用户);
            if (dbSubscribes == null || dbSubscribes.Count == 0) return subscribeList;
            foreach (var item in dbSubscribes)
            {
                if (subscribeGroupDao.getCountBySubscribe(groupId, item.Id) == 0) continue;
                subscribeList.Add(item);
            }
            return subscribeList;
        }

        /// <summary>
        /// 删除一个订阅编码下的所有订阅
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="subscribeCode"></param>
        public void delAllSubscribe(long groupId, string subscribeCode)
        {
            List<SubscribePO> dbSubscribes = subscribeDao.getSubscribes(subscribeCode, SubscribeType.米游社用户);
            foreach (var item in dbSubscribes) subscribeGroupDao.delSubscribe(groupId, item.Id);
        }


        private async Task<string> CancleSectionQuestion(IMiraiHttpSession session, IGroupMessageEventArgs args, StepInfo stepInfo, StepDetail currentStep)
        {
            string userIdstr = stepInfo.StepDetails[0].Answer;
            List<SubscribePO> subscribeList = getSubscribeList(args.Sender.Group.Id, userIdstr);
            if (subscribeList == null || subscribeList.Count == 0)
            {
                await session.SendMessageWithAtAsync(args, new PlainMessage(" 并没有订阅这个用户哦~"));
                return null;
            }

            StringBuilder questionBuilder = new StringBuilder();
            questionBuilder.AppendLine($"请在{currentStep.WaitSecond}秒内发送数字选择你要退订的频道：");
            questionBuilder.AppendLine($"{(int)MysSectionType.全部}：{Enum.GetName(typeof(MysSectionType), MysSectionType.全部)}");
            foreach (var item in subscribeList)
            {
                if (item.SubscribeSubType == (int)MysSectionType.全部) continue;
                if (Enum.IsDefined(typeof(MysSectionType), item.SubscribeSubType) == false) continue;
                MysSectionType mysSectionType = (MysSectionType)item.SubscribeSubType;
                questionBuilder.AppendLine($"{item.SubscribeSubType}：{Enum.GetName(typeof(MysSectionType), mysSectionType)}");
            }
            return questionBuilder.ToString();
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


        private async Task<bool> CheckSectionAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string value)
        {
            int sectionId = 0;
            if (int.TryParse(value, out sectionId) == false)
            {
                await session.SendMessageWithAtAsync(args, new PlainMessage(" 频道必须为数字"));
                return false;
            }
            if (Enum.IsDefined(typeof(MysSectionType), sectionId) == false)
            {
                await session.SendMessageWithAtAsync(args, new PlainMessage(" 频道不在范围内"));
                return false;
            }
            return true;
        }


        public async Task<List<MysSubscribe>> getMysUserSubscribeAsync(SubscribeInfo subscribeInfo, int getCount = 2)
        {
            List<MysSubscribe> mysSubscribeList = new List<MysSubscribe>();
            List<MysResult<MysPostDataDto>> postDataList = new List<MysResult<MysPostDataDto>>();
            foreach (var item in Enum.GetValues(typeof(MysSectionType)))
            {
                int typeId = (int)item;
                if (typeId == (int)MysSectionType.全部) continue;
                if (subscribeInfo.SubscribeSubType != (int)MysSectionType.全部 && subscribeInfo.SubscribeSubType != typeId) continue;
                postDataList.Add(await getMysUserPostDtoAsync(subscribeInfo.SubscribeCode, typeId));
                await Task.Delay(1000);
            }

            foreach (var mysPostInfo in postDataList)
            {
                int index = 0;
                if (mysPostInfo.data.list == null || mysPostInfo.data.list.Count == 0) continue;
                foreach (var item in mysPostInfo.data.list)
                {
                    if (++index > getCount) break;
                    int shelfLife = BotConfig.SubscribeConfig.Mihoyo.ShelfLife;
                    DateTime createTime = DateTimeHelper.UnixTimeStampToDateTime(item.post.created_at);
                    if (shelfLife > 0 && createTime < DateTime.Now.AddSeconds(-1 * shelfLife)) continue;

                    SubscribeRecordPO subscribeRecord = new SubscribeRecordPO(subscribeInfo.SubscribeId);
                    subscribeRecord.Title = item.post.subject?.filterEmoji().cutString(200);
                    subscribeRecord.Content = item.post.content?.filterEmoji().cutString(500);
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
