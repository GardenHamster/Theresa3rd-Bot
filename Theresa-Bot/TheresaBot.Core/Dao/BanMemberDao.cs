using TheresaBot.Core.Model.PO;

namespace TheresaBot.Core.Dao
{
    public class BanMemberDao : DbContext<BanMemberPO>
    {
        public List<BanMemberPO> GetBanMembers()
        {
            return Db.Queryable<BanMemberPO>().OrderByDescending(o => o.Id).ToList();
        }

        public BanMemberPO GetBanMember(long memberId)
        {
            return Db.Queryable<BanMemberPO>().Where(o => o.MemberId == memberId).First();
        }

        public int DeleteBanMember(long memberId)
        {
            return Db.Deleteable<BanMemberPO>().Where(o => o.MemberId == memberId).ExecuteCommand();
        }

    }
}
