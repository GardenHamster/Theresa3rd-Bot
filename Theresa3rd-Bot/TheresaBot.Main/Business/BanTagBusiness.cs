using TheresaBot.Main.Dao;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Model.Result;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Business
{
    internal class BanTagBusiness
    {
        private BanTagDao banTagDao;

        public BanTagBusiness()
        {
            this.banTagDao = new BanTagDao();
        }

        public List<BanTagPO> GetBanTags()
        {
            return banTagDao.getBanTags();
        }

        public BanTagPO GetBanTag(string keyword)
        {
            return banTagDao.getBanTag(keyword.Trim().ToUpper());
        }

        public void InsertOrUpdate(ModifyResult result, string[] keywords, TagMatchType tagMatchType)
        {
            foreach (string keyword in keywords)
            {
                InsertOrUpdate(result, keyword, tagMatchType);
            }
        }

        public void InsertOrUpdate(ModifyResult result, string keyword, TagMatchType tagMatchType)
        {
            var banTag = GetBanTag(keyword);
            if (banTag is null)
            {
                InsertBanTag(keyword, tagMatchType);
                result.CreateCount++;
            }
            else
            {
                UpdateBanTag(banTag, tagMatchType);
                result.UpdateCount++;
            }
        }

        public BanTagPO InsertBanTag(string keyword, TagMatchType tagMatchType)
        {
            BanTagPO banTag = new BanTagPO();
            banTag.KeyWord = keyword.Trim().ToUpper();
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


    }
}
