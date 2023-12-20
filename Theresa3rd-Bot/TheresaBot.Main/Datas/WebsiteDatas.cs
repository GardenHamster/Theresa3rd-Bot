using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Services;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Datas
{
    internal static class WebsiteDatas
    {
        public static WebsitePO Pixiv { get; private set; }

        public static WebsitePO BiliBili { get; private set; }

        public static WebsitePO Saucenao { get; private set; }

        public static void LoadWebsite()
        {
            try
            {
                var websiteService = new WebsiteService();
                Pixiv = websiteService.GetOrInsert(WebsiteType.Pixiv.ToString());
                BiliBili = websiteService.GetOrInsert(WebsiteType.BiliBili.ToString());
                Saucenao = websiteService.GetOrInsert(WebsiteType.Saucenao.ToString());
                LogHelper.Info("网站cookie加载完成...");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "网站cookie加载失败...");
            }
        }


    }
}
