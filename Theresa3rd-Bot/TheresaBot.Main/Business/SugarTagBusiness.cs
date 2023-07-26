using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheresaBot.Main.Dao;
using TheresaBot.Main.Model.PO;

namespace TheresaBot.Main.Business
{
    public class SugarTagBusiness
    {
        private SugarTagDao sugarTagDao;

        public SugarTagBusiness()
        {
            this.sugarTagDao = new SugarTagDao();
        }

        public Dictionary<string, string> GetSugarTags()
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

        public int DelSugarTags(string[] keyWords)
        {
            int count = 0;
            foreach (var keyWord in keyWords)
            {
                count += DelSugarTags(keyWord);
            }
            return count;
        }

        public int DelSugarTags(string keyWord)
        {
            keyWord = keyWord.Trim().ToUpper();
            return sugarTagDao.delSugar(keyWord);
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





    }
}
