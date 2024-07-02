using AngleSharp.Dom;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TheresaBot.Core.Dao;
using TheresaBot.Core.Helper;
using TheresaBot.Core.Model.Pixiv;
using TheresaBot.Core.Model.PO;
using YamlDotNet.Core;

namespace TheresaBot.Core.Services
{
    internal class PixivCollectionService
    {
        private PixivTagDao pixivTagDao;
        private PixivCollectionDao pixivCollectionDao;
        private PixivCollectionTagDao pixivCollectionTagDao;

        public PixivCollectionService()
        {
            pixivTagDao = new PixivTagDao();
            pixivCollectionDao = new PixivCollectionDao();
            pixivCollectionTagDao = new PixivCollectionTagDao();
        }

        public async Task<PixivCollectionPO> AddPixivCollection(PixivWorkInfo workInfo, PixivCollectionParam collectionParam, string localPath, string ossPath)
        {
            var extraTags = collectionParam.Tags;
            var collection = pixivCollectionDao.GetByPixivId(workInfo.PixivId);
            if (collection == null) collection = new PixivCollectionPO();
            collection.PixivId = workInfo.PixivId;
            collection.Level = collectionParam.Level;
            collection.Title = workInfo.illustTitle;
            collection.UserId = workInfo.UserId;
            collection.UserName = workInfo.UserName;
            collection.Pages = workInfo.pageCount;
            collection.Thumb = workInfo?.urls?.thumb ?? string.Empty;
            collection.Small = workInfo?.urls?.small ?? string.Empty;
            collection.Regular = workInfo?.urls?.regular ?? string.Empty;
            collection.Original = workInfo?.urls?.original ?? string.Empty;
            collection.IsGif = workInfo.IsGif;
            collection.IsR18 = workInfo.IsR18;
            collection.IsOriginal = workInfo.IsOriginal;
            collection.IsAI = workInfo.IsAI;
            collection.Width = workInfo.Width;
            collection.Height = workInfo.Height;
            collection.LocalPath = localPath;
            collection.OSSPath = ossPath;
            collection.CreateDate = workInfo.createDate;
            collection.AddDate = DateTime.Now;
            pixivCollectionDao.InsertOrUpdate(collection);
            await AddPixivTags(collection, workInfo);
            await AddPixivTags(collection, extraTags);
            return collection;
        }


        private async Task AddPixivTags(PixivCollectionPO collection, PixivWorkInfo pixivWorkInfo)
        {
            if (pixivWorkInfo.tags == null) return;
            if (pixivWorkInfo.tags.tags == null) return;
            foreach (var item in pixivWorkInfo.tags.tags)
            {
                var tagName = item?.tag?.Trim() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(tagName)) continue;
                await AddPixivTag(collection, tagName, false);
            }
        }

        private async Task AddPixivTags(PixivCollectionPO collection, List<string> extraTags)
        {
            foreach (var tagName in extraTags)
            {
                if (string.IsNullOrWhiteSpace(tagName)) continue;
                await AddPixivTag(collection, tagName, true);
            }
        }

        private async Task<PixivCollectionTagPO> AddPixivTag(PixivCollectionPO collection, string tagName, bool isExtra)
        {
            var pixivTag = pixivTagDao.getTag(tagName);
            if (pixivTag == null)
            {
                pixivTag = await GetTagFromPixiv(tagName);
                pixivTag = pixivTagDao.Insert(pixivTag);
            }
            var collectionTag = new PixivCollectionTagPO();
            collectionTag.CollectionId = collection.Id;
            collectionTag.TagId = pixivTag.Id;
            collectionTag.IsExtra = isExtra;
            return pixivCollectionTagDao.Insert(collectionTag);
        }

        private async Task<PixivTagPO> GetTagFromPixiv(string tagName)
        {
            JObject jObject = await PixivHelper.getPixivTagsAsync(tagName);
            return JObjectToPixivTag(jObject, tagName);
        }

        private PixivTagPO JObjectToPixivTag(JObject jObject, string tagName)
        {
            try
            {
                PixivTagPO pixivTag = new PixivTagPO(tagName);
                JObject body = jObject.Value<JObject>("body");
                JObject translationObj = body.Value<JObject>("tagTranslation");
                JObject tagObj = translationObj.Value<JObject>(tagName);
                pixivTag.Zh = tagObj.Value<string>("zh") ?? "";
                pixivTag.ZhTw = tagObj.Value<string>("zh_tw") ?? "";
                pixivTag.En = tagObj.Value<string>("en") ?? "";
                pixivTag.Ko = tagObj.Value<string>("ko") ?? "";
                return pixivTag;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"获取Pixiv标签【{tagName}】详情失败");
                return new PixivTagPO(tagName);
            }
        }




    }
}
