using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        private SubscribeRecordDao subscribeRecordDao;

        public MYSBusiness()
        {
            subscribeRecordDao = new SubscribeRecordDao();
        }

        public async Task<List<MysSubscribe>> getMysUserSubscribeAsync(MysSectionType sectionType, int subscribeId,  string userCode, int getCount = 2)
        {
            int index = 0;
            List<MysSubscribe> mysSubscribeList = new List<MysSubscribe>();
            MysResult<MysUserPostDataDto> pixivWorkInfo = getMysUserPostDto(userCode, sectionType);
            List<MysUserPostDto> postList = pixivWorkInfo.data.list;
            if (postList.Count == 0) return mysSubscribeList;
            foreach (var item in postList)
            {
                int shelfLife = BotConfig.SubscribeConfig.PixivTag.ShelfLife;
                DateTime createTime = DateTimeHelper.UnixTimeStampToDateTime(item.post.created_at);
                if (shelfLife > 0 && createTime < DateTime.Now.AddSeconds(-1 * shelfLife)) continue;
                
                if (++index > getCount) break;
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
                mysSubscribeList.Add(mysSubscribe);
                await Task.Delay(1000);
            }
            return mysSubscribeList;
        }


        public MysResult<MysUserPostDataDto> getMysUserPostDto(string userId, MysSectionType subType)
        {
            Dictionary<string, string> headerDic = new Dictionary<string, string>();
            string getUrl = HttpUrl.getMysPostListUrl(userId, (int)subType);
            string json = HttpHelper.HttpGet(getUrl, headerDic);
            return JsonConvert.DeserializeObject<MysResult<MysUserPostDataDto>>(json);
        }


    }
}
