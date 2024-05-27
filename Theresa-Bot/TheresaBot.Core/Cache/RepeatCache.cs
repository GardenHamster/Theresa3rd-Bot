using TheresaBot.Core.Common;
using TheresaBot.Core.Helper;
using TheresaBot.Core.Model.Cache;

namespace TheresaBot.Core.Cache
{
    public static class RepeatCache
    {
        /// <summary>
        /// 复读功能字典
        /// </summary>
        public static Dictionary<long, List<RepeatInfo>> GroupRepeatDic = new Dictionary<long, List<RepeatInfo>>();

        /// <summary>
        /// 判断是否可以复读
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="botId"></param>
        /// <param name="memberId"></param>
        /// <param name="simpleContent"></param>
        /// <returns></returns>
        public static bool CheckCanRepeat(long groupId, long botId, long memberId, string simpleContent)
        {
            lock (GroupRepeatDic)
            {
                try
                {
                    if (BotConfig.RepeaterConfig.Enable == false) return false;
                    if (string.IsNullOrWhiteSpace(simpleContent)) return false;
                    if (BotConfig.RepeaterConfig.RepeatTime == 0) return false;
                    if (GroupRepeatDic.ContainsKey(groupId) == false) GroupRepeatDic[groupId] = new List<RepeatInfo>();
                    RepeatInfo memberRepeat = new RepeatInfo(memberId, simpleContent);
                    List<RepeatInfo> repeatList = GroupRepeatDic[groupId];
                    RepeatInfo lastRepeat = repeatList.LastOrDefault();
                    if (lastRepeat is not null && lastRepeat.SendContent != memberRepeat.SendContent)
                    {
                        repeatList.Clear();
                    }
                    RepeatInfo sameRepeat = repeatList.Where(o => o.MemberId == memberId).FirstOrDefault();
                    if (sameRepeat is not null)
                    {
                        return false;
                    }
                    RepeatInfo botRepeat = repeatList.Where(o => o.MemberId == botId).FirstOrDefault();
                    if (botRepeat is not null)
                    {
                        return false;
                    }
                    repeatList.Add(memberRepeat);
                    if (repeatList.Count < BotConfig.RepeaterConfig.RepeatTime)
                    {
                        return false;
                    }
                    botRepeat = new RepeatInfo(botId, simpleContent);
                    repeatList.Add(botRepeat);//添加bot复读记录，避免重复触发复读
                    return true;
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex, "复读功能异常");
                    return false;
                }
            }
        }

    }
}
