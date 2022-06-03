using Theresa3rd_Bot.Common;

namespace Theresa3rd_Bot.Util
{
    public static class PermissionsHelper
    {
        public static bool IsShowR18(this long groupId)
        {
            if (BotConfig.PermissionsConfig?.SetuShowR18Groups == null) return false;
            return BotConfig.PermissionsConfig.SetuShowR18Groups.Contains(groupId);
        }


        public static bool IsShowR18Img(this long groupId)
        {
            if (BotConfig.PermissionsConfig?.SetuShowR18ImgGroups == null) return false;
            return BotConfig.PermissionsConfig.SetuShowR18ImgGroups.Contains(groupId);
        }

    }
}
