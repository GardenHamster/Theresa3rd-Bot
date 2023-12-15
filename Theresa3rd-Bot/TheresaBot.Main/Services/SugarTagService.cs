using TheresaBot.Main.Dao;
using TheresaBot.Main.Model.PO;

namespace TheresaBot.Main.Services
{
    public class SugarTagService
    {
        private SugarTagDao sugarTagDao;

        public SugarTagService()
        {
            sugarTagDao = new SugarTagDao();
        }

        public List<SugarTagPO> GetList()
        {
            return sugarTagDao.getSugars();
        }

        public Dictionary<string, string> LoadSugarTags()
        {
            var tagList = sugarTagDao.getSugars();
            var tagDic = new Dictionary<string, string>();
            foreach (SugarTagPO tag in tagList)
            {
                if (string.IsNullOrWhiteSpace(tag.KeyWord)) continue;
                if (string.IsNullOrWhiteSpace(tag.BindTags)) continue;
                tagDic.Add(tag.KeyWord.Trim().ToUpper(), tag.BindTags.Trim());
            }
            return tagDic;
        }

        public void SetSugarTags(string[] keyWords, string bindTags)
        {
            foreach (string keyWord in keyWords)
            {
                SetSugarTags(keyWord, bindTags);
            }
        }

        public SugarTagPO SetSugarTags(string keyWord, string bindTags)
        {
            keyWord = keyWord.Trim().ToUpper();
            var sugarTag = sugarTagDao.getSugar(keyWord);
            if (sugarTag is null)
            {
                sugarTag = new SugarTagPO();
                sugarTag.KeyWord = keyWord;
                sugarTag.BindTags = bindTags;
                sugarTag = sugarTagDao.Insert(sugarTag);
            }
            else
            {
                sugarTag.BindTags = bindTags;
                sugarTagDao.Update(sugarTag);
            }
            return sugarTag;
        }

        public void DelByKeyword(string[] keyWords)
        {
            foreach (var keyWord in keyWords)
            {
                DelByKeyword(keyWord);
            }
        }

        public int DelByKeyword(string keyWord)
        {
            keyWord = keyWord.Trim().ToUpper();
            return sugarTagDao.delSugar(keyWord);
        }

        public int DelById(int id)
        {
            return sugarTagDao.DeleteById(id);
        }

        public int DelById(int[] id)
        {
            return sugarTagDao.DeleteByIds(id);
        }

    }
}
