using TheresaBot.Main.Dao;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.PO;

namespace TheresaBot.Main.Services
{
    public class UCWordService
    {
        private UCWordDao ucWordDao;

        public UCWordService()
        {
            ucWordDao = new UCWordDao();
        }

        public void AddWords(string word1, string word2)
        {
            UCWordPO wordPO = new UCWordPO
            {
                Word1 = word1,
                Word2 = word2,
                CreateMember = 0,
                IsAuthorized = true,
                CreateDate = DateTime.Now,
            };
            ucWordDao.Insert(wordPO);
        }

        public void AddWords(string word1, string word2, long memberId)
        {
            UCWordPO wordPO = new UCWordPO
            {
                Word1 = word1,
                Word2 = word2,
                CreateMember = memberId.IsSuperManager() ? 0 : memberId,
                IsAuthorized = memberId.IsSuperManager(),
                CreateDate = DateTime.Now,
            };
            ucWordDao.Insert(wordPO);
        }

        public bool CheckWordExist(string[] word)
        {
            return ucWordDao.CheckWordExist(word);
        }

        public UCWordPO GetRandomWord(List<long> excludeMembers)
        {
            return ucWordDao.GetRandomWord(excludeMembers);
        }

        public List<UCWordPO> GetWords()
        {
            return ucWordDao.GetWords();
        }

        public int GetAvailableWordCount()
        {
            return ucWordDao.GetAvailableWordCount();
        }

        public int GetUnauthorizedCount(long memberId)
        {
            return ucWordDao.GetUnauthorizedCount(memberId);
        }

        public int deleteWords(int id)
        {
            return ucWordDao.DeleteById(id);
        }

        public int deleteWords(List<int> ids)
        {
            return ucWordDao.DeleteByIds(ids);
        }


    }
}
