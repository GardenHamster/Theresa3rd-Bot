using TheresaBot.Core.Dao;
using TheresaBot.Core.Model.PO;

namespace TheresaBot.Core.Services
{
    internal class PixivTagService
    {
        private PixivTagDao pixivTagDao;

        public PixivTagService()
        {
            pixivTagDao = new PixivTagDao();
        }

        public int[] getTagIds(string[] tagArr, bool fullMatch)
        {
            List<int> list = new List<int>();
            foreach (string tagName in tagArr)
            {
                if (string.IsNullOrWhiteSpace(tagName)) continue;
                List<PixivTagPO> tag = pixivTagDao.getTags(tagName, fullMatch);
                if (tag.Count > 0) list.AddRange(tag.Select(o => o.Id).ToList());
            }
            return list.ToArray();
        }






    }
}
