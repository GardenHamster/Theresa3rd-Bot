using TheresaBot.Core.Dao;
using TheresaBot.Core.Model.PO;
using TheresaBot.Core.Type;

namespace TheresaBot.Core.Services
{
    public class DictionaryService
    {
        private DictionaryDao dictionaryDao;

        public DictionaryService()
        {
            dictionaryDao = new DictionaryDao();
        }

        public List<DictionaryPO> GetDictionary()
        {
            return dictionaryDao.GetDictionary();
        }

        public List<DictionaryPO> GetDictionary(DictionaryType wordType)
        {
            return dictionaryDao.GetDictionary(wordType);
        }

        public List<DictionaryPO> GetDictionary(DictionaryType wordType, int subType)
        {
            return dictionaryDao.GetDictionary(wordType, subType);
        }

        public List<DictionaryPO> GetDictionary(DictionaryType wordType, string word)
        {
            return dictionaryDao.GetDictionary(wordType, word);
        }

        public List<DictionaryPO> GetDictionary(DictionaryType wordType, int subType, string word)
        {
            return dictionaryDao.GetDictionary(wordType, subType, word);
        }

        public void InsertDictionary(DictionaryType wordType, List<string> words, int subType = 0)
        {
            foreach (string word in words)
            {
                InsertDictionary(wordType, word, subType);
            }
        }

        public void InsertDictionary(DictionaryType wordType, string word, int subType = 0)
        {
            DictionaryPO dictionary = new DictionaryPO();
            dictionary.Words = word;
            dictionary.WordType = wordType;
            dictionary.SubType = subType;
            dictionary.CreateDate = DateTime.Now;
            dictionaryDao.Insert(dictionary);
        }

        public int DelByIds(int[] ids)
        {
            return dictionaryDao.DeleteByIds(ids);
        }

    }
}
