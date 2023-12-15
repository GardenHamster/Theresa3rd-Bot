using TheresaBot.Main.Helper;
using TheresaBot.Main.Services;

namespace TheresaBot.Main.Datas
{
    internal static class SugarTagDatas
    {
        public static Dictionary<string, string> SugarTagDic = new Dictionary<string, string>();

        public static void LoadDatas()
        {
            try
            {
                SugarTagDic = new SugarTagService().LoadSugarTags();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }

        /// <summary>
        /// 将标签糖转换为实际上pixiv中搜索的标签
        /// </summary>
        /// <param name="tagStr"></param>
        /// <returns></returns>
        public static string ToActualPixivTags(this string tagStr)
        {
            var tagUpper = tagStr.Trim().ToUpper();
            if (string.IsNullOrWhiteSpace(tagUpper)) return string.Empty;
            if (SugarTagDic.ContainsKey(tagUpper)) return SugarTagDic[tagUpper];
            return tagStr;
        }

    }
}
