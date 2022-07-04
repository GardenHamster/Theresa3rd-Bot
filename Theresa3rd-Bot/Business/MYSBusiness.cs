using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Dao;
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
        /// 获取某个群已订阅的列表
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="subscribeCode"></param>
        /// <returns></returns>
        public List<SubscribePO> getSubscribeList(string subscribeCode)
        {
            List<SubscribePO> subscribeList = new List<SubscribePO>();
            List<SubscribePO> dbSubscribes = subscribeDao.getSubscribes(subscribeCode, SubscribeType.米游社用户);
            if (dbSubscribes == null || dbSubscribes.Count == 0) return subscribeList;
            foreach (var item in dbSubscribes)
            {
                if (subscribeGroupDao.isExistsSubscribeGroup(item.Id)) subscribeList.Add(item);
            }
            return subscribeList;
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
                if (subscribeGroupDao.isExistsSubscribeGroup(groupId, item.Id)) subscribeList.Add(item);
            }
            return subscribeList;
        }

        /// <summary>
        /// 删除一个订阅编码下的所有订阅
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="subscribeCode"></param>
        public void delAllSubscribeGroup(long groupId, string subscribeCode)
        {
            List<SubscribePO> dbSubscribes = subscribeDao.getSubscribes(subscribeCode, SubscribeType.米游社用户);
            foreach (var item in dbSubscribes) subscribeGroupDao.delSubscribeGroup(groupId, item.Id);
        }


        public async Task<List<MysSubscribe>> getMysUserSubscribeAsync(SubscribeTask subscribeTask, int getCount = 5)
        {
            int index = 0;
            int subscribeId = subscribeTask.SubscribeId;
            List<MysSubscribe> mysSubscribeList = new List<MysSubscribe>();
            MysResult<MysPostDataDto> mysPostDataDto = await getMysUserPostDtoAsync(subscribeTask.SubscribeCode, 10);
            if (mysPostDataDto?.data?.list == null || mysPostDataDto.data.list.Count == 0) return mysSubscribeList;
            int shelfLife = BotConfig.SubscribeConfig.Mihoyo.ShelfLife;
            List<MysPostListDto> postList = mysPostDataDto.data.list.OrderByDescending(o => o.post.created_at).ToList();
            foreach (var item in postList)
            {
                try
                {
                    if (++index > getCount) break;
                    string postId = item.post.post_id;
                    DateTime createTime = DateTimeHelper.UnixTimeStampToDateTime(item.post.created_at);
                    if (shelfLife > 0 && createTime < DateTime.Now.AddSeconds(-1 * shelfLife)) break;
                    if (subscribeRecordDao.checkExists(subscribeTask.SubscribeType, postId)) continue;
                    SubscribeRecordPO subscribeRecord = new SubscribeRecordPO(subscribeId);
                    subscribeRecord.Title = item.post.subject?.filterEmoji().cutString(200);
                    subscribeRecord.Content = item.post.content?.filterEmoji().cutString(500);
                    subscribeRecord.CoverUrl = item.post.images.Count > 0 ? item.post.images[0] : "";
                    subscribeRecord.LinkUrl = HttpUrl.getMysArticleUrl(postId);
                    subscribeRecord.DynamicCode = postId;
                    subscribeRecord.DynamicType = SubscribeDynamicType.帖子;
                    MysSubscribe mysSubscribe = new MysSubscribe();
                    mysSubscribe.SubscribeRecord = subscribeRecordDao.Insert(subscribeRecord);
                    mysSubscribe.MysUserPostDto = item;
                    mysSubscribe.CreateTime = createTime;
                    mysSubscribeList.Add(mysSubscribe);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex, $"读取米游社[{subscribeTask.SubscribeId}]贴子[{item.post.post_id}]时出现异常");
                }
                finally
                {
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
            template = template.Replace("{Content}", mysSubscribe.SubscribeRecord.Content.cutString(200));
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
            chailList.Add(new PlainMessage($"{mysSubscribe.SubscribeRecord.Content.cutString(200)}\r\n"));
            FileInfo fileInfo = string.IsNullOrEmpty(mysSubscribe.SubscribeRecord.CoverUrl) ? null : await HttpHelper.DownImgAsync(mysSubscribe.SubscribeRecord.CoverUrl);
            if (fileInfo != null) chailList.Add((IChatMessage)await MiraiHelper.Session.UploadPictureAsync(UploadTarget.Group, fileInfo.FullName));
            chailList.Add(new PlainMessage($"{mysSubscribe.SubscribeRecord.LinkUrl}"));
            return chailList;
        }


        /*-------------------------------------------------------------接口相关--------------------------------------------------------------------------*/

        public async Task<MysResult<MysPostDataDto>> getMysUserPostDtoAsync(string userId, int size)
        {
            Dictionary<string, string> headerDic = new Dictionary<string, string>();
            string getUrl = HttpUrl.getMysPostListUrl(userId, size);
            string json = await HttpHelper.GetAsync(getUrl, headerDic);
            return JsonConvert.DeserializeObject<MysResult<MysPostDataDto>>(json);
        }


        public async Task<MysResult<MysUserFullInfoDto>> geMysUserFullInfoDtoAsync(string userId)
        {
            Dictionary<string, string> headerDic = new Dictionary<string, string>();
            string getUrl = HttpUrl.getMystUserFullInfo(userId);
            string json = await HttpHelper.GetAsync(getUrl, headerDic);
            return JsonConvert.DeserializeObject<MysResult<MysUserFullInfoDto>>(json);
        }


    }
}
