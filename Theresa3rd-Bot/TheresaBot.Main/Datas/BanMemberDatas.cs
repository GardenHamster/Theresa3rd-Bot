using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Services;

namespace TheresaBot.Main.Datas
{
    internal static class BanMemberDatas
    {
        public static List<BanMemberPO> BanMemberList = new List<BanMemberPO>();

        public static void LoadDatas()
        {
            try
            {
                BanMemberList = new BanMemberService().GetBanMembers();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "加载屏蔽用户列表失败...");
            }
        }

        public static bool IsBanMember(this long memberId)
        {
            if (memberId <= 0) return false;
            return BanMemberList.Any(o => o.MemberId == memberId);
        }

    }
}
