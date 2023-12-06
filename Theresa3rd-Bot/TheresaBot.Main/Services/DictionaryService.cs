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

        public List<DictionaryPO> getDictionary(WordType wordType)
        {
            return dictionaryDao.getDictionary(wordType);
        }

        public List<DictionaryPO> getDictionary(WordType wordType, int subType)
        {
            return dictionaryDao.getDictionary(wordType, subType);
        }

        public List<DictionaryPO> getDictionary(WordType wordType, string word)
        {
            return dictionaryDao.getDictionary(wordType, word);
        }

        public List<DictionaryPO> getDictionary(WordType wordType, int subType, string word)
        {
            return dictionaryDao.getDictionary(wordType, subType, word);
        }

        public void addDictionary(WordType wordType, string word, int subType = 0, string translate = "")
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
