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

        public List<BanMemberPO> GetBanMembers()
        {
            return banMemberDao.GetBanMembers();
        }

        public BanMemberPO GetBanMember(long memberId)
        {
            return banMemberDao.GetBanMember(memberId);
        }

        public void InsertBanMember(long[] memberIds)
        {
            foreach (var memberId in memberIds)
            {
                InsertBanMember(memberId);
            }
        }

        public BanMemberPO InsertBanMember(long memberId)
        {
            var banMember = banMemberDao.GetBanMember(memberId);
            if (banMember is not null)
            {
                return banMember;
            }
            else
            {
                banMember = new BanMemberPO();
                banMember.MemberId = memberId;
                banMember.CreateDate = DateTime.Now;
                return banMemberDao.Insert(banMember);
            }
        }

        public void DelBanMember(long[] memberIds)
        {
            foreach (var memberId in memberIds)
            {
                DelBanMember(memberId);
            }
        }

        public int DelBanMember(long memberId)
        {
            return banMemberDao.DeleteBanMember(memberId);
        }

        public int DelById(int id)
        {
            return banMemberDao.DeleteById(id);
        }

        public int DelByIds(int[] ids)
        {
            return banMemberDao.DeleteByIds(ids);
        }

    }
}
