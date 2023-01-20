using System.Collections.Generic;
using Theresa3rd_Bot.Common;

namespace Theresa3rd_Bot.Util
{
    public static class PermissionsHelper
    {
        /// <summary>
        /// 判断是否存在其中一个群需要下载图片
        /// </summary>
        /// <param name="groupIds"></param>
        /// <param name="isR18Img"></param>
        /// <returns></returns>
        public static bool IsDownSetuImg(this List<long> groupIds, bool isR18Img)
        {
            foreach (long groupId in groupIds)
            {
                if (groupId.IsShowSetuImg(isR18Img)) return true;
            }
            return false;
        }

        /// <summary>
        /// 判断某一个群是否需要下载图片
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="isR18Img"></param>
        /// <returns></returns>
        public static bool IsDownSetuImg(this long groupId, bool isR18Img)
        {
            return groupId.IsShowSetuImg(isR18Img);
        }

        /// <summary>
        /// 判断是否存在其中一个群需要显示AI图
        /// </summary>
        /// <param name="groupIds"></param>
        /// <returns></returns>
        public static bool IsShowAISetu(this List<long> groupIds)
        {
            foreach (long groupId in groupIds)
            {
                if (groupId.IsShowAISetu()) return true;
            }
            return false;
        }

        /// <summary>
        /// 判断某一个群是否需要显示AI图
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static bool IsShowAISetu(this long groupId)
        {
            if (BotConfig.PermissionsConfig?.SetuShowAIGroups == null) return true;
            return BotConfig.PermissionsConfig.SetuShowAIGroups.Contains(groupId);
        }

        /// <summary>
        /// 判断某一个群是否可以显示一张涩图
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="isR18Img"></param>
        /// <returns></returns>
        public static bool IsShowSetuImg(this long groupId, bool isR18Img)
        {
            List<long> SetuShowImgGroups = BotConfig.PermissionsConfig?.SetuShowImgGroups;
            if (SetuShowImgGroups == null) return false;
            if (SetuShowImgGroups.Contains(groupId) == false) return false;
            if (isR18Img) return false;
            return true;
        }

        /// <summary>
        /// 判断某一个群是否可以显示一张Saucenao的搜索结果图
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="isR18Img"></param>
        /// <returns></returns>
        public static bool IsShowSaucenaoImg(this long groupId, bool isR18Img)
        {
            List<long> SetuShowImgGroups = BotConfig.PermissionsConfig?.SetuShowImgGroups;
            if (SetuShowImgGroups == null) return false;
            if (SetuShowImgGroups.Contains(groupId) == false) return false;
            if (isR18Img) return false;
            return true;
        }

        /// <summary>
        /// 判断某一个群是否可以显示R18内容
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static bool IsShowR18Setu(this long groupId)
        {
            if (BotConfig.PermissionsConfig?.SetuShowR18Groups == null) return false;
            return BotConfig.PermissionsConfig.SetuShowR18Groups.Contains(groupId);
        }

        /// <summary>
        /// 判断某一个群是否可以显示R18的Saucenao的搜索结果
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static bool IsShowR18Saucenao(this long groupId)
        {
            if (BotConfig.PermissionsConfig?.SaucenaoR18Groups == null) return false;
            return BotConfig.PermissionsConfig.SaucenaoR18Groups.Contains(groupId);
        }

        /// <summary>
        /// 判断某一个成员是否被设置为超级管理员
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public static bool IsSuperManager(this long memberId)
        {
            if (BotConfig.PermissionsConfig?.SuperManagers == null) return false;
            return BotConfig.PermissionsConfig.SuperManagers.Contains(memberId);
        }

    }
}
