using TheresaBot.Main.Model.PO;

namespace TheresaBot.Main.Dao
{
    public class UCWordDao : DbContext<UCWordPO>
    {
        public bool CheckWordExist(string[] word)
        {
            return Db.Queryable<UCWordPO>().Any(o => (o.Word1 == word[0] && o.Word2 == word[1]) || (o.Word2 == word[0] && o.Word1 == word[1]));
        }

        public UCWordPO GetRandomWord(List<long> excludeMembers)
        {
            string sqlStr = "select * from uc_word where IsAuthorized=1 and CreateMember not in(@MemberIds) order by rand() limit 1";
            return Db.Ado.SqlQuery<UCWordPO>(sqlStr, new { MemberIds = excludeMembers }).FirstOrDefault();
        }

        public List<UCWordPO> GetWords()
        {
            return Db.Queryable<UCWordPO>().OrderByDescending(o => o.Id).ToList();
        }

        public int GetAvailableWordCount()
        {
            string sqlStr = "select count(Id) count from uc_word where IsAuthorized=1 and CreateMember=0";
            return Db.Ado.SqlQuery<int>(sqlStr).First();
        }

        public int GetUnauthorizedCount(long memberId)
        {
            return Db.Queryable<UCWordPO>().Where(o => o.IsAuthorized == false && o.CreateMember == memberId).Count();
        }

        public int AuthorizeWords(List<int> ids)
        {
            return Db.Updateable<UCWordPO>().SetColumns(o => o.IsAuthorized == true).Where(o => ids.Contains(o.Id)).ExecuteCommand();
        }


    }
}
