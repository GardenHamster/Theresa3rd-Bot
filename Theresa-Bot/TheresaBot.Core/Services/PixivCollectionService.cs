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
            PixivCollectionPO pixivCollection = new PixivCollectionPO();
            pixivCollection.PixivId = workInfo.PixivId;
            pixivCollection.Level = collectionParam.Level;
            pixivCollection.Title = workInfo.illustTitle;
            pixivCollection.UserId = workInfo.UserId;
            pixivCollection.UserName = workInfo.UserName;
            pixivCollection.Pages = workInfo.pageCount;
            pixivCollection.Thumb = workInfo?.urls?.thumb ?? string.Empty;
            pixivCollection.Small = workInfo?.urls?.small ?? string.Empty;
            pixivCollection.Regular = workInfo?.urls?.regular ?? string.Empty;
            pixivCollection.Original = workInfo?.urls?.original ?? string.Empty;
            pixivCollection.IsGif = workInfo.IsGif;
            pixivCollection.IsR18 = workInfo.IsR18;
            pixivCollection.IsOriginal = workInfo.IsOriginal;
            pixivCollection.IsAI = workInfo.IsAI;
            pixivCollection.Width = workInfo.Width;
            pixivCollection.Height = workInfo.Height;
            pixivCollection.LocalPath = localPath;
            pixivCollection.OSSPath = ossPath;
            pixivCollection.CreateDate = workInfo.createDate;
            pixivCollection.AddDate = DateTime.Now;
            pixivCollectionDao.Insert(pixivCollection);
        }

        


    }
}
