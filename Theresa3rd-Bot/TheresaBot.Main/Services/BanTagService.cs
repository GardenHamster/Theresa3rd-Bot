using TheresaBot.Main.Dao;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Services
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
            return banTagDao.getBanTags();
        }

        public BanTagPO GetBanTag(string keyword)
        {
            return banTagDao.getBanTag(keyword.Trim().ToUpper());
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

        public int DelBanTags(string[] keywords)
        {
            int count = 0;
            foreach (string keyword in keywords)
            {
                count += banTagDao.delBanTag(keyword);
            }
            return count;
        }

        public int DelBanTag(string keyword)
        {
            return banTagDao.delBanTag(keyword.Trim().ToUpper());
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
