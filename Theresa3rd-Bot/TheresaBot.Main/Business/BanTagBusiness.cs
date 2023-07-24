using TheresaBot.Main.Dao;
using TheresaBot.Main.Model.PO;
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

        public BanTagPO getBanTag(string keyword)
        {
            return banTagDao.getBanTag(keyword);
        }

        public BanTagPO AddBanTag(string keyword, bool fullMatch, bool isRegular = false)
        {
            BanTagPO banTag = new BanTagPO();
            banTag.KeyWord = keyword;
            banTag.IsRegular = isRegular;
            banTag.FullMatch = fullMatch;
            banTag.CreateDate = DateTime.Now;
            return banTagDao.Insert(banTag);
        }

        public BanTagPO AddBanTag(string keyword, TagBanType tagBanType)
        {
            BanTagPO banTag = new BanTagPO();
            banTag.KeyWord = keyword;
            banTag.IsRegular = tagBanType == TagBanType.Regular;
            banTag.FullMatch = tagBanType == TagBanType.FullMatch;
            banTag.CreateDate = DateTime.Now;
            return banTagDao.Insert(banTag);
        }

        public int DelBanTag(BanTagPO banTag)
        {
            return banTagDao.Delete(banTag);
        }


    }
}
