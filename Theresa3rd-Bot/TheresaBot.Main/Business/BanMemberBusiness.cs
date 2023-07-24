using TheresaBot.Main.Dao;
using TheresaBot.Main.Model.PO;

namespace TheresaBot.Main.Business
{
    internal class BanMemberBusiness
    {
        private BanMemberDao banMemberDao;

        public BanMemberBusiness()
        {
            this.banMemberDao = new BanMemberDao();
        }

        public List<BanMemberPO> getBanMembers()
        {
            return banMemberDao.getBanMembers();
        }

        public BanMemberPO getBanMember(long memberId)
        {
            return banMemberDao.getBanMember(memberId);
        }

        public BanMemberPO insertBanMembers(long memberId)
        {
            BanMemberPO banMember = new BanMemberPO();
            banMember.MemberId = memberId;
            banMember.CreateDate = DateTime.Now;
            return banMemberDao.Insert(banMember);
        }

        public int DelBanMember(BanMemberPO banMember)
        {
            return banMemberDao.Delete(banMember);
        }



    }
}
