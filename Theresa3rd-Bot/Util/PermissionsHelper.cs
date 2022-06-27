using Theresa3rd_Bot.Common;

namespace Theresa3rd_Bot.Util
{
    public static class PermissionsHelper
    {
        public static bool IsShowR18Setu(this long groupId)
        {
            if (BotConfig.PermissionsConfig?.SetuShowR18Groups == null) return false;
            return BotConfig.PermissionsConfig.SetuShowR18Groups.Contains(groupId);
        }

        public static bool IsShowR18SetuImg(this long groupId)
        {
            if (BotConfig.PermissionsConfig?.SetuShowR18ImgGroups == null) return false;
            return BotConfig.PermissionsConfig.SetuShowR18ImgGroups.Contains(groupId);
        }

        public static bool IsShowR18Saucenao(this long groupId)
        {
            if (BotConfig.PermissionsConfig?.SaucenaoR18Groups == null) return false;
            return BotConfig.PermissionsConfig.SaucenaoR18Groups.Contains(groupId);
        }

        public static bool IsShowR18SaucenaoImg(this long groupId)
        {
            if (BotConfig.PermissionsConfig?.SaucenaoR18ImgGroups == null) return false;
            return BotConfig.PermissionsConfig.SaucenaoR18ImgGroups.Contains(groupId);
        }

        public static bool IsSuperManager(this long memberId)
        {
            if (BotConfig.PermissionsConfig?.SuperManagers == null) return false;
            return BotConfig.PermissionsConfig.SuperManagers.Contains(memberId);
        }

    }
}
