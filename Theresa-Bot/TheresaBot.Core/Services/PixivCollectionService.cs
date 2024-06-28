using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheresaBot.Core.Dao;
using TheresaBot.Core.Model.Pixiv;
using TheresaBot.Core.Model.PO;

namespace TheresaBot.Core.Services
{
    internal class PixivCollectionService
    {
        private PixivCollectionDao pixivCollectionDao;

        public PixivCollectionService()
        {
            pixivCollectionDao = new PixivCollectionDao();
        }

        public void AddPixivCollection(PixivWorkInfo workInfo, PixivCollectionParam collectionParam, string localPath, string ossPath)
        {
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
        }

        public void AddTags()
        {
            
        }

        


    }
}
