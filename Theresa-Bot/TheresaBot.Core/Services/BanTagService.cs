using TheresaBot.Core.Dao;
using TheresaBot.Core.Model.PO;
using TheresaBot.Core.Type;

namespace TheresaBot.Core.Services
{
    internal class BanTagService
    {
        private BanTagDao banTagDao;

        public BanTagService()
        {
            banTagDao = new BanTagDao();
        }

        public List<BanTagPO> GetBanTags()
        {
            return banTagDao.GetBanTags();
        }

        public BanTagPO GetBanTag(string keyword)
        {
            return banTagDao.GetBanTag(keyword.Trim().ToUpper());
        }

        public void InsertOrUpdate(string[] keywords, TagMatchType tagMatchType)
        {
            foreach (string keyword in keywords)
            {
                InsertOrUpdate(keyword, tagMatchType);
            }
        }

        public void InsertOrUpdate(string keyword, TagMatchType tagMatchType)
        {
            var banTag = GetBanTag(keyword);
            if (banTag is null)
            {
                InsertBanTag(keyword, tagMatchType);
            }
            else
            {
                UpdateBanTag(banTag, tagMatchType);
            }
        }

        public BanTagPO InsertBanTag(string keyword, TagMatchType tagMatchType)
        {
            BanTagPO banTag = new BanTagPO();
            banTag.Keyword = keyword.Trim().ToUpper();
            banTag.IsRegular = tagMatchType == TagMatchType.Regular;
            banTag.FullMatch = tagMatchType == TagMatchType.FullMatch;
            banTag.CreateDate = DateTime.Now;
            return banTagDao.Insert(banTag);
        }

        public int UpdateBanTag(BanTagPO banTag, TagMatchType tagMatchType)
        {
            banTag.IsRegular = tagMatchType == TagMatchType.Regular;
            banTag.FullMatch = tagMatchType == TagMatchType.FullMatch;
            return banTagDao.Update(banTag);
        }

        public void DelBanTags(string[] keywords)
        {
            foreach (string keyword in keywords)
            {
                banTagDao.DeleteBanTag(keyword);
            }
        }

        public int DelBanTag(string keyword)
        {
            return banTagDao.DeleteBanTag(keyword.Trim().ToUpper());
        }

        public int DelById(int id)
        {
            return banTagDao.DeleteById(id);
        }

        public int DelByIds(int[] ids)
        {
            return banTagDao.DeleteByIds(ids);
        }


    }
}
