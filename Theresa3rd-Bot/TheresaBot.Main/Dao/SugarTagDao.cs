using TheresaBot.Main.Model.PO;

namespace TheresaBot.Main.Dao
{
    public class SugarTagDao : DbContext<SugarTagPO>
    {

        public List<SugarTagPO> GetSugars()
        {
            return Db.Queryable<SugarTagPO>().ToList();
        }

        public SugarTagPO GetSugar(string keyWord)
        {
            return Db.Queryable<SugarTagPO>().Where(o => o.KeyWord == keyWord).First();
        }

        public int DeleteSugar(string keyWord)
        {
            return Db.Deleteable<SugarTagPO>().Where(o => o.KeyWord == keyWord).ExecuteCommand();
        }



    }
}
