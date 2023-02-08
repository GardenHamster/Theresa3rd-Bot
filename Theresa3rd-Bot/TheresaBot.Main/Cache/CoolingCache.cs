﻿using TheresaBot.Main.Common;
using TheresaBot.Main.Model.Cache;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Cache
{
    public static class CoolingCache
    {
        /// <summary>
        /// 定时涩图任务冷却信息
        /// </summary>
        public static TimingCoolingInfo SetuTimingCoolingInfo = new TimingCoolingInfo();

        /// <summary>
        /// 群员功能冷却字典
        /// </summary>
        public static Dictionary<long, List<MemberCoolingInfo>> MemberCoolingDic = new Dictionary<long, List<MemberCoolingInfo>>();

        /// <summary>
        /// 群功能冷却共享字典
        /// </summary>
        public static Dictionary<long, GroupCoolingInfo> GroupCoolingDic = new Dictionary<long, GroupCoolingInfo>();

        /// <summary>
        /// 判断是否可以触发下一个定时涩图任务
        /// </summary>
        /// <param name="coolingMinutes"></param>
        /// <returns></returns>
        public static bool IsSetuTimingCooling(int coolingMinutes)
        {
            lock (SetuTimingCoolingInfo)
            {
                if (SetuTimingCoolingInfo.LastRunTime is null) return false;
                return DateTime.Now < SetuTimingCoolingInfo.LastRunTime.Value.AddMinutes(coolingMinutes);
            }
        }

        /// <summary>
        /// 记录定时涩图功能最后触发时间，进入CD状态
        /// </summary>
        public static void SetSetuTimingCooling()
        {
            lock (SetuTimingCoolingInfo)
            {
                SetuTimingCoolingInfo.LastRunTime = DateTime.Now;
            }
        }

        /// <summary>
        /// 检查涩图功能是否在冷却中
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public static int GetMemberSetuCD(long groupId, long memberId)
        {
            MemberCoolingInfo coolingInfo = GetMemberCoolingInfo(groupId, memberId);
            return GetCDSeconds(coolingInfo.LastGetSetuTime, BotConfig.SetuConfig.MemberCD);
        }

        /// <summary>
        /// 记录涩图功能最后使用时间，进入CD状态
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="memberId"></param>
        public static void SetMemberSetuCooling(long groupId, long memberId)
        {
            lock (MemberCoolingDic)
            {
                MemberCoolingInfo coolingInfo = GetMemberCoolingInfo(groupId, memberId);
                coolingInfo.LastGetSetuTime = DateTime.Now;
            }
        }

        /// <summary>
        /// 检查原图功能是否在冷却中
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public static int GetMemberSaucenaoCD(long groupId, long memberId)
        {
            MemberCoolingInfo coolingInfo = GetMemberCoolingInfo(groupId, memberId);
            return GetCDSeconds(coolingInfo.LastSaucenaoTime, BotConfig.SaucenaoConfig.MemberCD);
        }

        /// <summary>
        /// 记录搜图功能最后使用时间，进入CD状态
        /// </summary>
        /// <param name="groupId"></param>
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
        /// 检查涩图功能是否在共享CD中
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static int GetGroupSetuCD(long groupId)
        {
            GroupCoolingInfo coolingInfo = GetGroupCoolingInfo(groupId);
            return GetCDSeconds(coolingInfo.LastSetuTime, BotConfig.SetuConfig.GroupCD);
        }

        /// <summary>
        /// 记录涩图功能最后使用时间，进入共享CD状态
        /// </summary>
        /// <param name="groupId"></param>
        public static void SetGroupSetuCooling(long groupId)
        {
            lock (GroupCoolingDic)
            {
                GroupCoolingInfo coolingInfo = GetGroupCoolingInfo(groupId);
                coolingInfo.LastSetuTime = DateTime.Now;
            }
        }

        /// <summary>
        /// 检查日榜功能是否在共享CD中
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static int GetGroupPixivRankingCD(PixivRankingType rankingType, long groupId)
        {
            if (IsNoCD(groupId)) return 0;
            GroupCoolingInfo coolingInfo = GetGroupCoolingInfo(groupId);
            if (coolingInfo.LastPixivRankingTime.ContainsKey(rankingType) == false) return 0;
            DateTime? lastTime = coolingInfo.LastPixivRankingTime[rankingType];
            return GetCDSeconds(lastTime, BotConfig.PixivRankingConfig.GroupCD);
        }

        /// <summary>
        /// 记录日榜功能最后使用时间，进入共享CD状态
        /// </summary>
        /// <param name="groupId"></param>
        public static void SetGroupPixivRankingCooling(PixivRankingType rankingType, long groupId)
        {
            lock (GroupCoolingDic)
            {
                GroupCoolingInfo coolingInfo = GetGroupCoolingInfo(groupId);
                coolingInfo.LastPixivRankingTime[rankingType] = DateTime.Now;
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
        /// <returns></returns>
        public static bool IsPixivRankingHanding(long groupId)
        {
            GroupCoolingInfo coolingInfo = GetGroupCoolingInfo(groupId);
            return coolingInfo.IsPixivRankingHanding;
        }

        /// <summary>
        /// 标记请求处理中
        /// </summary>
        /// <param name="groupId"></param>
        public static void SetPixivRankingHanding(long groupId)
        {
            lock (GroupCoolingDic)
            {
                GroupCoolingInfo coolingInfo = GetGroupCoolingInfo(groupId);
                coolingInfo.IsPixivRankingHanding = true;
            }
        }

        /// <summary>
        /// 标记请求处理完成
        /// </summary>
        /// <param name="groupId"></param>
        public static void SetPixivRankingHandFinish(long groupId)
        {
            lock (GroupCoolingDic)
            {
                GroupCoolingInfo coolingInfo = GetGroupCoolingInfo(groupId);
                coolingInfo.IsPixivRankingHanding = false;
            }
        }



        /// <summary>
        /// 判断是否0CD
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        private static bool IsNoCD(long groupId)
        {
            if (BotConfig.PermissionsConfig.SetuNoneCDGroups.Contains(groupId)) return true;
            return false;
        }

        /// <summary>
        /// 检查时间是否在冷却中
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="secondInterval"></param>
        /// <returns></returns>
        private static int GetCDSeconds(DateTime? datetime, int secondInterval)
        {
            if (datetime is null || secondInterval == 0) return 0;
            TimeSpan timeSpan = new TimeSpan(DateTime.Now.Ticks) - new TimeSpan(datetime.Value.Ticks);
            return timeSpan.TotalSeconds >= secondInterval ? 0 : secondInterval - (int)timeSpan.TotalSeconds;
        }

        /// <summary>
        /// 根据memberId返回冷却信息,如果不存在则创建一个新对象并返回
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
        /// <returns></returns>
        private static GroupCoolingInfo GetGroupCoolingInfo(long groupId)
        {
            lock (GroupCoolingDic)
            {
                if (GroupCoolingDic.ContainsKey(groupId) == false) GroupCoolingDic[groupId] = new GroupCoolingInfo(groupId);
                return GroupCoolingDic[groupId];
            }
        }

    }
}
