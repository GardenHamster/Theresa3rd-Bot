using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Dao
{
    public class DictionaryDao : DbContext<DictionaryPO>
    {
        public List<DictionaryPO> getDictionary(WordType wordType)
        {
            return Db.Queryable<DictionaryPO>().Where(o => o.WordType == wordType).ToList();
        }

        public List<DictionaryPO> getDictionary(WordType wordType, int subType)
        {
            return Db.Queryable<DictionaryPO>().Where(o => o.WordType == wordType && o.SubType == subType).ToList();
        }

        public List<DictionaryPO> getDictionary(WordType wordType, string word)
        {
            return Db.Queryable<DictionaryPO>().Where(o => o.WordType == wordType && o.Words == word).ToList();
        }

        public List<DictionaryPO> getDictionary(WordType wordType, int subType, string word)
        {
            return Db.Queryable<DictionaryPO>().Where(o => o.WordType == wordType && o.SubType == subType && o.Words == word).ToList();
        }

    }
}
