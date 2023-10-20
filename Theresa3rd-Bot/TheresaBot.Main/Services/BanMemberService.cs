using TheresaBot.Main.Dao;
using TheresaBot.Main.Model.PO;

namespace TheresaBot.Main.Services
{
    internal class BanMemberService
    {
        private BanMemberDao banMemberDao;

        public BanMemberService()
        {
            banMemberDao = new BanMemberDao();
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

        public int DelBanMember(long memberId)
        {
            return banMemberDao.delBanMember(memberId);
        }

        public int DelById(int id)
        {
            return banMemberDao.DeleteById(id);
        }

    }
}
