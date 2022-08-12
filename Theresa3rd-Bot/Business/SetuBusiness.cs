using System;
using System.Text;
using Theresa3rd_Bot.Common;

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

        public string getOriginalUrl(string imgUrl)
        {
            imgUrl = imgUrl.Replace("https://i.pixiv.cat", "https://i.pximg.net");
            imgUrl = imgUrl.Replace("https://i.pixiv.re", "https://i.pximg.net");
            return imgUrl;
        }

        public string getProxyUrl(string imgUrl)
        {
            imgUrl = imgUrl.Replace("https://i.pximg.net", BotConfig.GeneralConfig.PixivProxy);
            imgUrl = imgUrl.Replace("https://i.pixiv.cat", BotConfig.GeneralConfig.PixivProxy);
            return imgUrl;
        }


    }
}
