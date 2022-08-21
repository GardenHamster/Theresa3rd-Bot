using System;
using System.Text;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Model.Pixiv;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Business
{
    public abstract class SetuBusiness
    {
        /// <summary>
        /// 获取色图默认提示信息
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="todayLeftCount"></param>
        /// <returns></returns>
        public string getDefaultRemindMsg(long groupId, long todayLeftCount)
        {
            StringBuilder remindBuilder = new StringBuilder();
            if (BotConfig.PermissionsConfig.SetuNoneCDGroups.Contains(groupId) == false)
            {
                if (remindBuilder.Length > 0) remindBuilder.Append("，");
                remindBuilder.Append($"{BotConfig.SetuConfig.MemberCD}秒后再来哦");
            }
            if (BotConfig.PermissionsConfig.SetuLimitlessGroups.Contains(groupId) == false && BotConfig.SetuConfig.MaxDaily > 0)
            {
                if (remindBuilder.Length > 0) remindBuilder.Append("，");
                remindBuilder.Append($"今天剩余使用次数{todayLeftCount}次");
            }
            if (BotConfig.SetuConfig.RevokeInterval > 0)
            {
                if (remindBuilder.Length > 0) remindBuilder.Append("，");
                remindBuilder.Append($"本消息将在{BotConfig.SetuConfig.RevokeInterval}秒后撤回，尽快保存哦");
            }
            if (remindBuilder.Length > 0)
            {
                remindBuilder.Append("\r\n");
            }
            return remindBuilder.ToString();
        }

        /// <summary>
        /// 根据配置文件设置的图片大小获取图片下载地址
        /// </summary>
        /// <param name="pixivWorkInfo"></param>
        /// <returns></returns>
        public string getDownImgUrl(string originalUrl)
        {
            string imgSize = BotConfig.GeneralConfig.PixivImgSize?.ToLower();
            if (imgSize == "original") return originalUrl;
            if (imgSize == "regular") return originalUrl.ToRegularUrl();
            if (imgSize == "small") return originalUrl.ToSmallUrl();
            if (imgSize == "thumb") return originalUrl.ToThumbUrl();
            return originalUrl.ToThumbUrl();
        }

    }
}
