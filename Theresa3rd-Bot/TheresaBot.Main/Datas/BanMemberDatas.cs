using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Services;

namespace TheresaBot.Main.Datas
{
    internal static class BanMemberDatas
    {
        private static List<BanMemberPO> BanMemberList = new List<BanMemberPO>();

        public static void LoadDatas()
        {
            try
            {
                BanMemberList = new BanMemberService().getBanMembers();
                LogHelper.Info("加载屏蔽用户列表完毕...");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "加载屏蔽用户列表失败...");
            }
        }

        public static bool IsBanMember(this long MemberId)
        {
            if (MemberId <= 0) return false;
            return BanMemberList.Any(o => o.MemberId == MemberId);
        }


    }
}
