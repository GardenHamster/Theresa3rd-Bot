using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Services;

namespace TheresaBot.Main.Datas
{
    internal static class BanPixiverDatas
    {
        public static List<BanPixiverPO> BanPixiverList = new List<BanPixiverPO>();

        public static void LoadDatas()
        {
            try
            {
                BanPixiverList = new BanPixiverService().getBanPixivers();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "加载屏蔽画师列表失败...");
            }
        }

        public static bool IsBanPixiver(this int pixiverId)
        {
            if (pixiverId <= 0) return false;
            return BanPixiverList.Any(o => o.PixiverId == pixiverId);
        }

        public static bool IsBanPixiver(this string pixiverId)
        {
            return BanPixiverList.Any(o => o.PixiverId.ToString() == pixiverId);
        }

    }
}
