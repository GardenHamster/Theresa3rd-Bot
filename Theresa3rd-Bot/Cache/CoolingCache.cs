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
        public static void SetMemberSTCooling(long groupId, long memberId)
        {
            lock (MemberCoolingDic)
            {
                MemberCoolingInfo coolingInfo = GetMemberCoolingInfo(groupId, memberId);
                coolingInfo.LastGetSTTime = DateTime.Now;
            }
        }

        /// <summary>
        /// 记录查找原图功能最后使用时间,进入CD状态
        /// </summary>
        /// <param name="memberId"></param>
        public static void SetMemberSaucenaoCooling(long groupId, long memberId)
        {
            lock (MemberCoolingDic)
            {
                MemberCoolingInfo coolingInfo = GetMemberCoolingInfo(groupId, memberId);
                coolingInfo.LastSaucenaoTime = DateTime.Now;
            }
        }

        /// <summary>
        /// 检查瑟图功能是否在冷却中
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public static int GetMemberSTCooling(long groupId, long memberId)
        {
            if (IsNoCool(groupId, memberId)) return 0;
            MemberCoolingInfo coolingInfo = GetMemberCoolingInfo(groupId, memberId);
            return GetCoolingSeconds(coolingInfo.LastGetSTTime, BotConfig.SetuConfig.MemberCD);
        }

        /// <summary>
        /// 检查查找原图功能是否在冷却中
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public static int GetMemberSaucenaoCooling(long groupId, long memberId)
        {
            if (IsNoCool(groupId, memberId)) return 0;
            MemberCoolingInfo coolingInfo = GetMemberCoolingInfo(groupId, memberId);
            return GetCoolingSeconds(coolingInfo.LastSaucenaoTime, BotConfig.SaucenaoConfig.MemberCD);
        }

        /// <summary>
        /// 检查共享
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="memberId"></param>
        /// <param name="ignoreNoCoolGroup"></param>
        /// <returns></returns>
        public static int GetGroupSTCooling(long groupId, long memberId, bool ignoreNoCoolGroup = false)
        {
            if (IsNoCool(groupId, memberId)) return 0;
            GroupCoolingInfo coolingInfo = GetGroupCoolingInfo(groupId, memberId);
            return GetCoolingSeconds(coolingInfo.LastGetSTTime, BotConfig.SetuConfig.GroupCD);
        }


        /// <summary>
        /// 检查查找原图功能是否在冷却中
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public static int GetGroupSaucenaoCooling(long groupId, long memberId, bool ignoreNoCoolGroup = false)
        {
            if (IsNoCool(groupId, memberId)) return 0;
            GroupCoolingInfo coolingInfo = GetGroupCoolingInfo(groupId, memberId);
            return GetCoolingSeconds(coolingInfo.LastSaucenaoTime, BotConfig.SaucenaoConfig.GroupCD);
        }

        /// <summary>
        /// 记录群瑟图功能最后使用时间,进入CD状态
        /// </summary>
        /// <param name="memberId"></param>
        public static void SetGroupSTCooling(long groupId, long memberId)
        {
            lock (MemberCoolingDic)
            {
                GroupCoolingInfo coolingInfo = GetGroupCoolingInfo(groupId, memberId);
                coolingInfo.LastGetSTTime = DateTime.Now;
            }
        }

        /// <summary>
        /// 记录群查找原图功能最后使用时间,进入CD状态
        /// </summary>
        /// <param name="memberId"></param>
        public static void SetGroupOriginalCooling(long groupId, long memberId)
        {
            lock (MemberCoolingDic)
            {
                GroupCoolingInfo coolingInfo = GetGroupCoolingInfo(groupId, memberId);
                coolingInfo.LastSaucenaoTime = DateTime.Now;
            }
        }

        /// <summary>
        /// 判断是否0CD
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        private static bool IsNoCool(long groupId, long memberId)
        {
            if (BotConfig.PermissionsConfig.SetuNoneCDGroups.Contains(groupId)) return true;
            return false;
        }

        /// <summary>
        /// 检查时间是否在冷却中
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="minInterval"></param>
        /// <returns></returns>
        private static int GetCoolingSeconds(DateTime? datetime, int secondInterval)
        {
            if (datetime == null || secondInterval == 0) return 0;
            TimeSpan timeSpan = new TimeSpan(DateTime.Now.Ticks) - new TimeSpan(datetime.Value.Ticks);
            return timeSpan.TotalSeconds >= secondInterval ? 0 : secondInterval - (int)timeSpan.TotalSeconds;
        }

        /// <summary>
        /// 标记请求处理中
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="memberId"></param>
        public static void SetHanding(long groupId, long memberId)
        {
            lock (MemberCoolingDic)
            {
                MemberCoolingInfo coolingInfo = GetMemberCoolingInfo(groupId, memberId);
                coolingInfo.Handing = true;
            }
        }

        /// <summary>
        /// 标记请求处理完成
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="memberId"></param>
        public static void SetHandFinish(long groupId, long memberId)
        {
            lock (MemberCoolingDic)
            {
                MemberCoolingInfo coolingInfo = GetMemberCoolingInfo(groupId, memberId);
                coolingInfo.Handing = false;
            }
        }

        /// <summary>
        /// 是否有请求在处理中
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public static bool IsHanding(long groupId, long memberId)
        {
            MemberCoolingInfo coolingInfo = GetMemberCoolingInfo(groupId, memberId);
            return coolingInfo.Handing;
        }


        /// <summary>
        /// 根据qqid返回冷却信息,如果不存在则创建一个新对象并返回
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        private static MemberCoolingInfo GetMemberCoolingInfo(long groupId, long memberId)
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
        private static GroupCoolingInfo GetGroupCoolingInfo(long groupId, long memberId)
        {
            lock (GroupCoolingDic)
            {
                if (GroupCoolingDic.ContainsKey(groupId) == false) GroupCoolingDic[groupId] = new GroupCoolingInfo(groupId);
                return GroupCoolingDic[groupId];
            }
        }

    }
}
