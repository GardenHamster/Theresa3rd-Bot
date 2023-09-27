using Newtonsoft.Json;
using System.Text;
using TheresaBot.Main.Common;
using TheresaBot.Main.Dao;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Mys;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Model.Subscribe;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Business
{
    internal class MiyousheBusiness
    {
        private SubscribeDao subscribeDao;
        private SubscribeGroupDao subscribeGroupDao;
        private SubscribeRecordDao subscribeRecordDao;

        public MiyousheBusiness()
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
            if (dbSubscribes is null || dbSubscribes.Count == 0) return subscribeList;
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
            if (dbSubscribes is null || dbSubscribes.Count == 0) return subscribeList;
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
            foreach (var item in dbSubscribes) subscribeGroupDao.delBySubscribeId(groupId, item.Id);
        }


        public async Task<List<MysSubscribe>> getMysUserSubscribeAsync(SubscribeTask subscribeTask, int getCount = 5)
        {
            int index = 0;
            int subscribeId = subscribeTask.SubscribeId;
            List<MysSubscribe> mysSubscribeList = new List<MysSubscribe>();
            MysResult<MysPostDataDto> mysPostDataDto = await getMysUserPostDtoAsync(subscribeTask.SubscribeCode, 10);
            if (mysPostDataDto?.data?.list is null || mysPostDataDto.data.list.Count == 0) return mysSubscribeList;
            int shelfLife = BotConfig.SubscribeConfig.Miyoushe.ShelfLife;
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
                    subscribeRecord.Title = item.post.subject?.FilterEmoji().CutString(200);
                    subscribeRecord.Content = item.post.content?.FilterEmoji().CutString(200);
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


        public string getPostInfoAsync(MysSubscribe mysSubscribe)
        {
            string template = BotConfig.SubscribeConfig.Miyoushe.Template?.Trim()?.TrimLine();
            if (string.IsNullOrWhiteSpace(template)) return getDefaultPostInfoAsync(mysSubscribe);
            template = template.Replace("{UserName}", mysSubscribe.MysUserPostDto.user.nickname);
            template = template.Replace("{CreateTime}", mysSubscribe.CreateTime.ToSimpleString());
            template = template.Replace("{Title}", mysSubscribe.SubscribeRecord.Title);
            template = template.Replace("{Content}", mysSubscribe.SubscribeRecord.Content);
            template = template.Replace("{Urls}", mysSubscribe.SubscribeRecord.LinkUrl);
            return template;
        }

        private string getDefaultPostInfoAsync(MysSubscribe mysSubscribe)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"{mysSubscribe.SubscribeRecord.Title}");
            stringBuilder.AppendLine($"{mysSubscribe.SubscribeRecord.Content.CutString(300)}");
            stringBuilder.Append($"{mysSubscribe.SubscribeRecord.LinkUrl}");
            return stringBuilder.ToString();
        }

        /*-------------------------------------------------------------接口相关--------------------------------------------------------------------------*/

        public async Task<MysResult<MysPostDataDto>> getMysUserPostDtoAsync(string userId, int size)
        {
            string referer = HttpUrl.getMysPostListRefer(userId);
            Dictionary<string, string> headerDic = GetMysHeader(referer);
            string getUrl = HttpUrl.getMysPostListUrl(userId, size);
            string json = await HttpHelper.GetAsync(getUrl, headerDic);
            return JsonConvert.DeserializeObject<MysResult<MysPostDataDto>>(json);
        }

        public async Task<MysResult<MysUserFullInfoDto>> geMysUserFullInfoDtoAsync(string userId)
        {
            string referer = HttpUrl.getMysUserInfoRefer(userId);
            Dictionary<string, string> headerDic = GetMysHeader(referer);
            string getUrl = HttpUrl.getMysUserInfoUrl(userId);
            string json = await HttpHelper.GetAsync(getUrl, headerDic);
            return JsonConvert.DeserializeObject<MysResult<MysUserFullInfoDto>>(json);
        }

        private static Dictionary<string, string> GetMysHeader(string referer)
        {
            Dictionary<string, string> headerDic = new Dictionary<string, string>();
            headerDic.Add("referer", referer);
            return headerDic;
        }


    }
}
