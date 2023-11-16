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

        public UCWordPO GetRandomWord(List<long> excludeMembers)
        {
            return ucWordDao.GetRandomWord(excludeMembers);
        }

        public int GetAvailableWordCount()
        {
            return ucWordDao.GetAvailableWordCount();
        }

    }
}
