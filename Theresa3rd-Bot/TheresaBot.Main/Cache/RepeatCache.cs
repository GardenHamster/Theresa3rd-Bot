using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Cache;

namespace TheresaBot.Main.Cache
{
    public static class RepeatCache
    {
        /// <summary>
        /// 复读功能字典
        /// </summary>
        public static Dictionary<long, List<RepeatInfo>> MemberRepeatDic = new Dictionary<long, List<RepeatInfo>>();

        /// <summary>
        /// 判断是否可以复读
        /// </summary>
        /// <param name="e"></param>
        /// <param name="groupId"></param>
        /// <param name="memberId"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        public static bool CheckCanRepeat(long groupId, long botId, long memberId, string word)
        {
            lock (MemberRepeatDic)
            {
                try
                {
                    if (BotConfig.RepeaterConfig.Enable == false) return false;
                    if (BotConfig.RepeaterConfig.RepeatTime == 0) return false;
                    if (MemberRepeatDic.ContainsKey(groupId) == false) MemberRepeatDic[groupId] = new List<RepeatInfo>();
                    RepeatInfo memberRepeat = new RepeatInfo(memberId, word);
                    List<RepeatInfo> memberRepeats = MemberRepeatDic[groupId];
                    RepeatInfo lastRepeat = memberRepeats.LastOrDefault();
                    if (lastRepeat != null && lastRepeat.Word != memberRepeat.Word)
                    {
                        memberRepeats.Clear();
                    }
                    RepeatInfo sameRepeat = memberRepeats.Where(o => o.MemberId == memberId).FirstOrDefault();
                    if (sameRepeat != null)
                    {
                        return false;
                    }
                    RepeatInfo robotRepeat = memberRepeats.Where(o => o.MemberId == botId).FirstOrDefault();
                    if (robotRepeat != null)
                    {
                        return false;
                    }
                    memberRepeats.Add(memberRepeat);
                    if (memberRepeats.Count < BotConfig.RepeaterConfig.RepeatTime)
                    {
                        return false;
                    }
                    memberRepeats.Add(new RepeatInfo(botId, word));
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
