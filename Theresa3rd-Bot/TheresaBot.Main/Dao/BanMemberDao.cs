using TheresaBot.Main.Model.PO;

namespace TheresaBot.Main.Dao
{
    public class BanMemberDao : DbContext<BanMemberPO>
    {
        public List<BanMemberPO> GetBanMembers()
        {
            return Db.Queryable<BanMemberPO>().ToList();
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
