using System.Text.RegularExpressions;
using TheresaBot.Main.Business;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.PO;

namespace TheresaBot.Main.Datas
{
    public static class BanTagDatas
    {
        private static List<BanTagPO> BanTagList = new List<BanTagPO>();

        public static void LoadDatas()
        {
            try
            {
                BanTagList = new BanTagBusiness().GetBanTags();
                LogHelper.Info("加载屏蔽标签列表完毕...");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "加载屏蔽标签列表失败...");
            }
        }

        /// <summary>
        /// 返回列表中被禁止的标签
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static List<string> HavingBanTags(this List<string> tags)
        {
            var banTags = new List<string>();
            foreach (string tag in tags)
            {
                if (IsBanTag(tag)) banTags.Add(tag);
            }
            return banTags;
        }

        /// <summary>
        /// 判断是否被禁止的标签
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static bool IsBanTag(this string tag)
        {
            foreach (BanTagPO banTag in BanTagList)
            {
                if (IsBanTag(banTag, tag)) return true;
            }
            return false;
        }

        private static bool IsBanTag(BanTagPO banTag, string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                return false;
            }
            if (banTag.IsRegular)
            {
                return Regex.Match(tag, banTag.KeyWord).Success;
            }
            if (banTag.FullMatch)
            {
                return tag.EqualsIgnoreCase(banTag.KeyWord);
            }
            else
            {
                return tag.ContainsIgnoreCase(banTag.KeyWord);
            }
        }

    }
}
