using TheresaBot.Main.Business;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.PO;

namespace TheresaBot.Main.Datas
{
    public static class BanMemberDatas
    {
        private static List<BanMemberPO> BanMemberList = new List<BanMemberPO>();

        public static void LoadDatas()
        {
            try
            {
                BanMemberList = new BanMemberBusiness().getBanMembers();
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
