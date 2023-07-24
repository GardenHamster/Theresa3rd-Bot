using TheresaBot.Main.Model.PO;

namespace TheresaBot.Main.Dao
{
    public class BanMemberDao : DbContext<BanMemberPO>
    {
        public List<BanMemberPO> getBanMembers()
        {
            return Db.Queryable<BanMemberPO>().ToList();
        }

        public BanMemberPO getBanMember(long memberId)
        {
            return Db.Queryable<BanMemberPO>().Where(o => o.MemberId == memberId).First();
        }

    }
}
