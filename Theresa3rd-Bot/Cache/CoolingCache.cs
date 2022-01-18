using System;
using System.Collections.Generic;
using System.Linq;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Model.Cache;

namespace Theresa3rd_Bot.Cache
{
    public static class CoolingCache
    {
        /// <summary>
        /// 群员功能冷却字典
        /// </summary>
        public static Dictionary<long, List<MemberCoolingInfo>> MemberCoolingDic = new Dictionary<long, List<MemberCoolingInfo>>();

        /// <summary>
        /// 群功能冷却共享字典
        /// </summary>
        public static Dictionary<long, GroupCoolingInfo> GroupCoolingDic = new Dictionary<long, GroupCoolingInfo>();

        /// <summary>
        /// 记录瑟图功能最后使用时间,进入CD状态
        /// </summary>
        /// <param name="memberId"></param>
        public static void setMemberSTCooling(long groupId, long memberId)
        {
            MemberCoolingInfo coolingInfo = getMemberCoolingInfo(groupId, memberId);
            coolingInfo.LastGetSTTime = DateTime.Now;
        }

        /// <summary>
        /// 记录查找原图功能最后使用时间,进入CD状态
        /// </summary>
        /// <param name="memberId"></param>
        public static void setMemberSaucenaoCooling(long groupId, long memberId)
        {
            MemberCoolingInfo coolingInfo = getMemberCoolingInfo(groupId, memberId);
            coolingInfo.LastSaucenaoTime = DateTime.Now;
        }

        /// <summary>
        /// 检查瑟图功能是否在冷却中
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public static int isMemberSTCooling(long groupId, long memberId)
        {
            if (isNoCool(groupId, memberId)) return 0;
            MemberCoolingInfo coolingInfo = getMemberCoolingInfo(groupId, memberId);
            return isCooling(coolingInfo.LastGetSTTime, BotConfig.SetuConfig.MemberCD);
        }

        /// <summary>
        /// 检查查找原图功能是否在冷却中
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public static int isMemberSaucenaoCooling(long groupId, long memberId)
        {
            if (isNoCool(groupId, memberId)) return 0;
            MemberCoolingInfo coolingInfo = getMemberCoolingInfo(groupId, memberId);
            return isCooling(coolingInfo.LastSaucenaoTime, BotConfig.SaucenaoConfig.MemberCD);
        }

        /// <summary>
        /// 检查共享
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="memberId"></param>
        /// <param name="ignoreNoCoolGroup"></param>
        /// <returns></returns>
        public static int isGroupSTCooling(long groupId, long memberId, bool ignoreNoCoolGroup = false)
        {
            if (isNoCool(groupId, memberId)) return 0;
            GroupCoolingInfo coolingInfo = getGroupCoolingInfo(groupId, memberId);
            return isCooling(coolingInfo.LastGetSTTime, BotConfig.SetuConfig.GroupCD);
        }

        /// <summary>
        /// 检查查找原图功能是否在冷却中
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public static int isGroupSaucenaoCooling(long groupId, long memberId, bool ignoreNoCoolGroup = false)
        {
            if (isNoCool(groupId, memberId)) return 0;
            GroupCoolingInfo coolingInfo = getGroupCoolingInfo(groupId, memberId);
            return isCooling(coolingInfo.LastSaucenaoTime, BotConfig.SaucenaoConfig.GroupCD);
        }

        /// <summary>
        /// 记录群瑟图功能最后使用时间,进入CD状态
        /// </summary>
        /// <param name="memberId"></param>
        public static void setGroupSTCooling(long groupId, long memberId)
        {
            GroupCoolingInfo coolingInfo = getGroupCoolingInfo(groupId, memberId);
            coolingInfo.LastGetSTTime = DateTime.Now;
        }

        /// <summary>
        /// 记录群查找原图功能最后使用时间,进入CD状态
        /// </summary>
        /// <param name="memberId"></param>
        public static void setGroupOriginalCooling(long groupId, long memberId)
        {
            GroupCoolingInfo coolingInfo = getGroupCoolingInfo(groupId, memberId);
            coolingInfo.LastSaucenaoTime = DateTime.Now;
        }

        /// <summary>
        /// 判断是否0CD
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        private static bool isNoCool(long groupId, long memberId)
        {
            if (BotConfig.SetuConfig.NoneCDGroups.Contains(groupId)) return true;
            if (BotConfig.SetuConfig.NoneCDMembers.Contains(memberId)) return true;
            return false;
        }

        /// <summary>
        /// 检查时间是否在冷却中
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="minInterval"></param>
        /// <returns></returns>
        private static int isCooling(DateTime? datetime, int secondInterval)
        {
            if (datetime == null) return 0;
            TimeSpan timeSpan = new TimeSpan(DateTime.Now.Ticks) - new TimeSpan(datetime.Value.Ticks);
            return timeSpan.TotalSeconds >= secondInterval ? 0 : secondInterval - (int)timeSpan.TotalSeconds;
        }

        /// <summary>
        /// 根据qqid返回冷却信息,如果不存在则创建一个新对象并返回
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        private static MemberCoolingInfo getMemberCoolingInfo(long groupId, long memberId)
        {
            lock (MemberCoolingDic)
            {
                if (MemberCoolingDic.ContainsKey(groupId) == false) MemberCoolingDic[groupId] = new List<MemberCoolingInfo>();
                MemberCoolingInfo coolingInfo = MemberCoolingDic[groupId].Where(o => o.MemberId == memberId).FirstOrDefault();
                if (coolingInfo != null) return coolingInfo;
                coolingInfo = new MemberCoolingInfo(memberId);
                MemberCoolingDic[groupId].Add(coolingInfo);
                return coolingInfo;
            }
        }

        /// <summary>
        /// 根据群id返回冷却信息,如果不存在则创建一个新对象并返回
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        private static GroupCoolingInfo getGroupCoolingInfo(long groupId, long memberId)
        {
            lock (GroupCoolingDic)
            {
                if (GroupCoolingDic.ContainsKey(groupId) == false) GroupCoolingDic[groupId] = new GroupCoolingInfo(groupId);
                return GroupCoolingDic[groupId];
            }
        }

        /// <summary>
        /// 标记请求处理中
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="memberId"></param>
        public static void setHanding(long groupId, long memberId)
        {
            MemberCoolingInfo coolingInfo = getMemberCoolingInfo(groupId, memberId);
            coolingInfo.Handing = true;
        }

        /// <summary>
        /// 标记请求处理完成
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="memberId"></param>
        public static void setHandFinish(long groupId, long memberId)
        {
            MemberCoolingInfo coolingInfo = getMemberCoolingInfo(groupId, memberId);
            coolingInfo.Handing = false;
        }

        /// <summary>
        /// 是否有请求在处理中
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public static bool isHanding(long groupId, long memberId)
        {
            MemberCoolingInfo coolingInfo = getMemberCoolingInfo(groupId, memberId);
            return coolingInfo.Handing;
        }

    }
}
