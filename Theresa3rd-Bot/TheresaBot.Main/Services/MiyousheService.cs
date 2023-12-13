using Newtonsoft.Json;
using System.Text;
using TheresaBot.Main.Common;
using TheresaBot.Main.Dao;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Mys;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Model.Subscribe;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Services
{
    internal class MiyousheService
    {
        private SubscribeRecordDao subscribeRecordDao;

        public MiyousheService()
        {
            subscribeRecordDao = new SubscribeRecordDao();
        }

        public async Task<List<MysSubscribe>> scanPostAsync(SubscribeTask subscribeTask)
        {
            var index = 0;
            var maxScan = 10;
            var subscribeId = subscribeTask.SubscribeId;
            var mysPostList = new List<MysSubscribe>();
            int shelfLife = BotConfig.SubscribeConfig.Miyoushe.ShelfLife;
            var mysPostData = await getUserPostAsync(subscribeTask.SubscribeCode, 10);
            if (mysPostData?.data?.list is null || mysPostData.data.list.Count == 0) return mysPostList;
            var postList = mysPostData.data.list.OrderByDescending(o => o.post.created_at).ToList();
            foreach (var item in postList)
            {
                try
                {
                    if (++index > maxScan) break;
                    if (item.post.IsExpired(shelfLife)) break;
                    string postId = item.post.post_id;
                    if (subscribeRecordDao.checkExists(subscribeTask.SubscribeType, postId)) continue;
                    SubscribeRecordPO subscribeRecord = new SubscribeRecordPO(subscribeId);
                    subscribeRecord.Title = item.post.subject?.FilterEmoji()?.CutString(200) ?? string.Empty;
                    subscribeRecord.Content = item.post.content?.FilterEmoji()?.CutString(200) ?? string.Empty;
                    subscribeRecord.CoverUrl = item.post.images.FirstOrDefault() ?? string.Empty;
                    subscribeRecord.LinkUrl = HttpUrl.getMysArticleUrl(postId);
                    subscribeRecord.DynamicType = SubscribeDynamicType.帖子;
                    subscribeRecord.DynamicCode = postId;
                    MysSubscribe mysSubscribe = new MysSubscribe();
                    mysSubscribe.SubscribeRecord = subscribeRecordDao.Insert(subscribeRecord);
                    mysSubscribe.CreateTime = item.post.CreateTime;
                    mysSubscribe.MysUserPostDto = item;
                    mysPostList.Add(mysSubscribe);
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
            return mysPostList;
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

        public async Task<MysResult<MysPostDataDto>> getUserPostAsync(string userId, int size)
        {
            string referer = HttpUrl.getMysPostListRefer(userId);
            Dictionary<string, string> headerDic = GetHeaders(referer);
            string getUrl = HttpUrl.getMysPostListUrl(userId, size);
            string json = await HttpHelper.GetAsync(getUrl, headerDic);
            return JsonConvert.DeserializeObject<MysResult<MysPostDataDto>>(json);
        }

        public async Task<MysResult<MysUserDataDto>> getUserInfoAsync(string userId)
        {
            string referer = HttpUrl.getMysUserInfoRefer(userId);
            Dictionary<string, string> headerDic = GetHeaders(referer);
            string getUrl = HttpUrl.getMysUserInfoUrl(userId);
            string json = await HttpHelper.GetAsync(getUrl, headerDic);
            return JsonConvert.DeserializeObject<MysResult<MysUserDataDto>>(json);
        }

        private static Dictionary<string, string> GetHeaders(string referer)
        {
            Dictionary<string, string> headerDic = new Dictionary<string, string>();
            headerDic.Add("referer", referer);
            return headerDic;
        }


    }
}
