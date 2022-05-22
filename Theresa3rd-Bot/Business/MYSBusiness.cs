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
        private SubscribeRecordDao subscribeRecordDao;

        public MYSBusiness()
        {
            subscribeRecordDao = new SubscribeRecordDao();
        }

        public async Task subscribeMYSUserAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string message)
        {
            StepInfo stepInfo = StepCache.CreateStep(args.Sender.Group.Id, args.Sender.Id);
            if (stepInfo == null)
            {
                await session.SendGroupMessageAsync(args.Sender.Group.Id, new PlainMessage("你的一个订阅任务正在执行中，请等待执行完毕后重试"));
                return;
            }

            StepDetail sectionStep = new StepDetail(60, $"选择你要订阅的版块：\r\n{EnumHelper.MysSectionOption}", CheckSectionAsync);
            StepDetail uidStep = new StepDetail(60, "输入要订阅用户的id:");
            stepInfo.AddStep(sectionStep);
            stepInfo.AddStep(uidStep);
            bool isSuccess= stepInfo.StartStep(session, args).Result;
            if (isSuccess == false) return;

            MysSectionType mysSectionType = (MysSectionType)Convert.ToInt32(sectionStep.Answer);
            string userId = uidStep.Answer;


        }

        private async Task<bool> CheckSectionAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, string value)
        {
            int sectionType = 0;
            if (int.TryParse(value, out sectionType) == false)
            {
                await session.SendMessageWithAtAsync(args, new PlainMessage("输入的内容必须是数字"));
                return false;
            }
            if (Enum.IsDefined(typeof(MysSectionType), value) == false)
            {
                await session.SendMessageWithAtAsync(args, new PlainMessage("输入的内容不在范围内"));
                return false;
            }
            return true;
        }

        public async Task<List<MysSubscribe>> getMysUserSubscribeAsync(MysSectionType sectionType, int subscribeId,  string userCode, int getCount = 2)
        {
            int index = 0;
            List<MysSubscribe> mysSubscribeList = new List<MysSubscribe>();
            MysResult<MysPostDataDto> mysPostInfo = getMysUserPostDto(userCode, sectionType);
            List<MysPostListDto> postList = mysPostInfo.data.list;
            if (postList.Count == 0) return mysSubscribeList;
            foreach (var item in postList)
            {
                if (++index > getCount) break;
                int shelfLife = BotConfig.SubscribeConfig.Mihoyo.ShelfLife;
                DateTime createTime = DateTimeHelper.UnixTimeStampToDateTime(item.post.created_at);
                if (shelfLife > 0 && createTime < DateTime.Now.AddSeconds(-1 * shelfLife)) continue;
                
                SubscribeRecordPO subscribeRecord = new SubscribeRecordPO(subscribeId);
                subscribeRecord.Title = item.post.subject.cutString(200);
                subscribeRecord.Content = item.post.content.cutString(500);
                subscribeRecord.CoverUrl = item.post.images.Count > 0 ? item.post.images[0] : "";
                subscribeRecord.LinkUrl = HttpUrl.getMysArticleUrl(item.post.post_id);
                subscribeRecord.DynamicCode = item.post.post_id;
                subscribeRecord.DynamicType = SubscribeDynamicType.帖子;

                SubscribeRecordPO dbSubscribe = subscribeRecordDao.checkExists(subscribeId, item.post.post_id);
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


        public MysResult<MysPostDataDto> getMysUserPostDto(string userId, MysSectionType subType)
        {
            Dictionary<string, string> headerDic = new Dictionary<string, string>();
            string getUrl = HttpUrl.getMysPostListUrl(userId, (int)subType);
            string json = HttpHelper.HttpGetAsync(getUrl, headerDic).Result;
            return JsonConvert.DeserializeObject<MysResult<MysPostDataDto>>(json);
        }

        public async Task<List<IChatMessage>> getSubscribeInfoAsync(MysSubscribe mysSubscribe, string template = "")
        {
            if (string.IsNullOrWhiteSpace(template)) return getDefaultSubscribeInfoAsync(mysSubscribe).Result;
            template = template.Replace("{UserName}", mysSubscribe.MysUserPostDto.user.nickname);
            template = template.Replace("{CreateTime}", mysSubscribe.CreateTime.ToSimpleString());
            template = template.Replace("{Title}", mysSubscribe.SubscribeRecord.Title);
            template = template.Replace("{Content}", mysSubscribe.SubscribeRecord.Content);
            template = template.Replace("{Urls}", mysSubscribe.SubscribeRecord.LinkUrl);
            List<IChatMessage> chailList = new List<IChatMessage>();
            chailList.Add(new PlainMessage(template));
            FileInfo fileInfo = string.IsNullOrEmpty(mysSubscribe.SubscribeRecord.CoverUrl) ? null : HttpHelper.DownImgAsync(mysSubscribe.SubscribeRecord.CoverUrl).Result;
            if (fileInfo != null) chailList.Add((IChatMessage)await MiraiHelper.Session.UploadPictureAsync(UploadTarget.Group, fileInfo.FullName));
            return chailList;
        }

        public async Task<List<IChatMessage>> getDefaultSubscribeInfoAsync(MysSubscribe mysSubscribe)
        {
            List<IChatMessage> chailList = new List<IChatMessage>();
            chailList.Add(new PlainMessage($"米游社[{mysSubscribe.MysUserPostDto.user.nickname}]发布了新帖子，发布时间{mysSubscribe.CreateTime.ToSimpleString()}：\r\n"));
            chailList.Add(new PlainMessage($"{mysSubscribe.SubscribeRecord.Title}\r\n"));
            chailList.Add(new PlainMessage($"{mysSubscribe.SubscribeRecord.Content}\r\n"));
            FileInfo fileInfo = string.IsNullOrEmpty(mysSubscribe.SubscribeRecord.CoverUrl) ? null : HttpHelper.DownImgAsync(mysSubscribe.SubscribeRecord.CoverUrl).Result;
            if (fileInfo != null) chailList.Add((IChatMessage)await MiraiHelper.Session.UploadPictureAsync(UploadTarget.Group, fileInfo.FullName));
            chailList.Add(new PlainMessage($"{mysSubscribe.SubscribeRecord.LinkUrl}"));
            return chailList;
        }


    }
}
