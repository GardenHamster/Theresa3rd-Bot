using TheresaBot.Main.Business;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Datas
{
    public static class WebsiteDatas
    {
        public static WebsitePO Pixiv { get; private set; }
        public static WebsitePO Bili { get; private set; }
        public static WebsitePO Saucenao { get; private set; }

        public static void LoadWebsite()
        {
            try
            {
                var websiteBusiness = new WebsiteBusiness();
                Pixiv = websiteBusiness.getOrInsertWebsite(Enum.GetName(typeof(WebsiteType), WebsiteType.Pixiv));
                Bili = websiteBusiness.getOrInsertWebsite(Enum.GetName(typeof(WebsiteType), WebsiteType.Bili));
                Saucenao = websiteBusiness.getOrInsertWebsite(Enum.GetName(typeof(WebsiteType), WebsiteType.Saucenao));
                LogHelper.Info("网站cookie加载完成...");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "网站cookie加载失败...");
            }
        }


    }
}
