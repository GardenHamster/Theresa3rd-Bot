using TheresaBot.Main.Dao;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Services
{
    public class DictionaryService
    {
        private DictionaryDao dictionaryDao;

        public DictionaryService()
        {
            dictionaryDao = new DictionaryDao();
        }

        public List<DictionaryPO> GetDictionary(WordType wordType)
        {
            return dictionaryDao.GetDictionary(wordType);
        }

        public List<DictionaryPO> GetDictionary(WordType wordType, int subType)
        {
            return dictionaryDao.GetDictionary(wordType, subType);
        }

        public List<DictionaryPO> GetDictionary(WordType wordType, string word)
        {
            return dictionaryDao.GetDictionary(wordType, word);
        }

        public List<DictionaryPO> GetDictionary(WordType wordType, int subType, string word)
        {
            return dictionaryDao.GetDictionary(wordType, subType, word);
        }

        public void InsertDictionary(WordType wordType, string word, int subType = 0, string translate = "")
        {
            DictionaryPO dictionary = new DictionaryPO();
            dictionary.Words = word;
            dictionary.WordType = wordType;
            dictionary.SubType = subType;
            dictionary.CreateDate = DateTime.Now;
            dictionaryDao.Insert(dictionary);
        }


    }
}
