using TheresaBot.Main.Model.PO;

namespace TheresaBot.Main.Dao
{
    public class UCWordDao : DbContext<UCWordPO>
    {
        public UCWordPO GetRandomWord(List<long> excludeMembers)
        {
            string sqlStr = "select * from uc_word where IsAuthorized=1 and CreateMember not in(@MemberIds) order by rand() limit 1";
            return Db.Ado.SqlQuery<UCWordPO>(sqlStr, new { MemberIds = excludeMembers }).FirstOrDefault();
        }

        public int GetAvailableWordCount()
        {
            string sqlStr = "select count(Id) count from uc_word where IsAuthorized=1 and CreateMember=0";
            return Db.Ado.SqlQuery<int>(sqlStr).First();
        }

    }
}
