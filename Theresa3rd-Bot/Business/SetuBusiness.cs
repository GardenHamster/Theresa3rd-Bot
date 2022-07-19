using System.Text;
using Theresa3rd_Bot.Common;

namespace Theresa3rd_Bot.Business
{
    public abstract class SetuBusiness
    {
        public string getDefaultRemindMsg(long groupId, long todayLeftCount)
        {
            StringBuilder warnBuilder = new StringBuilder();
            if (BotConfig.PermissionsConfig.SetuNoneCDGroups.Contains(groupId) == false)
            {
                if (warnBuilder.Length > 0) warnBuilder.Append("，");
                warnBuilder.Append($"{BotConfig.SetuConfig.MemberCD}秒后再来哦");
            }
            if (BotConfig.PermissionsConfig.SetuLimitlessGroups.Contains(groupId) == false && BotConfig.SetuConfig.MaxDaily > 0)
            {
                if (warnBuilder.Length > 0) warnBuilder.Append("，");
                warnBuilder.Append($"今天剩余使用次数{todayLeftCount}次");
            }
            if (BotConfig.SetuConfig.RevokeInterval > 0)
            {
                if (warnBuilder.Length > 0) warnBuilder.Append("，");
                warnBuilder.Append($"本消息将在{BotConfig.SetuConfig.RevokeInterval}秒后撤回，尽快保存哦");
            }
            if (warnBuilder.Length > 0)
            {
                warnBuilder.AppendLine();
            }
            return warnBuilder.ToString();
        }


    }
}
