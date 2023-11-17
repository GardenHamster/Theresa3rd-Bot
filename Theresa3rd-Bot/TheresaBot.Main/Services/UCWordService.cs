using TheresaBot.Main.Dao;
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

        public void AddWords(string[] wordArr, long memberId, bool isAuthed)
        {
            UCWordPO wordPO = new UCWordPO
            {
                Word1 = wordArr[0],
                Word2 = wordArr[1],
                CreateMember = memberId,
                IsAuthorized = isAuthed,
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

        public int GetAvailableWordCount()
        {
            return ucWordDao.GetAvailableWordCount();
        }

        public int GetUnauthorizedCount(long memberId)
        {
            return ucWordDao.GetUnauthorizedCount(memberId);
        }

    }
}
